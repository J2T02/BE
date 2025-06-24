using SWP.Models;

namespace SWP.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetAccountAsync(int accId);
    }
}
