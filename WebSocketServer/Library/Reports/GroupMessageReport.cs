using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketServer.Library.Reports
{
    internal class GroupMessageReport:MessageReport
    {
        public Int64 group_id { get; set; }
        public object anonymous { get; set; }
    }
}
