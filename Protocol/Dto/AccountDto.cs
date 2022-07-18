namespace Protocol.Dto
{
    [Serializable]
    public class AccountDto
    {
        public string Account;
        public string Password;
        
        public AccountDto()
        {
            
        }
        public AccountDto(string account,string password)
        {
            Account = account;
            Password = password;
        }
    }
}