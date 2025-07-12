
using AppControl.Controls;
using AppControl.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace AppControl
{
    public class Program
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

                // trả lời preflight (OPTIONS)
                if (request.HttpMethod == "OPTIONS")
                {
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Close();
                    continue;
                }

                if (request.HttpMethod == "POST" && request.Url != null && request.Url.AbsolutePath == "/handle-left-click")
                {
                    await HandleLeftMouseClick(request, response);
                }
                else if (request.HttpMethod == "POST" && request.Url != null && request.Url.AbsolutePath == "/handle-right-click")
                {
                    await HandleRightMouseClick(request, response);
                }
                else if (request.HttpMethod == "POST" && request.Url != null && request.Url.AbsolutePath == "/handle-mouse-scroll")
                {
                    await HandleMouseScroll(request, response);
                }
                else if (request.HttpMethod == "POST" && request.Url != null && request.Url.AbsolutePath == "/handle-keyboard")
                {
                    await HandleKeyboard(request, response);
                }
                else
                {
                    // 404 Not Found
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                }

                response.OutputStream.Close();
            }
        }

        public static async Task HandleLeftMouseClick(HttpListenerRequest request, HttpListenerResponse response)
        {
            string body;
            using (var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding))
            {
                body = await reader.ReadToEndAsync();
            }

            Console.WriteLine($"Received POST at /handle-left-click: {body}");

            // Chuyển JSON thành đối tượng C#
            var data = JsonSerializer.Deserialize<MouseClick>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data != null)
            {
                MouseClickControl.LeftMouseClick(data.X, data.Y);
            }

            var responseObject = new
            {
                status = "success",
                message = "Data received at /handle-left-click",
                timestamp = DateTime.UtcNow
            };

            string json = JsonSerializer.Serialize(responseObject);
            byte[] buffer = Encoding.UTF8.GetBytes(json);

            response.ContentLength64 = buffer.Length;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.OK;

            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task HandleRightMouseClick(HttpListenerRequest request, HttpListenerResponse response)
        {
            string body;
            using (var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding))
            {
                body = await reader.ReadToEndAsync();
            }

            Console.WriteLine($"Received POST at /data: {body}");

            // Chuyển JSON thành đối tượng C#
            var data = JsonSerializer.Deserialize<MouseClick>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data != null)
            {
                MouseClickControl.RightMouseClick(data.X, data.Y);
            }

            var responseObject = new
            {
                status = "success",
                message = "Data received at /handle-right-click",
                timestamp = DateTime.UtcNow
            };

            string json = JsonSerializer.Serialize(responseObject);
            byte[] buffer = Encoding.UTF8.GetBytes(json);

            response.ContentLength64 = buffer.Length;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.OK;

            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task HandleMouseScroll(HttpListenerRequest request, HttpListenerResponse response)
        {
            string body;
            using (var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding))
            {
                body = await reader.ReadToEndAsync();
            }

            Console.WriteLine($"Received POST at /data: {body}");

            // Chuyển JSON thành đối tượng C#
            var data = JsonSerializer.Deserialize<MouseScroll>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data != null)
            {
                // di chuyển trỏ chuột đến vị trí cần scroll
                //ActionControls.SetMousePos(data.MouseX, data.MouseY);

                // scroll
                ScrollControls.ScrollHorizontal(data.DeltaX);
                ScrollControls.ScrollVerticle(data.DeltaY);
            }

            var responseObject = new
            {
                status = "success",
                message = "Data received at /handle-mouse-scroll",
                timestamp = DateTime.UtcNow
            };

            string json = JsonSerializer.Serialize(responseObject);
            byte[] buffer = Encoding.UTF8.GetBytes(json);

            response.ContentLength64 = buffer.Length;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.OK;

            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task HandleKeyboard(HttpListenerRequest request, HttpListenerResponse response)
        {
            string body;
            using (var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding))
            {
                body = await reader.ReadToEndAsync();
            }

            Console.WriteLine($"Received POST at /data: {body}");

            // Chuyển JSON thành đối tượng C#
            var data = JsonSerializer.Deserialize<Keyboard>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data != null)
            {
                KeyboardControl.SendKey(data.KeyCode);
            }

            var responseObject = new
            {
                status = "success",
                message = "Data received at /handle-keyboard",
                timestamp = DateTime.UtcNow
            };

            string json = JsonSerializer.Serialize(responseObject);
            byte[] buffer = Encoding.UTF8.GetBytes(json);

            response.ContentLength64 = buffer.Length;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.OK;

            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }


    }
}
