using Microsoft.AspNetCore.SignalR;

namespace StunServer.Hubs
{
    public class ScreenShareHub : Hub
    {
        public static readonly Dictionary<string, string> UserConnections = new();

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            UserConnections[Context.ConnectionId] = Context.ConnectionId;
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
            UserConnections.Remove(Context.ConnectionId);
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.Others.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendSignal(string user, object data)
        {
            await Clients.Others.SendAsync("ReceiveSignal", user, data);
        }
    }
}
