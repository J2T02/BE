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

        public async Task<List<Account?>> GetAllAccoun()
        {
            return await _context.Accounts.Where(a => a.RoleId == 4).ToListAsync();
                ;
        }
    }
    
}
