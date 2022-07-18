using AsServer;
using AsServer.Utilities.Concurrent;
using GameServer.Model;

namespace GameServer.Cache
{
    /// <summary>
    /// 账号缓存
    /// </summary>
    public class AccountCache
    {
        /// <summary>
        /// 账号对应的数据模型
        /// </summary>
        private Dictionary<string, AccountModel> _accountModelDict = new Dictionary<string, AccountModel>();

        /// <summary>
        /// 是否存在账号
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool IsExist(string account)
        {
            return _accountModelDict.ContainsKey(account);
        }

        /// <summary>
        /// 用来存储账号的id
        /// 后期是数据库处理
        /// </summary>
        private ConcurrentInt _id = new ConcurrentInt(-1);
        /// <summary>
        /// 创建账号数据模型信息
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        public void Create(string account, string password)
        {
            var accountModel = new AccountModel(_id.Add_Get(), account, password);
            _accountModelDict.Add(accountModel.Account,accountModel);
        }

        /// <summary>
        /// 获取账号对应的数据模型
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public AccountModel GetModel(string account)
        {
            return _accountModelDict[account];
        }

        /// <summary>
        /// 账号密码是否匹配
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool IsMacth(string account, string password)
        {
            return _accountModelDict[account].Password == password;
        }

        /// <summary>
        /// 账号对应的客户端连接对象
        /// </summary>
        public Dictionary<string, ClientPeer> _accountClientPeerDict = new Dictionary<string, ClientPeer>();
        public Dictionary<ClientPeer, string> _clientPeerAccountDict = new Dictionary<ClientPeer, string>();


        public bool IsOnline(string account)
        {
            return _accountClientPeerDict.ContainsKey(account);
        }

        public bool isOnline(ClientPeer clientPeer)
        {
            return _clientPeerAccountDict.ContainsKey(clientPeer);
        }

        /// <summary>
        /// 用户上线
        /// </summary>
        /// <param name="clientPeer"></param>
        /// <param name="account"></param>
        public void Online(ClientPeer clientPeer, string account)
        {
            _accountClientPeerDict.Add(account,clientPeer);
            _clientPeerAccountDict.Add(clientPeer,account);
        }

        /// <summary>
        /// 下线
        /// </summary>
        /// <param name="clientPeer"></param>
        public void Offline(ClientPeer clientPeer)
        {
            string account = _clientPeerAccountDict[clientPeer];
            _clientPeerAccountDict.Remove(clientPeer);
            _accountClientPeerDict.Remove(account);
        }

        public void Offline(string account)
        {
            ClientPeer clientPeer = _accountClientPeerDict[account];
            _accountClientPeerDict.Remove(account);
            _clientPeerAccountDict.Remove(clientPeer);
        }

        /// <summary>
        /// 获取在线玩家ID
        /// </summary>
        /// <param name="clientPeer"></param>
        /// <returns></returns>
        public int GetId(ClientPeer clientPeer)
        {
            string account = _clientPeerAccountDict[clientPeer];
            return _accountModelDict[account].Id;
        }
    }
}