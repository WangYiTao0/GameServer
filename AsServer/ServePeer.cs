using System;
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
        /// 应用层
        /// </summary>
        private IApplication _application;

        public void SetApplication(IApplication app)
        {
            _application = app;
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
                //IPv4 流传输 TCP
                _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _acceptSemaphore = new Semaphore(maxCount, maxCount);

                //直接New出 最大对象
                _clientPeerPool = new ClientPeerPool(maxCount);
                ClientPeer tmpClientPeer = null;
                for (int i = 0; i < maxCount; i++)
                {
                    tmpClientPeer = new ClientPeer();
                    //注册事件
                    //注册接收完成的事件
                    tmpClientPeer.ReceiveArgs.Completed += OnReceiveDataCompleted;
                    //注册处理收到的数据完成时
                    tmpClientPeer.ProcessReceiveDataCompleted = OnProcessReceiveDataCompleted;
                    //处理发送时断开连接
                    tmpClientPeer.sendDisConnect = Disconnect;
                    _clientPeerPool.Enqueue(tmpClientPeer);
                }

                _serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));

                _serverSocket.Listen(10);//同时等待的人数

                Console.WriteLine("Server Start-up...");

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
                e.Completed += OnAcceptCompleted;
            }
            // true 等待执行  false 执行完毕
            bool result = _serverSocket.AcceptAsync(e);

            if(!result)
            {
                ProcessAccept(e);
            }
            //else
            //{
            //    e.Completed += OnAcceptCompleted;
            //}
        }

        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        /// <summary>
        /// 处理连接请求
        /// </summary>
        /// <param name="e"></param>
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            //限制线程的访问
            _acceptSemaphore.WaitOne();
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
                //拷贝数据到数组
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
                        Disconnect(client, "客户端主动断开连接");
                    }
                    else
                    {
                        //由于网络异常 被动断开连接
                        Disconnect(client, client.ReceiveArgs.SocketError.ToString());
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
        private void OnProcessReceiveDataCompleted(ClientPeer client, SocketMsg msg)
        {
            //给应用层 让其使用
            _application.OnReceive(client, msg);

        }

        #endregion

        #region 发送数据

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="client">断开的客户端连接对象</param>
        /// <param name="reason">断开的原因</param>
        public void Disconnect(ClientPeer client, string reason)
        {
            try
            {
                //清空一些数据
                if(client == null)
                {
                    throw new Exception("当前指定的客户端对象为空，无法断开连接");
                }

                //通知应用层断开连接了
                _application.OnDisconnect(client);
                client.Disconnect();
                //回收对象 方便下次使用
                _clientPeerPool.Enqueue(client);
                _acceptSemaphore.Release();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        #endregion

    }
}
