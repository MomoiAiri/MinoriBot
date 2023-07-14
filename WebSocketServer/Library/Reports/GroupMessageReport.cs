using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Library.Reports
{
    internal class GroupMessageReport : MessageReport
    {
        public long group_id { get; set; }
        public object anonymous { get; set; }
    }
}
