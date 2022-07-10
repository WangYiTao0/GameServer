using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] EncodePacket(byte[] data)
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
                    bw.Write(data.Length);
                    //再写入数据
                    bw.Write(data);

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
                //throw new Exception("数据缓存长度不足4 不能构成一格完整的消息");

                return null;
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
                        return null;
                        //throw new Exception("数据长度不够包头约定的长度 不能构成一个完整的消息");
                    }
                    


                    byte[] date = br.ReadBytes(length);

                    // 一下缓存区
                    dataCache.Clear();
                    dataCache.AddRange(br.ReadBytes(dateRemainLength));

                    return date;
                }
            }
        }

        #endregion

        #region 构造发送的 SoccketMsg类
        /// <summary>
        /// 把SocketMsg 类转换成字节数组 发送出去
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] EncodeMsg(SocketMsg msg)
        {
            MemoryStream ms = new MemoryStream();   
            BinaryWriter bw = new  BinaryWriter(ms);
            bw.Write(msg.OpCode);
            bw.Write(msg.SubCode);
            //如果不等于 null 才需要把object 转换成字节数据 存起来
            if(msg.Value != null)
            {
                byte[] valueBytes = EncodeObj(msg.Value);

                bw.Write(valueBytes);
            }

            byte[] data = new byte[ms.Length];
            Buffer.BlockCopy(ms.GetBuffer(), 0, data, 0, (int)ms.Length);
            bw.Close();
            ms.Close();
            return data;
        }

        /// <summary>
        /// 将收到的字节数组 转换成SocketMessage 对象 供应用层使用
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static SocketMsg DecodeMsg(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryReader br = new BinaryReader(ms);

            SocketMsg msg = new SocketMsg();
            msg.OpCode = br.ReadInt32();
            msg.SubCode = br.ReadInt32();

            //还有个剩余的字节 代表value 有值
            if(ms.Length > ms.Position)
            {
                byte[] valueBates = br.ReadBytes((int)(ms.Length - ms.Position));
                object value = DecodeMsg(valueBates);
                msg.Value = value;
            }
            br.Close();
            ms.Close();
            return msg;
        }

        #endregion

        #region 把一个object类型转换成byte[]

        /// <summary>
        /// 序列化 object对象
        /// obj -> byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] EncodeObj(object value)
        {
            using(MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, value);

                byte[] valueBytes = new byte[ms.Length];
                Buffer.BlockCopy(ms.GetBuffer(), 0, valueBytes, 0, (int)ms.Length);

                return valueBytes;
            }
        }

        /// <summary>
        /// 反序列化对象
        /// byte[] -> obj
        /// </summary>
        /// <param name="valueByte"></param>
        /// <returns></returns>
        public static object DecodeObj(byte[] valueByte)
        {
            using (MemoryStream ms = new MemoryStream(valueByte))
            {
                BinaryFormatter bf = new BinaryFormatter();
                object value = bf.Deserialize(ms);
                return value;
            }
        }

        #endregion
    }
}
