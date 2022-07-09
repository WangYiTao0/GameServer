using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsServer
{
    /// <summary>
    /// 客户端的连接池 
    /// 宠用客户端连接对象
    /// 防止过多的new
    /// </summary>
    internal class ClientPeerPool
    {
        private Queue<ClientPeer> _ClientPeerQueue;

        public ClientPeerPool(int capacity)
        {
            _ClientPeerQueue = new Queue<ClientPeer>(capacity);
        }

        public void Enqueue(ClientPeer client)
        {
            _ClientPeerQueue.Enqueue(client);
        }

        public ClientPeer Dequeue()
        {
            return _ClientPeerQueue.Dequeue();
        }
    }
}
