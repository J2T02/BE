using SWP.Models;

namespace SWP.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetAccountAsync(int accId);
        Task<List<Account>> GetAllAccoun();
        Task<Account?> GetAccountByEmailAsync(string emailOrPhone);
        Task UpdatePasswordAsync(Account account, string hashedNewPassword);
    }
}
