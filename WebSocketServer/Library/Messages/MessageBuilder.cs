using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketServer.Library.Messages
{
    public class MessageBuilder
    {
        public StringBuilder _stringBuilder;
        public MessageBuilder()
        {
            _stringBuilder = new StringBuilder();
        }

        public MessageBuilder WithAt(Int64 user_id)
        {
            _stringBuilder.Append($"[CQ:at,qq={user_id}]");
            return this;
        }
        public MessageBuilder WithAtAll()
        {

            _stringBuilder.Append($"[CQ:at,qq=all]");
            return this;
        }
        public MessageBuilder WithAt(Int64 user_id,string name)
        {
            _stringBuilder.Append($"[CQ:at,qq={user_id},name={name}]");
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileType">0:网页链接，1:本地链接</param>
        /// <returns></returns>
        public MessageBuilder WithImage(string file ,int fileType)
        {
            if (fileType == 0)
            {
                _stringBuilder.Append($"[CQ:image,file={file}]");
            }
            if(fileType == 1)
            {
                _stringBuilder.Append($"[CQ:image,file=file:///{file}]");
            }
            return this;
        }
        public MessageBuilder WithText(string text)
        {
            _stringBuilder.Append(text);
            return this;
        }
        public override string ToString()
        {
            return _stringBuilder.ToString();
        }
    }
}
