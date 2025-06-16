using Microsoft.EntityFrameworkCore;
using SWP.Interfaces;
using SWP.Models;

namespace SWP.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly HIEM_MUONContext _context;
        public CustomerRepository(HIEM_MUONContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
           return await _context.Customers.Include(c => c.Acc).ToListAsync();
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.CusId == id);
        }


    }
    
    
}
