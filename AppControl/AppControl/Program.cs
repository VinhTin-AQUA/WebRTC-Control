
using System.IO.Pipes;

namespace AppControl
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("App listening for coordinates...");
            while (true)
            {
                using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("SendRightClick", PipeDirection.In))
                {
                    pipeServer.WaitForConnection();
                    using (StreamReader reader = new StreamReader(pipeServer))
                    {
                        string? message = reader.ReadLine(); // Expect format: "x,y"

                        if (string.IsNullOrEmpty(message))
                        {
                            Console.WriteLine("Message null");
                            continue;
                        }

                        if (!string.IsNullOrEmpty(message) && message.Contains(","))
                        {
                            var parts = message.Split(',');
                            if (int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
                            {
                                Console.WriteLine($"Received click at ({x}, {y})");
                                //Actions.RightMouseClick(x, y);
                                Actions.LeftMouseClick(x, y);
                            }
                        }
                    }
                }
            }

        }
    }
}
