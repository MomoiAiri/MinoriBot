using MinoriBot.Enums;
using MinoriBot.Enums.Bandori;
using MinoriBot.Library.Messages;
using MinoriBot.Library.Reports;
using MinoriBot.Utils.PicFunction;
using MinoriBot.Utils.Routers;
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
            if (rawMessage.ToLower().StartsWith("sk查卡"))
            {
                string message = rawMessage.Substring(5);
                string reply = await SearchInfo.SearchCharacter(message);
                return reply;
            }
            if (rawMessage.ToLower()=="t10")
            {
                using (var client = new HttpClient())
                {
                    string uri = "https://bestdori.com/api/eventtop/data?server=3&event=207&mid=0&interval=3600000";
                    var response = await client.GetAsync(uri);
                    string json = await response.Content.ReadAsStringAsync();
                    LeaderBoard lb = JsonConvert.DeserializeObject<LeaderBoard>(json);
                    StringBuilder sb = new StringBuilder();
                    int index = 1;
                    for(int i = lb.points.Count - 10; i < lb.points.Count; i++)
                    {
                        for(int j =0;j<lb.users.Count;j++)
                        {
                            if (lb.points[i].uid == lb.users[j].uid)
                            {
                                sb.Append($"{index}.{lb.users[j].name}  {lb.points[i].value}pt\n");
                                index++;
                                break;
                            }
                        }
                    }
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(lb.points[lb.points.Count - 1].time);
                    DateTime dateTime = dateTimeOffset.LocalDateTime;
                    sb.Append("更新时间：" + dateTime.ToString("MM-dd HH:mm:ss"));
                    MessageBuilder messageBuilder = new MessageBuilder();
                    messageBuilder.WithText(sb.ToString());
                    return messageBuilder.ToString();
                }
            }
            //if (rawMessage.ToLower() == "ycm")
            //{
            //    Random random= new Random();
            //    int r = random.Next(0, 11);
            //    if (r < 3)
            //    {
            //        return "整天有车吗有车吗就不会自己开个车？";
            //    }
            //    else
            //    {
            //        return "";
            //    }
            //}
            //if(Regex.IsMatch(rawMessage,@"来点(.*?)涩图"))
            //{
            //    using(var client = new HttpClient())
            //    {
            //        Match match = Regex.Match(rawMessage, @"来点(.*?)涩图");
            //        string tag = match.Groups[1].Value;
            //        var response = await client.GetAsync($"https://api.lolicon.app/setu/v2?r18=0&tag={tag}");
            //        Match url = Regex.Match(await response.Content.ReadAsStringAsync(), @"http.*\.(jpg|png)");
            //        MessageBuilder message = new MessageBuilder();
            //        return message.WithImage(url.Value, 0).ToString();
            //    }
            //}
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
