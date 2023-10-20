using MinoriBot.Enums;
using MinoriBot.Enums.Bandori;
using MinoriBot.Library.Messages;
using MinoriBot.Library.Reports;
using MinoriBot.Utils.PicFunction;
using MinoriBot.Utils.Routers;
using MinoriBot.Utils.StaticFilesLoader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                    string help = "指令列表：\nsk查卡\nsk查活动";
                    MessageBuilder messageBuilder = new MessageBuilder();
                    messageBuilder.WithAt(groupMessageReport.user_id).WithText(help);
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
                if (command.ToLower() == "sk更新")
                {
                    return await SkDataBase.UpdateDB_Command(Mode.Update);
                }
                if (command.ToLower() == "memory")
                {
                    Process currentProcess = Process.GetCurrentProcess();
                    long menoryUsed = currentProcess.WorkingSet64;
                    double menoryUsedMB = menoryUsed / (1024.0 * 1024.0);
                    return $"当前占用内存：{menoryUsedMB.ToString("F0")}MB";
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
            if (rawMessage.ToLower().StartsWith("sk"))
            {
                if (rawMessage.ToLower().StartsWith("查卡池"))
                {
                    string message = rawMessage.Substring(4).ToLower();
                    string file = await SearchGacha.SearchSkGacha(message);
                    if (file == "error") return "无有效关键词";
                    if (file == "none") return "未查询到相关卡池";
                    MessageBuilder messageBuilder = new MessageBuilder();
                    string reply = messageBuilder.WithImage(file, 2).ToString();
                    return reply;
                }
                if (rawMessage.ToLower().StartsWith("查卡"))
                {
                    string message = rawMessage.Substring(3).ToLower();
                    string file = await SearchCard.SearchCharacter(message);
                    if (file == "error") return "无有效关键词";
                    if (file == "none") return "未查询到相关卡牌";
                    MessageBuilder messageBuilder = new MessageBuilder();
                    string reply = messageBuilder.WithImage(file, 2).ToString();
                    return reply;
                }
                if (rawMessage.ToLower().StartsWith("查活动"))
                {
                    string message = rawMessage.Substring(4).ToLower();
                    string file = await SearchEvent.SearchSkEvents(message);
                    if (file == "error") return "无有效关键词";
                    if (file == "none") return "未查询到相关活动";
                    MessageBuilder messageBuilder = new MessageBuilder();
                    string reply = messageBuilder.WithImage(file, 2).ToString();
                    return reply;
                }
                if (rawMessage.ToLower().StartsWith("查曲"))
                {
                    string message = rawMessage.Substring(3).ToLower();
                    string file = await SearchMusic.SearchSkMusics(message);
                    if (file == "error") return "无有效关键词";
                    if (file == "none") return "未查询到相关歌曲";
                    MessageBuilder messageBuilder = new MessageBuilder();
                    string reply = messageBuilder.WithImage(file, 2).ToString();
                    return reply;
                }
                if (rawMessage.ToLower().StartsWith("卡面"))
                {
                    string message = rawMessage.Substring(3).ToLower();
                    List<string> files = await SearchCard.GetCardIllustrationImage(message);
                    if (files == null) return "无有效关键词或未查询到相关歌曲";
                    MessageBuilder messageBuilder = new MessageBuilder();
                    string reply = string.Empty;
                    for (int i = 0; i < files.Count; i++)
                    {
                        messageBuilder.WithImage(files[i], 0);
                    }
                    reply = messageBuilder.ToString();
                    return reply;
                }
            }
            if (rawMessage.ToLower() == "help" || rawMessage == "帮助")
            {
                string help = "指令列表：\nsk查卡  查询烧烤卡牌\nsk查活动  查询烧烤活动\n注：词条之间使用空格分开";
                MessageBuilder messageBuilder = new MessageBuilder();
                messageBuilder.WithAt(groupMessageReport.user_id).WithText(help);
                return messageBuilder.ToString();
            }
            if(rawMessage.ToLower() == "pt公式")
            {
                MessageBuilder messageBuilder = new MessageBuilder();
                messageBuilder.WithImage($"{AppDomain.CurrentDomain.BaseDirectory}/ptformula.png", 1);
                return messageBuilder.ToString();
            }
            if (rawMessage.ToLower().StartsWith("t10"))
            {
                int eventId = 210;
                if (rawMessage.Length > 3)
                {
                    if(int.TryParse(rawMessage.Substring(4), out int result))
                    {
                        eventId = result;
                    }
                }
                using (var client = new HttpClient())
                {
                    string uri = $"https://bestdori.com/api/eventtop/data?server=3&event={eventId}&mid=0&interval=3600000";
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
