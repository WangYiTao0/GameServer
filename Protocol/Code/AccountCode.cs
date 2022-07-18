namespace Protocol.Code
{
    public class AccountCode
    {
        //注册操作码
        /// <summary>
        /// Client Request
        /// </summary>
        public const int REGISTER_CLIENT_REQUEST = 0; // Client Request //参数 Account Dto
        /// <summary>
        /// Server response
        /// </summary>
        public const int REGISTER_SERVER_RESPONSE = 1; // Server response;

        /// <summary>
        /// 登录操作码
        /// </summary>
        public const int LOGIN = 2;//账号密码//参数 Account Dto
    }
}

