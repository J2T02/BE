using SWP.Dtos.Account;
using SWP.Models;

namespace SWP.Mapper
{
    public static class AccountDetailMapper
    {
        public static AccountDetailResponeDto ToAccountDetailResponeDto(this Account account)
        {
            return new AccountDetailResponeDto
            {
                Mail = account.Mail,
                FullName = account.FullName,
                Phone = account.Phone
            };
        }

        

        public static List<AccountDetailResponeDto> ToAccountDetailResponeDto(this List<Account> accounts)
        {
            return accounts.Select(a => a.ToAccountDetailResponeDto()).ToList();
        }
    }
}
