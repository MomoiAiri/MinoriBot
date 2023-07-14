using MinoriBot.Enums;
using MinoriBot.Library.Messages;
using MinoriBot.Library.Reports;
using MinoriBot.Utils.MessageProcessing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Utils
{
    internal class PraseMessage
    {
        GroupMessageProcessing groupMessageProcessing;
        public PraseMessage()
        {
            groupMessageProcessing = new GroupMessageProcessing();
        }
        public string ProcessingMessage(string message)
        {
            string result = "";
            try
            {
                object back = PraseJson(message);
                if (back != null)
                {
                    result = JsonConvert.SerializeObject(back);
                }
            }
            catch { }

            return result;
        }
        public object PraseJson(string json)
        {
            BaseReport baseReport = new BaseReport();
            if (json != null)
            {
                baseReport = JsonConvert.DeserializeObject<BaseReport>(json);
            }
            switch (baseReport.post_type)
            {
                case "message":
                    MessageReport messageReport = JsonConvert.DeserializeObject<MessageReport>(json);
                    switch (messageReport.message_type)
                    {
                        case "group":
                            GroupMessageReport groupMessageReport = JsonConvert.DeserializeObject<GroupMessageReport>(json);
                            Console.WriteLine($"群{groupMessageReport.group_id} {groupMessageReport.sender.nickname} : {groupMessageReport.raw_message}");
                            string msg = groupMessageProcessing.GroupMessageProcess(groupMessageReport);
                            if (msg != "")
                            {
                                GroupMessage groupMessage = new GroupMessage() { group_id = groupMessageReport.group_id, message = msg };
                                ReportBack<GroupMessage> reportBack = new ReportBack<GroupMessage>(groupMessage) { action = ActionTypes.send_group_msg.ToString() };
                                return reportBack;
                            }
                            break;
                        case "private":
                            PrivateMessageReport privateMessageReport = JsonConvert.DeserializeObject<PrivateMessageReport>(json);
                            return "";
                            break;
                    }
                    break;
                case "message_sent":
                    break;
                case "request":
                    break;
                case "notice":
                    break;
                case "meta_event":
                    break;
            }
            return null;
        }
    }
}
