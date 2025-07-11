using Microsoft.AspNetCore.Builder;
using WorkerServiceControl.Workers;

namespace WorkerServiceControl
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();

            builder.Services.AddHostedService<Worker>();
            builder.Services.AddWindowsService();
            builder.Services.AddSwaggerGen();


            var host = builder.Build();

            if (host.Environment.IsDevelopment())
            {
                host.UseSwagger();
                host.UseSwaggerUI();
            }


            host.MapControllers();

            host.Run();
        }
    }
}