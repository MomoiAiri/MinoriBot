﻿using MinoriBot.Enums;
using MinoriBot.Library.Messages;
using MinoriBot.Library.Reports;
using MinoriBot.Utils.PicFunction;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MinoriBot.Utils.MessageProcessing
{
    internal class GroupMessageProcessing
    {
        public async Task<string> GroupMessageProcessAsync(GroupMessageReport groupMessageReport,WebSocket ws)
        {
            //new PicFunction.PicProcessing
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
                if (command.Contains("来点")&& command.Contains("涩图"))
                {
                    string text = command;
                    string pattern = @"^来点(.*)涩图$";
                    MessageBuilder messageBuilder = new MessageBuilder();
                    
                    Match match = Regex.Match(text, pattern);

                    if (match.Success)
                    {
                        string promts = match.Groups[1].Value;
                        Console.WriteLine("中间的内容： " + promts);
                        //messageBuilder.WithAt(groupMessageReport.user_id).WithText("正在尝试生成");
                        await SendSetuAsync(groupMessageReport.group_id,promts,ws);
                    }
                    else
                    {
                        //Console.WriteLine("未找到匹配的内容");
                        messageBuilder.WithAt(groupMessageReport.user_id).WithText("有病");
                    }
                    return messageBuilder.ToString();
                }
            }
            return "";
        }

        private async Task SendSetuAsync(long group_id,string promts,WebSocket ws)
        {
            await PicProcessing.Generate(promts);
            MessageBuilder messageBuilder= new MessageBuilder();
            string addr = System.Environment.CurrentDirectory;
            messageBuilder.WithText($@"[CQ:image,file=file:///{addr}/output.png]");
            //messageBuilder.WithText("说的道理");
            GroupMessage groupMessage = new GroupMessage() { group_id = group_id ,message = messageBuilder.ToString()};
            ReportBack<GroupMessage> reportBack = new ReportBack<GroupMessage>(groupMessage) { action = ActionTypes.send_group_msg.ToString() };
            string msg = JsonConvert.SerializeObject(reportBack);
            await MessageSender.SendMessage(messageBuilder.ToString(),ws);
        }

        private string HTMLCodeEscape(string str)
        {
            return WebUtility.HtmlDecode(str);
        }

    }
}