using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketServer.Library.Reports
{
    public class BaseReport
    {
        public Int64 time { get; set; }
        public Int64 self_id { get; set; }
        public string post_type { get; set; }
    }
}
