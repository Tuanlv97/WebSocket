using System.Net.WebSockets;
using System.Text;

namespace WebSocketService.Middlewares
{
    public abstract class SocketManagerHandler
    {
        public SocketConnectionManager SocketConnectionManager { get; set; }

        public SocketManagerHandler(SocketConnectionManager socketConnectionManager)
        {
            SocketConnectionManager = socketConnectionManager;
        }

        public virtual async Task OnConnected(WebSocket socket)
        {
            SocketConnectionManager.AddSocket(socket);
        }

        public virtual async Task OnDisconnected(WebSocket socket)
        {
            await SocketConnectionManager.RemoveSocket(SocketConnectionManager.GetId(socket));
        }

        public async Task SendMessageAsync(WebSocket socket, string message)
        {
            if (socket.State != WebSocketState.Open)
                return;

            await socket.SendAsync(buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(message),
                                                                  offset: 0,
                                                                  count: message.Length),
                                   messageType: WebSocketMessageType.Text,
                                   endOfMessage: true,
                                   cancellationToken: CancellationToken.None);
        }

        public async Task SendMessageAsync(string socketId, string message)
        {
            await SendMessageAsync(SocketConnectionManager.GetSocketById(socketId).FirstOrDefault().Value, message);
        }

        public async Task SendMessageToAllAsync(string message)
        {
            foreach (var pair in SocketConnectionManager.GetAll())
            {
                if (pair.Value.State == WebSocketState.Open)
                    await SendMessageAsync(pair.Value, message);
            }
        }

        public async Task SendMessageToDataFindAsync(string message, List<string> keyCodes)
        {
            foreach (var pair in SocketConnectionManager.FindSocketsByIds(keyCodes))
            {
                if (pair.Value.State == WebSocketState.Open)
                    await SendMessageAsync(pair.Value, message);
            }
        }

        public abstract Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);
    }
}
