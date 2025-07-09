using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SWP.Data;
using SWP.Dtos.Account;
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
        private readonly IAccountRepository _accountRepo;
        public CustomerController(ICustomerRepository customerRepository, HIEM_MUONContext context, IAccountRepository accountRepository)
        {
            _customerRepository = customerRepository;
            _context = context;
            _accountRepo = accountRepository;
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
            return Ok(BaseRespone<List<CustomerDto>>.SuccessResponse(customerDtos, "Lấy dữ liệu thành công", HttpStatusCode.OK));
        }

        [HttpGet("{id}")]
        public async Task<BaseRespone<CustomerDto>> GetCustomerDetail(int id)
        {
            try
            {
                var customer = await _customerRepository.GetCustomerByCusIdAsync(id);

                if (customer == null)
                {
                    return new BaseRespone<CustomerDto>(
                        HttpStatusCode.NotFound,
                        "Không tìm thấy khách hàng với ID này"
                    );
                }

                var customerDto = new CustomerDto
                {
                    HusName = customer.HusName,
                    HusYob = customer.HusYob,
                    WifeName = customer.WifeName,
                    WifeYob = customer.WifeYob,
                    AccCus = new Dtos.Account.AccountDetailResponeDto
                    {
                        FullName = customer.Acc.FullName,
                        Mail = customer.Acc.Mail,
                        Phone = customer.Acc.Phone,
                    }
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

        [HttpPut("update")]
        public async Task<ActionResult> UpdateCustomer([FromBody] UpdateCustomerRequestDto updateDto, [FromQuery] int AccountId)
        {


            try
            {
                var result = await _customerRepository.CreateCustomerAsync(updateDto, AccountId);
                return Ok(BaseRespone<UpdateCustomerResponseDto>.SuccessResponse(result, "Cập nhật thông tin khách hàng thành công", HttpStatusCode.OK));
            }
            catch (ArgumentException ex)
            {
                return NotFound(BaseRespone<UpdateCustomerResponseDto>.ErrorResponse(ex.Message, System.Net.HttpStatusCode.NotFound));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    BaseRespone<UpdateCustomerResponseDto>.ErrorResponse(
                        "Cập nhật thông tin khách hàng không thành công do lỗi hệ thống: " + ex.Message,
                        System.Net.HttpStatusCode.InternalServerError));
            }
        }
        [HttpGet("FindCustomer/{txtSearch}")]
        public async Task<IActionResult> FindCustomer(string txtSearch)
        {
            var accountSearch = await _accountRepo.GetAccountByMailOrPhone(txtSearch);
            if (accountSearch == null)
            {
                return NotFound(BaseRespone<string>.ErrorResponse("Không tìm thấy tài khoản khách hàng", HttpStatusCode.NotFound));
            }
            var customerSearch = await _customerRepository.GetCustomerByAccountId(accountSearch.AccId);
            if (customerSearch == null)
            {
                return NotFound(BaseRespone<string>.ErrorResponse("Không tìm thấy khách hàng với tài khoản này", HttpStatusCode.NotFound));
            }
            var resultDto = customerSearch.ToCustomerDto();
            return Ok(BaseRespone<CustomerDto>.SuccessResponse(resultDto, "Tìm kiếm thông tin khách hàng thành công"));
        }

    }
}
