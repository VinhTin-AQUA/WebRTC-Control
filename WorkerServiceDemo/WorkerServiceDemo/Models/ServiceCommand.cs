namespace WorkerServiceDemo.Models
{
    public class ServiceCommand
    {
        public string CommandName { get; set; } = string.Empty;
        public Dictionary<string, string> Parameters { get; set; } = new();
    }
}
