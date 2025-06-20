using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SWP.Data;
using SWP.Dtos.Customer;
using SWP.Interfaces;
using SWP.Mapper;
using SWP.Models;
using System.Net;
using System.Security.Claims;

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

            //var customerDto = customers.Select(c => c.ToCustomerDto());
            var customerDtos = customers.Select(x => x.ToCustomerDto()).ToList();
            return Ok( BaseRespone<List<CustomerDto>>.SuccessResponse(customerDtos,"Lấy dữ liệu thành công",HttpStatusCode.OK));
        }

        [HttpGet("id")]
        public async Task<BaseRespone<CustomerDto>> GetCustomerDetail()
        {
            try
            {
                var accountIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (accountIdClaim == null)
                {
                    return new BaseRespone<CustomerDto>(HttpStatusCode.NotFound, "Không tìm thấy khách hàng với tài khoản này");
                }

                int accountId = int.Parse(accountIdClaim);

                var customerid = await _context.Customers.FirstOrDefaultAsync(c => c.AccId == accountId);
                if (customerid == null)
                {
                   return new BaseRespone<CustomerDto>(HttpStatusCode.NotFound, "Không tìm thấy khách hàng với tài khoản này");
                }

                int Id = customerid.CusId;
                var customer = await _context.Customers
           .Include(c => c.Acc).FirstOrDefaultAsync(c => c.CusId == Id);

                //var customer = await _context.Customers.FindAsync(id);   //

                if (customer == null)
                {
                    return new BaseRespone<CustomerDto>(HttpStatusCode.NotFound, "Không tìm thấy khách hàng");
                }

                var customerDto = new CustomerDto
                {
                    
                    HusName = customer.HusName,
                    HusYob = customer.HusYob,
                    WifeName = customer.WifeName,
                    WifeYob = customer.WifeYob,
                   
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
