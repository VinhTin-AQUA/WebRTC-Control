//using System.Text;
//using System.Text.Json;

//namespace StunServer.Services
//{
//    public class MouseRemoteHandler
//    {
//        private static readonly string workerServiceUrl = "http://localhost:5000/api";

//        public async Task SendCoordinatesToWorkerService(int x, int y)
//        {
//            var coordinate = new
//            {
//                X = x,
//                Y = y
//            };

//            string jsonData = JsonSerializer.Serialize(coordinate);
//            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

//            using HttpClient client = new();
//            try
//            {
//                HttpResponseMessage response = await client.PostAsync($"{workerServiceUrl}/Cursor/SendRightClick", content);
//                string responseContent = await response.Content.ReadAsStringAsync();

//                Console.WriteLine($"Status Code: {response.StatusCode}");
//                Console.WriteLine("Response Body:");
//                Console.WriteLine(responseContent);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Lỗi: " + ex.Message);
//            }
//        }
//    }
//}
