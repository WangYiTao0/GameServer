using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AsServer
{
    internal class ClientPeer
    {
        public Socket ClientSocket { get; set; }

        #region 接收数据

        public delegate void ReceiveCompleted(ClientPeer client, object value);
        /// <summary>
        /// 一格消息解析完成的回调
        /// </summary>
        public ReceiveCompleted ProcessReceiveDataCompleted;

        /// <summary>
        /// 数据缓存区  接收到数据后 存入
        /// </summary>
        private List<byte> dataCache = new List<byte>();

        /// <summary>
        /// 接受的网络异步套接字请求
        /// </summary>
        public SocketAsyncEventArgs ReceiveArgs { get; set; }

        private bool _isProcess = false;



        /// <summary>
        /// 自身处理数据包
        /// </summary>
        /// <param name="packet"></param>
        public void StartReceive(byte[] packet)
        {
            dataCache.AddRange(packet);
            if(!_isProcess)
            {
                ProcessReceiveData();
            }

        }

        /// <summary>
        /// 处理接收的数据
        /// </summary>
        private void ProcessReceiveData()
        {
            _isProcess = true;
            //解析数据包
            byte[] data = EncodeTool.DecodePacket(ref dataCache);

            if(data == null)
            {
                _isProcess = false;
                return;
            }

            //需要转成一个具体的类型 供我们使用
            object value = data;
            //回调给上层
            ProcessReceiveDataCompleted?.Invoke(this, value);  
            //尾递归
            ProcessReceiveData();
        }


        //// 粘包拆包问题  包头 包尾 消息头 消息尾
        //void Test()
        //{
        //    //构造消息
        //    byte[] bt = Encoding.Default.GetBytes("12345");
        //    // 头  消息的长度
        //    int length = bt.Length;
        //    byte[] bt1 = BitConverter.GetBytes(length); 
        //    // 尾巴 消息本身 bt
        //    //bt1 + bt

        //    //读取
        //    // int length  = 前四个字节转成的int
        //    // 然后读取长度数据


        //}
        #endregion
    }
}
