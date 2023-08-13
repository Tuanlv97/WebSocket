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
            await SendMessageAsync(socketId, $" {socketId} du lieu di dong sẽ luu vao cache");
        }

        public async Task OnConnectedCustom(WebSocket socket, string socketIdPC, string messageNew)
        {
            await base.OnConnected(socket);
            string dataTomobile = "du lieu lay trong cache tra ve cho mobile";
            await SendMessageAsync(socket, dataTomobile);
            await SendMessageAsync(SocketConnectionManager.GetSocketById(socketIdPC).FirstOrDefault().Value, $"{messageNew} du lieu nay gui ve cho web");
        }


        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var socketId = SocketConnectionManager.GetId(socket);
            var message = $"{socketId} said: {Encoding.UTF8.GetString(buffer, 0, result.Count)}";

            await SendMessageToAllAsync(message);
        }

        public  async Task ReceiveCustomAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var socketId = SocketConnectionManager.GetId(socket);
            var message = $"{socketId} said: {Encoding.UTF8.GetString(buffer, 0, result.Count)}";

            

            await SendMessageAsync(socket, message);
        }
    }
}
