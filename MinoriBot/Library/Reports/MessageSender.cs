using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Library.Reports
{
    public class MessageSender
    {
        public long user_id { get; set; }
        public string nickname { get; set; }
        public string sex { get; set; }
        public int age { get; set; }
        public long group_id { get; set; }
        public string card { get; set; }
        public string area { get; set; }
        public string level { get; set; }
        public string role { get; set; }
        public string title { get; set; }
    }
}
