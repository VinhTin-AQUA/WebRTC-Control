using WorkerServiceDemo.Models;

namespace WorkerServiceDemo.Processors
{
    public class CommandProcessor
    {
        public async Task<object> ExecuteCommandAsync(ServiceCommand command)
        {
            return command.CommandName switch
            {
                "START_JOB" => await StartJob(command.Parameters),
                "STOP_JOB" => await StopJob(command.Parameters),
                "GET_STATUS" => await GetStatus(command.Parameters),
                "UPDATE_CONFIG" => await UpdateConfig(command.Parameters),
                _ => throw new ArgumentException($"Unknown command: {command.CommandName}")
            };
        }

        private async Task<JobResult> StartJob(Dictionary<string, string> parameters)
        {
            // Validate parameters
            if (!parameters.TryGetValue("jobType", out var jobType))
                throw new ArgumentException("Missing jobType parameter");

            // Giả lập xử lý bất đồng bộ
            await Task.Delay(100);

            return new JobResult
            {
                JobId = Guid.NewGuid().ToString(),
                Status = "Running",
                StartTime = DateTime.UtcNow
            };
        }

        private async Task<object> StopJob(Dictionary<string, string> parameters)
        {
            return parameters.Values.Select(x => x).ToList();
        }

        private async Task<object> GetStatus(Dictionary<string, string> parameters)
        {
            return parameters.Values.Select(x => x).ToList();
        }

        private async Task<object> UpdateConfig(Dictionary<string, string> parameters)
        {
            return parameters.Values.Select(x => x).ToList();
        }
    }
}
