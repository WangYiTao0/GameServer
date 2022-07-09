﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsServer
{
    /// <summary>
    /// 服务器端
    /// </summary>
    public class ServePeer
    {
        private Socket _serverSocket;

        /// <summary>
        /// 限制客户端连接两的信号量
        /// </summary>
        private Semaphore _acceptSemaphore;

        private ClientPeerPool _clientPeerPool;

        /// <summary>
        /// 用来开启服务器
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="maxCount">最大连接数量</param>
        public void Start(int port, int maxCount)
        {
            try
            {
                _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _acceptSemaphore = new Semaphore(maxCount, maxCount);

                //直接New出 最大对象
                _clientPeerPool = new ClientPeerPool(maxCount);
                ClientPeer tmpClientPeer = null;
                for (int i = 0; i < maxCount; i++)
                {
                    tmpClientPeer = new ClientPeer();

                    tmpClientPeer.ReceiveArgs = new SocketAsyncEventArgs();
                    //注册接收完成的事件
                    tmpClientPeer.ReceiveArgs.Completed += OnReceiveDataCompleted;
                    //等于自身
                    tmpClientPeer.ReceiveArgs.UserToken = tmpClientPeer;
                    tmpClientPeer.ProcessReceiveDataCompleted += OnProcessReceiveDataCompleted;
                    _clientPeerPool.Enqueue(tmpClientPeer);
                }

                _serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));

                _serverSocket.Listen(10);//同时等待的人数

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
            //限制线程的访问
            _acceptSemaphore.WaitOne();

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
            //Socket clientSocket = e.AcceptSocket;
            //保存 
            ClientPeer client = _clientPeerPool.Dequeue();
            client.ClientSocket = e.AcceptSocket;

            //开始接收数据
            StartReceive(client);

            e.AcceptSocket = null;
            StartAccept(e);
        }

        #endregion

        #region 接收数据
        /// <summary>
        /// 开始接收数据
        /// </summary>
        /// <param name="client"></param>
        private void StartReceive(ClientPeer client)
        {
            try
            {
               bool result =  client.ClientSocket.ReceiveAsync(client.ReceiveArgs);
                if(!result)
                {
                    ProcessReceive(client.ReceiveArgs);
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 处理接收的请求
        /// </summary>
        /// <param name="e"></param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            ClientPeer client = e.UserToken as ClientPeer;
            //判断网络消息是否接收成功 && 传输的字节数有值
            if(client.ReceiveArgs.SocketError == SocketError.Success && client.ReceiveArgs.BytesTransferred > 0)
            {
                byte[] packet = new byte[client.ReceiveArgs.BytesTransferred];

                Buffer.BlockCopy(client.ReceiveArgs.Buffer,0,packet,0, client.ReceiveArgs.BytesTransferred);

                //让客户端自身处理数据 自身解析
                client.StartReceive(packet);

                // 尾递归
                StartReceive(client);
            }
            //断开连接了
            else
            {
                //如果没有传输的字节数 就代表断开连接了
                if (client.ReceiveArgs.BytesTransferred == 0)
                {
                    if(client.ReceiveArgs.SocketError == SocketError.Success)
                    {
                        //客户端主动断开连接
                        //TODO
                    }
                    else
                    {
                        //由于网络异常 被动断开连接
                        //TODO
                    }
                }
            }
         }

        /// <summary>
        /// 当接收完成时 触发的事件 
        /// </summary>
        /// <param name="e"></param>
        private void OnReceiveDataCompleted(object sender, SocketAsyncEventArgs e)
        {
            ProcessReceive(e);
        }

        /// <summary>
        /// 一条数据解析完成的数据
        /// </summary>
        /// <param name="client">对应连接的对象</param>
        /// <param name="value">解析出来一格具体能使用的类型</param>
        private void OnProcessReceiveDataCompleted(ClientPeer client, Object value)
        {
            //给应用层 让其使用
            //TODO 

        }

        #endregion

        #region 发送数据
        #endregion

        #region 断开连接
        #endregion
    }
}
