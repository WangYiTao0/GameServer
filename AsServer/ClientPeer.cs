using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AsServer
{
    /// <summary>
    /// 服务端
    /// </summary>
    public class ClientPeer
    {
        public Socket ClientSocket { get; set; }

        public ClientPeer()
        {
            ReceiveArgs = new SocketAsyncEventArgs();
            ReceiveArgs.UserToken = this;
            ReceiveArgs.SetBuffer(new byte[1024], 0, 1024);
            SendArgs = new SocketAsyncEventArgs();
            SendArgs.Completed += OnSendArgsCompleted;
        }


        #region 接收数据

        public delegate void ReceiveCompleted(ClientPeer client, SocketMsg value);
        /// <summary>
        /// 一格消息解析完成的回调
        /// </summary>
        public ReceiveCompleted ProcessReceiveDataCompleted;

        /// <summary>
        /// 数据缓存区  接收到数据后 存入
        /// </summary>
        private List<byte> _dataCache = new List<byte>();

        /// <summary>
        /// 接受的网络异步套接字请求
        /// </summary>
        public SocketAsyncEventArgs ReceiveArgs { get; set; }

        private bool _isReceiveProcess = false;



        /// <summary>
        /// 自身处理数据包
        /// </summary>
        /// <param name="packet"></param>
        public void StartReceive(byte[] packet)
        {
            _dataCache.AddRange(packet);
            if(!_isReceiveProcess)
            {
                ProcessReceiveData();
            }

        }

        /// <summary>
        /// 处理接收的数据
        /// </summary>
        private void ProcessReceiveData()
        {
            _isReceiveProcess = true;
            //解析数据包
            byte[] data = EncodeTool.DecodePacket(ref _dataCache);

            if(data == null)
            {
                _isReceiveProcess = false;
                return;
            }

            //需要转成一个具体的类型 供我们使用
            SocketMsg msg = EncodeTool.DecodeMsg(data);
            //回调给上层
            ProcessReceiveDataCompleted?.Invoke(this, msg);  
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

        #region 发送数据

        private Queue<byte[]> _sendQueue = new Queue<byte[]>();
        private bool _isSendProcess;
        /// <summary>
        /// 发送的异步套接字操作
        /// </summary>
        private SocketAsyncEventArgs SendArgs;

        /// <summary>
        /// 发送时断开连接的回调
        /// </summary>
        /// <param name="client"></param>
        /// <param name="reason"></param>
        public delegate void SendDisconnect(ClientPeer client, string reason);
        public SendDisconnect sendDisConnect;

        /// <summary>
        /// 发送网络消息
        /// </summary>
        /// <param name="opCode">操作码</param>
        /// <param name="subCode">子操作</param>
        /// <param name="value">参数</param>
        public void Send(int opCode, int subCode, object value)
        {
            SocketMsg msg = new SocketMsg(opCode, subCode, value);
            byte[] data = EncodeTool.EncodeMsg(msg);
            byte[] packet = EncodeTool.EncodePacket(data);

            _sendQueue.Enqueue(packet);
            if(!_isSendProcess)
                Send();

        }

        private void Send()
        {
            _isSendProcess = true;
            //如果数据的条数等于0的话 就停止发送
            if(_sendQueue.Count == 0)
            {
                _isSendProcess=false;
                return;
            }

            byte[] packet = _sendQueue.Dequeue();
            //设置消息发送异步对象的发送数据缓冲区
            SendArgs.SetBuffer(packet,0, packet.Length);
            //取出一条数据
            bool result = ClientSocket.SendAsync(SendArgs);
            if(!result)
            {
                ProcessSend();
            }
        }

        private void OnSendArgsCompleted(object sender, SocketAsyncEventArgs e)
        {
            ProcessSend();
        }

        /// <summary>
        /// 当异步发送请求完成时调用
        /// </summary>
        private void ProcessSend()
        {
            //发送有没有错误
            if(SendArgs.SocketError != SocketError.Success)
            {
                //发送 出错了 客户端断开连接
                sendDisConnect(this, SendArgs.SocketError.ToString());
            }
            else
            {
                Send();
            }
        }
        #endregion

        #region 断开连接

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            //清空数据
            _dataCache.Clear();
            _isReceiveProcess= false;
            _sendQueue.Clear();
            _isSendProcess = false;

            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
            ClientSocket = null;
        }
        #endregion


    }
}
