using Shared.Util.Extension;
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

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(Domain))
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    var baseAddress = new Uri(Domain);
                    client.BaseAddress = baseAddress;
                    var response = await client.GetAsync(baseAddress.AddPath("/api/Token"));
                    await LogAsync(response.IsSuccessStatusCode, $"[{(int)response.StatusCode} {response.StatusCode}]  {Schedule}");
                }
            }
        }

        public async virtual Task LogAsync(bool success, string msg)
        {
            await Task.Delay(0);
        }
    }
}
