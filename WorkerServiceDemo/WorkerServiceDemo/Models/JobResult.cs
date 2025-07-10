namespace WorkerServiceDemo.Models
{
    internal class JobResult
    {
        public string JobId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
    }
}
