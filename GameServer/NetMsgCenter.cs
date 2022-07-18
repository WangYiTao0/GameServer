using AsServer;
using GameServer.Logic;
using Protocol.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    /// <summary>
    /// 对网络消息中心进行分类 转发
    /// 如逻辑层 账号模块， 
    /// 数据层 给逻辑提供数据
    /// 缓存层
    /// 模型层
    /// </summary>
    public class NetMsgCenter : IApplication
    {
        IHandler _accountHandle = new AccountHandle();

        public void OnConnect(ClientPeer client)
        {
        }

        public void OnDisconnect(ClientPeer client)
        {
            _accountHandle.OnDisConnect(client);
        }


        public void OnReceive(ClientPeer client, SocketMsg msg)
        {
            switch (msg.OpCode)
            {
                case OpCode.ACCOUNT:
                    _accountHandle.OnReceive(client, msg.SubCode, msg.Value);
                    break;

                default:
                    break;
            }
        }
    }
}
