using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinoriBot.Enums.Sekai;
using OpenAI_API;
using OpenAI_API.Chat;

namespace MinoriBot.Utils.ChatFunction
{
    internal class PicPromtsProcessing
    {
        public static PicPromtsProcessing Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new PicPromtsProcessing();
                    }
                    return instance;
                }
            }
        }
        static OpenAIAPI api; // shorthand
        private static object padlock;
        private static PicPromtsProcessing instance;
        OpenAI_API.Chat.Conversation chat;
        public PicPromtsProcessing() 
        {
            api = new OpenAIAPI("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");
            Initialize();
        }
        public async Task<string> GetRespond(string TalkTo)
        {
            try
            {
                Console.WriteLine("在GetResponse");
                chat.AppendUserInput(TalkTo);
                string response = await chat.GetResponseFromChatbot();
                if (response == null)
                {
                    return "快去请发电狗子修BUG";
                }
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await this.Initialize();
                return "错误报告" + e.Message;

                //chat.AppendUserInput(TalkTo);
                //return await chat.GetResponseFromChatbot();
            }
            return null;
        }
        public async Task<string> Initialize()
        {
            try
            {
                chat = api.Chat.CreateConversation();
                
                chat.AppendSystemMessage("接下里我会给你一段特征描述，请你将这段描述变为分割的英文单词，之间使用半角逗号分隔，不要回复其他东西");

                chat.AppendSystemMessage("白毛红瞳萝莉");
                chat.AppendExampleChatbotOutput("white hair,red eyes,loli,");

                chat.AppendSystemMessage("闭上眼睛的塔什干");
                chat.AppendExampleChatbotOutput("close eyes,Tashkent,");

                string response = await chat.GetResponseFromChatbot();
                Console.WriteLine("回复的promot="+response);
                return response;
            }
            catch (Exception e)
            {
                return "塔什干报废了" + e.Message;
            }

        }

    }
}
