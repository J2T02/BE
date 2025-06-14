using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP.Data;
using SWP.Dtos.Customer;
using SWP.Interfaces;
using SWP.Mapper;
using SWP.Models;
using System.Net;

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

        [HttpGet("{id}")]
        public async Task<BaseRespone<CustomerDto>> GetCustomerDetail(int id)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(id);

                if (customer == null)
                {
                    return new BaseRespone<CustomerDto>(HttpStatusCode.NotFound, "Không tìm thấy khách hàng");
                }

                var customerDto = new CustomerDto
                {
                    CusId = customer.CusId,
                    AccId = customer.AccId,
                    HusName = customer.HusName,
                    HusYob = customer.HusYob,
                    WifeName = customer.WifeName,
                    WifeYob = customer.WifeYob,
                    Phone = customer.Phone,
                    Mail = customer.Mail
                };

                return new BaseRespone<CustomerDto>(
                    data: customerDto,
                    message: "Lấy dữ liệu thành công",
                    statusCode: HttpStatusCode.OK
                );
            }
            catch (Exception ex)
            {
                return new BaseRespone<CustomerDto>(
                    statusCode: HttpStatusCode.InternalServerError,
                    message: "Lỗi: " + ex.Message
                );
            }
        }


    }
}
