using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketServer.Library.Reports
{
    internal class PrivateMessageReport:MessageReport
    {
        public Int64 target_id { get; set; }
        public int temp_source { get; set; }
    }
}
