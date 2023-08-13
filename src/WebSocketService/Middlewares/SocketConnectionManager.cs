using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace WebSocketService.Middlewares
{
    public class SocketConnectionManager
    {
        private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public Dictionary<string, WebSocket> GetSocketById(string id)
        {
            var data =  _sockets.Where(c => c.Key == id).ToDictionary(c => c.Key, m => m.Value);
            return data;
        }

        public ConcurrentDictionary<string, WebSocket> GetAll()
        {
            return _sockets;
        }

        public Dictionary<string, WebSocket> FindSocketsByIds(List<string> keyCodes)
        {
            return _sockets.Where(c => keyCodes.Contains(c.Key)).ToDictionary(o => o.Key, m => m.Value);
        }

        public string GetId(WebSocket socket)
        {
            return _sockets.FirstOrDefault(p => p.Value == socket).Key;
        }
        public void AddSocket(WebSocket socket)
        {
            _sockets.TryAdd(CreateConnectionId(), socket);
        }

        public async Task RemoveSocket(string id)
        {
            WebSocket socket;
            _sockets.TryRemove(id, out socket);

            await socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                                    statusDescription: "Closed by the ConnectionManager",
                                    cancellationToken: CancellationToken.None);
        }

        private string CreateConnectionId()
        {
            return Guid.NewGuid().ToString();
        }

    }
}
