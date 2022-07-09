﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AsServer
{
    /// <summary>
    /// 服务器端
    /// </summary>
    public class ServePeer
    {
        private Socket _serverSocket;

        public ServePeer()
        {
            _serverSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp); 
        }

        /// <summary>
        /// 用来开启服务器
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="maxCount">最大连接数量</param>
        public void Start(int port, int maxCount)
        {
            try
            {
                _serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
                _serverSocket.Listen(maxCount);//同时等待的人数

                Console.WriteLine("服务器启动...");

                StartAccept(null);
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
        }


        #region 接收客户端的连接
        /// <summary>
        /// 开始等待客户端数据
        /// </summary>
        /// <param name="e"></param>
        private void StartAccept(SocketAsyncEventArgs e)
        {
            if(e == null)
            { 
                e = new SocketAsyncEventArgs();
                e.Completed += Accept_Completed;
            }


            // true 等待执行  false 执行完毕
            bool result = _serverSocket.AcceptAsync(e);

            if(!result)
            {
                ProcessAccept(e);
            }
            else
            {
                e.Completed += Accept_Completed;
            }
        }

        private void Accept_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        /// <summary>
        /// 处理连接请求
        /// </summary>
        /// <param name="e"></param>
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            //等到客户端对象
            Socket clientSocket = e.AcceptSocket;
            //保存 
            //TODO
        }

        #endregion

        #region 发送数据
        #endregion

        #region 断开连接
        #endregion

        #region 接收数据
        #endregion
    }
}