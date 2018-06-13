using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Api.Schedule.Instance
{
    public class TaskHeartbeat : IScheduledTask
    {
        private string Domain { get; set; }
        /// <summary>
        /// frequency in seconds
        /// </summary>
        public int? Frequency { get; set; }

        public string Schedule { get { return "Heart Beats"; } }

        public TaskHeartbeat(string domain, int? frequency)
        {
            Domain = domain;
            Frequency = frequency;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(Domain))
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.BaseAddress = new Uri(Domain);
                    var response = client.GetAsync(string.Empty).Result;
                    var responseMsg = response.EnsureSuccessStatusCode();
                    var stringResponse = response.Content.ReadAsStringAsync().Result;
                }

                return Task.Factory.StartNew(() => Log());
            }

            return Task.CompletedTask;
        }

        public virtual void Log() { }
    }
}
