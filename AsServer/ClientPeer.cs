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
        public Socket _clientSocket;

        /// <summary>
        /// 设置连接对象
        /// </summary>
        /// <param name="clientSocket"></param>
        public void SetSocket(Socket clientSocket)
        {
            _clientSocket = clientSocket;

            //粘包和 拆包
            clientSocket.Receive();
        }

        #region 接收数据
        /// <summary>
        /// 数据缓存区  接收到数据后 存入
        /// </summary>
        private List<byte> dataCache = new List<byte>();

        // 粘包拆包问题  包头 包尾 消息头 消息尾
        void Test()
        {
            //构造消息
            byte[] bt = Encoding.Default.GetBytes("12345");
            // 头  消息的长度
            int length = bt.Length;
            byte[] bt1 = BitConverter.GetBytes(length); 
            // 尾巴 消息本身 bt
            //bt1 + bt

            //读取
            // int length  = 前四个字节转成的int
            // 然后读取长度数据


        }
        #endregion
    }
}
