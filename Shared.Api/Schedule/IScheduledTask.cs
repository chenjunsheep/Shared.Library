using System.Threading;
using System.Threading.Tasks;

namespace Shared.Api.Schedule
{
    public interface IScheduledTask
    {
        /// <summary>
        /// schedule name, it is expected to be unique
        /// </summary>
        string Schedule { get; }

        /// <summary>
        /// excuting frequency by seconds.
        /// null value means excute once.
        /// </summary>
        int? Frequency { get; set; }

        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}