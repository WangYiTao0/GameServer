using AsServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol.Code;
using Protocol.Dto;

namespace GameServer.Logic
{
    
    /// <summary>
    /// 账号处理的逻辑层
    /// </summary>
    internal class AccountHandle : IHandler
    {
        public void OnDisConnect(ClientPeer client)
        {
            throw new NotImplementedException();
        }

        public void OnReceive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case AccountCode.REGISTER_CLIENT_REQUEST:
                {
                    AccountDto accountDto = value as AccountDto;
                    // Console.WriteLine(accountDto.Account);
                    // Console.WriteLine(accountDto.Password);
                }
                    break;
                case AccountCode.LOGIN:
                {
                    AccountDto accountDto = value as AccountDto;
                    // Console.WriteLine(accountDto.Account);
                    // Console.WriteLine(accountDto.Password);
                }
                    break;
                default:
                    break;
            }
        }
    }
}
