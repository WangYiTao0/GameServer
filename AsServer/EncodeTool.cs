using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsServer
{
    /// <summary>
    /// 关于编码的工具类
    /// </summary>
    public static class EncodeTool
    {
        #region 粘包拆包问题 封装一格有规定的数据包

        /// <summary>
        /// 构造消息体 消息头 + 消息尾
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] EncodePacket(byte[] value)
        {
            //内存流对象
            //要记得释放对象
            //ms.Close();
            //bw.Close();
            //using 关键字 自动释放资源
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    //先写入长度
                    bw.Write(value.Length);
                    //再写入数据
                    bw.Write(value);

                    byte[] byteArray = new byte[(int)ms.Length];

                    //使用Buffer类进行数据操作
                    Buffer.BlockCopy(ms.GetBuffer(), 0, byteArray, 0, (int)ms.Length);

                    return byteArray;
                }
            }
        }

        /// <summary>
        /// 解析消息体 从缓存里取出一格一格完整的数据包
        /// </summary>
        /// <returns></returns>
        public static byte[] DecodePacket(ref List<byte> dataCache)
        {

            if(dataCache.Count < 4)
            {
                //四个字节构成的int 长度 不能构成一格完整的消息
                throw new Exception("数据缓存长度不足4 不能构成一格完整的消息");
            }

            using (MemoryStream ms = new MemoryStream(dataCache.ToArray()))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    //先写入长度 ReadInt32 读取前面四个字节
                    int length = br.ReadInt32();
                    //ms.Length 这个数据的总长度 ms.Position 当前读取到的位置
                    // %1111&1111 读取四个字节的长度
                    int dateRemainLength = (int)(ms.Length - ms.Position);
                    if (length > dateRemainLength)
                    {
                        throw new Exception("数据长度不够包头约定的长度 不能构成一个完整的消息");
                    }
                    


                    byte[] date = br.ReadBytes(length);

                    // 保存一下缓存区
                    dataCache.Clear();
                    dataCache.AddRange(br.ReadBytes(dateRemainLength));

                    return date;
                }
            }

            return;
        }

        #endregion
    }
}
