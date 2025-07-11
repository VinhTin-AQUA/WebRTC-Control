
using System.IO.Pipes;

namespace WorkerServiceControl.Services
{
    public static class Sender
    {
        public static void SendRightClick(int x, int y)
        {
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "SendRightClick", PipeDirection.Out))
            {
                try
                {
                    pipeClient.Connect(1000); // timeout 1s
                    using (StreamWriter writer = new StreamWriter(pipeClient))
                    {
                        writer.AutoFlush = true;
                        writer.WriteLine($"{x},{y}");
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
