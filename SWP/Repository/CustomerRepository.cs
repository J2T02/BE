using Microsoft.EntityFrameworkCore;
using SWP.Dtos.Customer;
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

        public async Task<UpdateCustomerResponseDto> CreateCustomerAsync(UpdateCustomerRequestDto request, int accountId)
        {
            // 1. Tìm customer theo accountId
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.AccId == accountId);
            if (customer == null)
                throw new ArgumentException("Customer không tồn tại cho tài khoản này.");

            // 2. Cập nhật thông tin
            customer.HusName = request.HusName.Trim();
            customer.WifeName = request.WifeName.Trim();
            customer.HusYob = request.HusYob;
            customer.WifeYob = request.WifeYob;

            // 3. Lưu thay đổi
            await _context.SaveChangesAsync();

            // 4. Trả về DTO response
            return new UpdateCustomerResponseDto
            {
                
                HusName = customer.HusName,
                WifeName = customer.WifeName,
                HusYob = customer.HusYob ?? default,
                WifeYob = customer.WifeYob ?? default
            };
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
           return await _context.Customers.Include(c => c.Acc).ToListAsync();
        }

        public async Task<Customer?> GetCustomerByAccountId(int accountId)
        {
            return await _context.Customers.Include(c => c.Acc)
                .FirstOrDefaultAsync(c => c.AccId == accountId);
        }

        public async Task<Customer?> GetCustomerByCusIdAsync(int id)
        {
            // Ưu tiên tìm theo CusId trước
            var customer = await _context.Customers
                .Include(c => c.Acc)
                .FirstOrDefaultAsync(c => c.CusId == id);

            if (customer == null)
            {
                customer = await _context.Customers
                    .Include(c => c.Acc)
                    .FirstOrDefaultAsync(c => c.AccId == id);
            }

            return customer;
        }


    }
    
    
}
