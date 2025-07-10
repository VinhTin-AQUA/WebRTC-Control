
using System.Threading.Tasks;
using WorkerServiceDemo.Processors;
using WorkerServiceDemo.Workers;

namespace WorkerServiceDemo
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddWindowsService(options =>
            {
                options.ServiceName = ".NET Service Demo";
            });
            builder.Services.AddHostedService<CommandProcessingService>();
            builder.Services.AddSingleton<CommandProcessor>();

            var host = builder.Build();
            await host.RunAsync();
        }
    }
}