namespace WebSocketService.Middlewares.Model
{
    public class RequestModel
    {
        public string Authorization { get; set; }
        public string Secret { get; set; }
        public string SocketIdPc { get; set; }
        public string Contents { get; set; }
    }
}
