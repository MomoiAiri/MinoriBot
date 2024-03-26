using MinoriBot.Enums;
using MinoriBot.Enums.Bandori;
using MinoriBot.Library.Messages;
using MinoriBot.Library.Reports;
using MinoriBot.Utils.PicFunction;
using MinoriBot.Utils.Routers;
using MinoriBot.Utils.StaticFilesLoader;
using MinoriBot.Utils.View;
using Newtonsoft.Json;
using SkiaSharp;
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
                if (command == "wiki")
                {
                    MessageBuilder messageBuilder = new MessageBuilder();
                    messageBuilder.WithAt(groupMessageReport.user_id).WithText("暂无wiki");
                    return messageBuilder.ToString();
                }
                //if (command.StartsWith("cq"))
                //{
                //    string cqCode = HTMLCodeEscape(rawMessage.Split(' ')[1]);
                //    MessageBuilder messageBuilder = new MessageBuilder();
                //    messageBuilder.WithText(cqCode);
                //    return messageBuilder.ToString();
                //}
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
                if (command.ToLower().StartsWith("sk"))
                {
                    if (command.ToLower().StartsWith("sk查卡池"))
                    {
                        string message = command.Substring(6).ToLower();
                        string file = await SearchGacha.SearchSkGacha(message);
                        if (file == "error") return "无有效关键词";
                        if (file == "none") return "未查询到相关卡池";
                        MessageBuilder messageBuilder = new MessageBuilder();
                        string reply = messageBuilder.WithImage(file, 2).ToString();
                        return reply;
                    }
                    if (command.ToLower().StartsWith("sk卡面"))
                    {
                        string message = command.Substring(5).ToLower();
                        List<MessageObj> files = await SearchCard.GetCardIllustrationImage(message);
                        if (files == null) return "无有效关键词或未查询到相关卡面";
                        MessageBuilder messageBuilder = new MessageBuilder();
                        string reply = string.Empty;
                        for (int i = 0; i < files.Count; i++)
                        {
                            messageBuilder.WithImage(files[i].content, 2);
                        }
                        reply = messageBuilder.ToString();
                        return reply;
                    }
                    if (command.ToLower().StartsWith("sk查卡"))
                    {
                        string message = command.Substring(5).ToLower();
                        MessageObj msg = (await SearchCard.SearchCharacter(message))[0];
                        MessageBuilder messageBuilder = new MessageBuilder();
                        if (msg.type == "image")
                        {
                            messageBuilder.WithImage(msg.content, 2);
                        }
                        else
                        {
                            messageBuilder.WithText(msg.content);
                        }
                        return messageBuilder.ToString();
                    }
                    if (command.ToLower().StartsWith("sk查活动"))
                    {
                        string message = command.Substring(6).ToLower();
                        MessageObj msg = (await SearchEvent.SearchSkEvents(message))[0];
                        MessageBuilder messageBuilder = new MessageBuilder();
                        if (msg.type == "image")
                        {
                            messageBuilder.WithImage(msg.content, 2);
                        }
                        else
                        {
                            messageBuilder.WithText(msg.content);
                        }
                        return messageBuilder.ToString();
                    }
                    if (command.ToLower().StartsWith("sk查曲"))
                    {
                        string message = command.Substring(5).ToLower();
                        MessageObj msg = (await SearchMusic.SearchSkMusics(message))[0];
                        MessageBuilder messageBuilder = new MessageBuilder();
                        if (msg.type == "image")
                        {
                            messageBuilder.WithImage(msg.content, 2);
                        }
                        else
                        {
                            messageBuilder.WithText(msg.content);
                        }
                        return messageBuilder.ToString();
                    }

                    if (command.ToLower().StartsWith("sk查谱面"))
                    {
                        string message = command.Substring(6).ToLower();
                        MessageObj msg = (await SearchChart.SearchSkMusicChart(message))[0];
                        MessageBuilder messageBuilder = new MessageBuilder();
                        if (msg.type == "image")
                        {
                            messageBuilder.WithImage(msg.content, 2);
                        }
                        else
                        {
                            messageBuilder.WithText(msg.content);
                        }
                        return messageBuilder.ToString();
                    }
                }
            }
            if(rawMessage.ToLower() == "pt公式")
            {
                MessageBuilder messageBuilder= new MessageBuilder();
                string file = $"{AppDomain.CurrentDomain.BaseDirectory}/asset/normal/ptformula.png";
                messageBuilder.WithImage(ImageCreater.ConvertBitmapToBase64(SKBitmap.Decode(file)),2);
                return messageBuilder.ToString();
            }
            return "";
        }

    }
}
