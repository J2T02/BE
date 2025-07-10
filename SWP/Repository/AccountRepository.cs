using Microsoft.EntityFrameworkCore;
using SWP.Interfaces;
using SWP.Models;

namespace SWP.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly HIEM_MUONContext _context;

        
        public AccountRepository(HIEM_MUONContext context)
        {
            _context = context;
        }
        public async Task<Account?> GetAccountAsync(int accId)
        {
            // Assuming you want to return the first account or a specific one
            return await _context.Accounts.FirstOrDefaultAsync(a => a.AccId == accId);
        }

        public Task<Account?> GetAccountByEmailAsync(string emailOrPhone)
        {
            return _context.Accounts
                .FirstOrDefaultAsync(a => a.Mail == emailOrPhone || a.Phone == emailOrPhone);
        }

        public async Task<Account?> GetAccountByIdAsync(int accId)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.AccId == accId);
        }

        public Task<Account?> GetAccountByMailOrPhone(string mailOrPhone)
        {
            return _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.Mail == mailOrPhone || a.Phone == mailOrPhone);
        }

        public async Task<List<Account?>> GetAllAccoun()
        {
            return await _context.Accounts.Where(a => a.RoleId == 4).ToListAsync();
        }

        public async Task UpdatePasswordAsync(Account account, string hashedNewPassword)
        {
            account.Password = hashedNewPassword;
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }
    }
    
}
