using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MinoriBot.Utils
{
    public static class Utils
    {
        public static T DeepCopy<T>(T obj)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", nameof(obj));
            }

            // 创建一个内存流来保存对象的副本
            using (var memoryStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, obj);

                // 移动内存流的位置到开头
                memoryStream.Seek(0, SeekOrigin.Begin);

                // 使用反序列化创建一个新的对象副本
                return (T)formatter.Deserialize(memoryStream);
            }
        }
        public static DateTime TimeStampToDateTime(long timeStamp)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(timeStamp);
            return dateTimeOffset.LocalDateTime;
        }
    }
}
