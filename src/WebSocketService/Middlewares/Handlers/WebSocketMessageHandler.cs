using System.Net.WebSockets;
using System.Text;

namespace WebSocketService.Middlewares.Handlers
{
    public class WebSocketMessageHandler : SocketManagerHandler
    {
        public WebSocketMessageHandler(SocketConnectionManager connections) : base(connections)
        {
        }
        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);
            var socketId = SocketConnectionManager.GetId(socket);
            await SendMessageAsync(socketId, $"{socketId} Ma socket khi call lan dau tien");
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var socketId = SocketConnectionManager.GetId(socket);
            var message = $"{socketId} said: {Encoding.UTF8.GetString(buffer, 0, result.Count)}";

            await SendMessageToAllAsync(message);
        }
    }
}
