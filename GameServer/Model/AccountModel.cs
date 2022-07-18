
namespace GameServer.Model;

/// <summary>
/// 账号模型
/// </summary>
public class AccountModel
{
    public int Id;
    public string Account;
    public string Password;
    
    //创建日期 电话号码

    public AccountModel(int id ,string account, string password)
    {
        Id = id;
        Account = account;
        Password = password;
    }
}