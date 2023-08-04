using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Library.Messages
{
    internal class PrivateMessage
    {
        public long user_id { get; set; }
        public long group_id { get; set; }
        public string message { get; set; }
        public bool auto_escape { get; set; }
    }
}
