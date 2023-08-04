using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Library.Reports
{
    internal class MessageReport : BaseReport
    {
        public string message_type { get; set; }
        public string sub_type { get; set; }
        public int message_id { get; set; }
        public long user_id { get; set; }
        public string message { get; set; }
        public string raw_message { get; set; }
        public int font { get; set; }
        public MessageSender sender { get; set; }

    }
}
