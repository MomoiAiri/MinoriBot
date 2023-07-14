using MinoriBot.Library.Messages;
using MinoriBot.Library.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Utils.MessageProcessing
{
    internal class GroupMessageProcessing
    {
        public string GroupMessageProcess(GroupMessageReport groupMessageReport)
        {
            string rawMessage = groupMessageReport.raw_message;
            if (rawMessage.StartsWith("/"))
            {
                string command = rawMessage.Substring(1);
                if (command == "help")
                {
                    MessageBuilder messageBuilder = new MessageBuilder();
                    messageBuilder.WithAt(groupMessageReport.user_id).WithText("暂无帮助");
                    return messageBuilder.ToString();
                }
                if (command == "wiki")
                {
                    MessageBuilder messageBuilder = new MessageBuilder();
                    messageBuilder.WithAt(groupMessageReport.user_id).WithText("暂无wiki");
                    return messageBuilder.ToString();
                }
                if (command.StartsWith("cq"))
                {
                    string cqCode = HTMLCodeEscape(rawMessage.Split(' ')[1]);
                    MessageBuilder messageBuilder = new MessageBuilder();
                    messageBuilder.WithText(cqCode);
                    return messageBuilder.ToString();
                }
            }

            return "";
        }
        private string HTMLCodeEscape(string str)
        {
            return WebUtility.HtmlDecode(str);
        }
    }
}
