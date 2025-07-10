using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using WorkerServiceDemo.Models;
using WorkerServiceDemo.Processors;

namespace WorkerServiceDemo.Workers
{
    public class CommandProcessingService : BackgroundService
    {
        private readonly CommandProcessor _commandProcessor;

        public CommandProcessingService()
        {
            _commandProcessor = new CommandProcessor();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessIncomingCommands(stoppingToken);
                await Task.Delay(1000, stoppingToken); // Chờ giữa các lần lắng nghe
            }
        }

        private async Task ProcessIncomingCommands(CancellationToken cancellationToken)
        {
            try
            {
                using var pipeServer = new NamedPipeServerStream(
                    "MyAppCommandPipe",
                    PipeDirection.InOut,
                    NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Message,
                    PipeOptions.Asynchronous);

                await pipeServer.WaitForConnectionAsync(cancellationToken);

                try
                {
                    // Đọc dữ liệu từ pipe
                    var buffer = new byte[1024];
                    var bytesRead = await pipeServer.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    var json = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // Xử lý command
                    var response = await ProcessCommand(json);

                    // Gửi phản hồi
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    await pipeServer.WriteAsync(responseBytes, 0, responseBytes.Length, cancellationToken);
                    await pipeServer.FlushAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing command: {ex}");
                }
                finally
                {
                    pipeServer.Disconnect();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in command processing loop: {ex}");
            }
        }

        private async Task<string> ProcessCommand(string jsonCommand)
        {
            try
            {
                var command = JsonSerializer.Deserialize<ServiceCommand>(jsonCommand);

                if (command == null)
                {
                    return "ERROR: Invalid command format";
                }

                // Xử lý command
                var result = await _commandProcessor.ExecuteCommandAsync(command);

                return JsonSerializer.Serialize(new CommandResponse
                {
                    Success = true,
                    Message = "Command executed successfully",
                    Data = result
                });
            }
            catch (JsonException ex)
            {
                return JsonSerializer.Serialize(new CommandResponse
                {
                    Success = false,
                    Message = $"Invalid JSON format: {ex.Message}"
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new CommandResponse
                {
                    Success = false,
                    Message = $"Command processing failed: {ex.Message}"
                });
            }
        }
    }
}
