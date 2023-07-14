using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Library.Reports
{
    public class BaseReport
    {
        public long time { get; set; }
        public long self_id { get; set; }
        public string post_type { get; set; }
    }
}
