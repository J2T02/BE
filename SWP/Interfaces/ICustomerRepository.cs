using SWP.Dtos.Customer;
using SWP.Models;

namespace SWP.Interfaces
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllCustomersAsync();
        Task<Customer?> GetCustomerByIdAsync(int id);
        
        Task<UpdateCustomerResponseDto> CreateCustomerAsync(UpdateCustomerRequestDto request, int accountId);
    }
}
