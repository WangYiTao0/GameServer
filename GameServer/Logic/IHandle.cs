using AsServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Logic
{
    internal interface IHandler
    {
        void OnReceive(ClientPeer client, int subCode, object value);
        void OnDisConnect(ClientPeer client);
    }
}
