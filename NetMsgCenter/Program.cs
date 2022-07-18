
using AsServer;

namespace GameServer
{
    /// <summary>
    /// 网络消息中心
    /// </summary>

    public class NetMsgCenter : IApplication
    {
        public void OnConnect(ClientPeer client)
        {
            throw new NotImplementedException();
        }

        public void OnDisconnect(ClientPeer client)
        {
            throw new NotImplementedException();
        }

        public void OnReceive(ClientPeer client, SocketMsg msg)
        {
            throw new NotImplementedException();
        }
    }
}