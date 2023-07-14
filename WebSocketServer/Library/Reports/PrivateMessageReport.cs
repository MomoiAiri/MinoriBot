using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Library.Reports
{
    internal class PrivateMessageReport : MessageReport
    {
        public long target_id { get; set; }
        public int temp_source { get; set; }
    }
}
