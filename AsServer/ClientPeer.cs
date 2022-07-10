﻿using System;
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

        public ClientPeer()
        {
            ReceiveArgs = new SocketAsyncEventArgs();
            ReceiveArgs.UserToken = this;
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

        private bool _isProcess = false;



        /// <summary>
        /// 自身处理数据包
        /// </summary>
        /// <param name="packet"></param>
        public void StartReceive(byte[] packet)
        {
            _dataCache.AddRange(packet);
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
            byte[] data = EncodeTool.DecodePacket(ref _dataCache);

            if(data == null)
            {
                _isProcess = false;
                return;
            }

            //TODO 需要转成一个具体的类型 供我们使用
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

        #region 断开连接

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            //清空数据
            _dataCache.Clear();
            _isProcess= false;
            //TODO 给发送数据预留

            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
            ClientSocket = null;
        }
        #endregion
    }
}
