using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.WebSockets;
using WebSocketService.Middlewares.Handlers;
using WebSocketService.Middlewares.Model;

namespace WebSocketService.Middlewares
{
    public class SocketManagerMiddleware
    {
        private readonly RequestDelegate _next;
        private SocketManagerHandler SocketManagerHandler { get; set; }
        private WebSocketMessageHandler WebSocketMessageHandler { get; set; }

        public SocketManagerMiddleware(RequestDelegate next,
                                          SocketManagerHandler socketManagerHandler,
                                          WebSocketMessageHandler webSocketMessageHandler)
        {
            _next = next;
            SocketManagerHandler = socketManagerHandler;
            WebSocketMessageHandler = webSocketMessageHandler;
        }


        public async Task Invoke(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                string jwt = context.Request.Headers["Authorization"];

                var dataRequest = context.Request.Headers["data"];
               var model = JsonConvert.DeserializeObject<RequestModel>(dataRequest);

                if (!string.IsNullOrEmpty(jwt) && jwt.StartsWith("Bearer "))
                {
                    var handler = new JwtSecurityTokenHandler();
                    if (handler.ReadToken(jwt[7..]) is JwtSecurityToken token)
                    {
                        string keyCodePc = model.SocketIdPc;
                        string messageMobile = "Đăng nhập mobile thành công";

                        var socket = await context.WebSockets.AcceptWebSocketAsync();
                        if (string.IsNullOrEmpty(keyCodePc))
                        {
                            await SocketManagerHandler.OnConnected(socket);
                        }
                        else
                        {
                            await WebSocketMessageHandler.OnConnectedCustom(socket, keyCodePc, messageMobile);
                        }

                       
                            await Receive(socket, async (result, buffer) =>
                            {
                                if (result.MessageType == WebSocketMessageType.Text)
                                {
                                    await SocketManagerHandler.ReceiveAsync(socket, result, buffer);
                                    return;
                                }

                                else if (result.MessageType == WebSocketMessageType.Close)
                                {
                                    await SocketManagerHandler.OnDisconnected(socket);
                                    return;
                                }
                            });
                        }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        return;
                    }
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    //await SocketManagerHandler.OnDisconnected(socket);
                    return;
                }

            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }
        }
        public async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                                                       cancellationToken: CancellationToken.None);

                handleMessage(result, buffer);
            }
        }

    }
}
