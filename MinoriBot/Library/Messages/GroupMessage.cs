using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Library.Messages
{
    public class GroupMessage
    {
        public long group_id { get; set; }
        public string message { get; set; }
        public bool auto_escape { get; set; }
    }
}
