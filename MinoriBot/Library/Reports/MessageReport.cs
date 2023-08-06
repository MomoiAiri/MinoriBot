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
        public List<MessageReportMsg> message { get; set; }
        public string raw_message { get; set; }
        public int font { get; set; }
        public MessageSender sender { get; set; }

    }
    public class MessageReportMsg
    {
        public string type { get; set; }
        public List<MessageReportMsgData> messageReportMsgDatas { get; set; }
    }
    public class MessageReportMsgData
    {
        public string text { get; set; }//文本
        public string id { get; set; }//表情id
        public string file { get; set; }//语音文件,短视频文件
        public string qq { get; set; }//QQ号
        public string name { get; set; }//此栏无效，此人在群里
        public string url { get; set; }//链接分享
        public string title { get; set; }//链接标题
        public string audio { get; set; }//音频文件
        public string dtype { get; set; }//表示音乐自定义分享456
    }
}
