using MinoriBot.Enums.Sekai;
using MinoriBot.Utils.StaticFilesLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Utils.Assistant
{
    public class VirtualConcert
    {
        string currentPath = AppDomain.CurrentDomain.BaseDirectory;
        long currentEventAggregateAt = 0;
        DateTime currentEventEndTime;
        DateTime remindTime;
        bool isRemind = false;
        List<long> userId =new List<long>();
        public VirtualConcert()
        {
            Console.WriteLine("提醒小助手启动");
            
            if (!Directory.Exists(currentPath + "asset/settings"))
            {
                Directory.CreateDirectory(currentPath + "asset/settings");
            }
            if(!File.Exists(currentPath + "asset/settings/remind_users.txt"))
            {
                File.WriteAllText(currentPath + "asset/settings/remind_users.txt", "");
            }
            List<string> userIdstr = File.ReadLines(currentPath + "asset/settings/remind_users.txt").ToList();
            foreach (string s in userIdstr)
            {
                if (long.TryParse(s, out long result))
                {
                    userId.Add(result);
                }
            }
            FileSystemWatcher watcher = new FileSystemWatcher(currentPath + "asset/settings/","remind_users.txt");
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += UpdateUserIds;
            watcher.EnableRaisingEvents = true;
            Thread thread = new Thread(async()=> await RemindWatchConcert());
            thread.Start();
        }
        private void UpdateUserIds(object sender, FileSystemEventArgs e)
        {
            userId.Clear();
            List<string> userIdstr = File.ReadLines(currentPath + "asset/settings/remind_users.txt").ToList();
            foreach(string s in userIdstr)
            {
                if(long.TryParse(s,out long result))
                {
                    userId.Add(result);
                }
            }
            Console.WriteLine("更新提醒人员:\n" + File.ReadAllText(currentPath + "asset/settings/remind_users.txt"));
        }

        private void GetCurrentEventAggregateAt()
        {
            SkEvents currentEvent = SkDataBase.skEvents[SkDataBase.skEvents.Count - 1];
            int index = 2;
            while (Utils.TimeStampToDateTime(currentEvent.startAt) > DateTime.Now)
            {
                currentEvent = SkDataBase.skEvents[SkDataBase.skEvents.Count - index];
                index++;
            }
            long aggregateAtTime = currentEvent.aggregateAt;
            if(aggregateAtTime != currentEventAggregateAt)
            {
                isRemind = false;
            }
            currentEventAggregateAt = aggregateAtTime;
            currentEventEndTime = Utils.TimeStampToDateTime(currentEventAggregateAt);
            remindTime = currentEventEndTime.AddHours(1).AddMinutes(-10);
            Console.WriteLine(currentEventEndTime);
        }
        private async Task RemindWatchConcert()
        {
            
            while (userId.Count!=0)
            {
                GetCurrentEventAggregateAt();
                if (!isRemind)
                {
                    //if(DateTime.Now >remindTime)
                    //{
                        string remindWord = "啤酒烧烤启动，记得看演唱会！！！";
                        foreach (long id in userId) 
                        {
                            await MessageSender.SendPrivateMessage(remindWord, null, id);
                        }
                        isRemind = true;
                    //}
                }
                Thread.Sleep(1000 * 60 * 5);
                
            }
        }
    }
}
