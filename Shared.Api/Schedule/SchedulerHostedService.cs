using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Api.Schedule
{
    public class SchedulerHostedService : HostedService
    {
        public event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskException;
            
        private readonly List<SchedulerTaskWrapper> _scheduledTasks = new List<SchedulerTaskWrapper>();

        public SchedulerHostedService(IEnumerable<IScheduledTask> scheduledTasks)
        {
            var referenceTime = DateTime.UtcNow;
            
            foreach (var scheduledTask in scheduledTasks)
            {
                _scheduledTasks.Add(new SchedulerTaskWrapper(scheduledTask, referenceTime));
            }
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ExecuteEndlessAsync(cancellationToken);
                
                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }

        private async Task ExecuteEndlessAsync(CancellationToken cancellationToken)
        {
            var taskFactory = new TaskFactory(TaskScheduler.Current);            
            var tasksAvailable = _scheduledTasks.Where(t => t.IsAvailable(DateTime.UtcNow)).ToList();

            foreach (var targetTask in tasksAvailable)
            {
                await taskFactory.StartNew(
                    async () =>
                    {
                        try
                        {
                            await targetTask.RunAsync(cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            var args = new UnobservedTaskExceptionEventArgs(ex as AggregateException ?? new AggregateException(ex));

                            UnobservedTaskException?.Invoke(this, args);

                            if (!args.Observed)
                            {
                                throw;
                            }
                        }
                    },
                    cancellationToken);
            }
        }

        private class SchedulerTaskWrapper
        {
            private IScheduledTask Task { get; set; }

            private DateTime LastRunTime { get; set; }
            private DateTime NextRunTime { get; set; }

            public SchedulerTaskWrapper(IScheduledTask task, DateTime curTime)
            {
                Task = task;
                NextRunTime = curTime;
            }

            public Task RunAsync(CancellationToken cancellationToken)
            {
                LastRunTime = NextRunTime;

                if (Task != null)
                {
                    if (Task.Frequency.HasValue)
                    {
                        NextRunTime = NextRunTime.AddSeconds(Task.Frequency.Value);
                    }
                    return Task.ExecuteAsync(cancellationToken);
                }

                NextRunTime = DateTime.MaxValue;

                return System.Threading.Tasks.Task.CompletedTask;
            }

            public bool IsAvailable(DateTime currentTime)
            {
                return Task != null && NextRunTime < currentTime && LastRunTime != NextRunTime;
            }
        }
    }
}