using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketServer.Library.Messages
{
    public class ReportBack<T>
    {
        public string action { get; set; }//终结点
        //public string @params { get; set; }
        public T @params { get; set; }
        public string echo { get; set; }

        public ReportBack(T value)
        {
            @params = value;
        }
    }
}
