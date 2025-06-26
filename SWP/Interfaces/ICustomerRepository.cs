using SWP.Dtos.Customer;
using SWP.Models;

namespace SWP.Interfaces
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllCustomersAsync();
        Task<Customer?> GetCustomerByCusIdAsync(int id);
        
        Task<UpdateCustomerResponseDto> CreateCustomerAsync(UpdateCustomerRequestDto request, int accountId);
    }
}
