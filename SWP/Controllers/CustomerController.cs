using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP.Interfaces;
using SWP.Mapper;
using SWP.Models;

namespace SWP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly HIEM_MUONContext _context;
        public CustomerController(ICustomerRepository customerRepository, HIEM_MUONContext context)
        {
            _customerRepository = customerRepository;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var customers = await _customerRepository.GetAllCustomersAsync();
            var customerDto = customers.Select(c => c.ToCustomerDto());
             return Ok(customers);
        }
    }
}
