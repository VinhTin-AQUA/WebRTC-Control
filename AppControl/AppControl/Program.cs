
using AppControl.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace AppControl
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5000/");
            listener.Start();
            Console.WriteLine("Listening on http://localhost:5000/");

            while (true)
            {
                var context = await listener.GetContextAsync();
                var request = context.Request;
                var response = context.Response;

                // CORS
                response.AddHeader("Access-Control-Allow-Origin", "*");
                response.AddHeader("Access-Control-Allow-Headers", "Content-Type");
                response.AddHeader("Access-Control-Allow-Methods", "GET, PUT, DELETE, POST, OPTIONS");

                // Trả lời preflight (OPTIONS)
                if (request.HttpMethod == "OPTIONS")
                {
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Close();
                    continue;
                }

                if (request.HttpMethod == "POST" && request.Url != null && request.Url.AbsolutePath == "/handle-click")
                {
                    string body; 
                    using (var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        body = await reader.ReadToEndAsync();
                    }

                    Console.WriteLine($"Received POST at /data: {body}");

                    // Chuyển JSON thành đối tượng C#
                    var data = JsonSerializer.Deserialize<CursorCoordinate>(body, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (data != null)
                    {
                        ActionControls.LeftMouseClick(data.X, data.Y);
                    }

                    var responseObject = new
                    {
                        status = "success",
                        message = "Data received at /handle-click",
                        timestamp = DateTime.UtcNow
                    };

                    string json = JsonSerializer.Serialize(responseObject);
                    byte[] buffer = Encoding.UTF8.GetBytes(json);

                    response.ContentLength64 = buffer.Length;
                    response.ContentType = "application/json"; 
                    response.StatusCode = (int)HttpStatusCode.OK;

                    await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                }
                else
                {
                    // 404 Not Found
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                }

                response.OutputStream.Close();
            }
        }
    }
}
