using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using StunServer.Hubs;

namespace StunServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WebRTCController : ControllerBase
    {
        private readonly IHubContext<ScreenShareHub> _hubContext;

        public WebRTCController(IHubContext<ScreenShareHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult GetAllConnections()
        {
            var connections = ScreenShareHub.UserConnections.Keys.ToList();
            return Ok(connections);
        }

        [HttpPost("{connectionId}")]
        public async Task<IActionResult> StartScreenShare(string connectionId)
        {
            if (!ScreenShareHub.UserConnections.ContainsKey(connectionId))
                return NotFound("Client not connected.");

            await _hubContext.Clients.Client(connectionId).SendAsync("Command", "StartScreenShare");
            return Ok("Start command sent to client: " + connectionId);
        }
    }
}
