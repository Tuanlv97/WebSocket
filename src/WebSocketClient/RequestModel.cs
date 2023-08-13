using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketClient
{
    public class RequestModel
    {
        public string Authorization { get; set; }
        public string Secret { get; set; }
        public string SocketIdPc { get; set; }
        public string Contents { get; set; }

    }
}
