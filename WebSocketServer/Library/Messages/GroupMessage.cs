using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketServer.Library.Messages
{
    public class GroupMessage
    {
        public Int64 group_id { get; set; }
        public string message { get; set; }
        public Boolean auto_escape { get; set; }
    }
}
