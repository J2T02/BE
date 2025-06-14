using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Data;

//using SWP.Data;
using SWP.Dtos.Account;
using SWP.Interfaces;
using SWP.Models;
using System.Data;
using System.Net;

namespace SWP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly HIEM_MUONContext _context;
        private readonly PasswordHasher<Account> _passwordHasher;

        public AccountController(ITokenService tokenService, HIEM_MUONContext context)
        {
            _tokenService = tokenService;
            _context = context;
            _passwordHasher = new PasswordHasher<Account>();
        }

        [HttpPost("register")]
        public async Task<BaseRespone<NewUserDto>> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                // Kiểm tra validation từ DataAnnotations
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

                    var firstError = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .FirstOrDefault()?.ErrorMessage ?? "Dữ liệu không hợp lệ";

                    return new BaseRespone<NewUserDto>(HttpStatusCode.BadRequest, firstError);
                }

                if (await _context.Accounts.AnyAsync(a => a.AccName == registerDto.AccName))
                {
                    return new BaseRespone<NewUserDto>(HttpStatusCode.BadRequest, "Tên tài khoản đã tồn tại vui lòng thử lại.");
                }

                if (await _context.Customers.AnyAsync(c => c.Mail == registerDto.Mail))
                {
                    return new BaseRespone<NewUserDto>(HttpStatusCode.BadRequest, "Số điện thoại hoặc Email đã tồn tại vui lòng thử lại");
                }

                if (await _context.Customers.AnyAsync(c => c.Phone == registerDto.Phone))
                {
                    return new BaseRespone<NewUserDto>(HttpStatusCode.BadRequest, "Số điện thoại hoặc Email đã tồn tại vui lòng thử lại");
                }



                var account = new Account
                {

                    AccName = registerDto.AccName,
                    RoleId = 3
                };
                account.Password = _passwordHasher.HashPassword(account, registerDto.Password);
                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();

                var customer = new Customer
                {
                    AccId = account.AccId,

                    Phone = registerDto.Phone,
                    Mail = registerDto.Mail
                };
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                var role = await _context.Roles.FindAsync(account.RoleId);

                var token = _tokenService.CreateToken(account.AccName!, account.AccId, role?.RoleName ?? "Customer");
                
                var newUser = new NewUserDto
                {
                    Token = token,
                    UserName = account.AccName!,
                    Role = role?.RoleName ?? "Customer",
                };

                return new BaseRespone<NewUserDto>(newUser, "Đăng ký thành công", HttpStatusCode.OK);


            }
            catch (Exception e)
            {
                return new BaseRespone<NewUserDto>(HttpStatusCode.InternalServerError, $"Lỗi hệ thống: {e.Message}");

            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var account = await _context.Accounts.Include(a => a.Role).FirstOrDefaultAsync(a => a.AccName == loginDto.UserName );
                if (account == null)
                    return BadRequest("Tên tài khoản hoặc mật khẩu không đúng");

                //Xác thực mật khẩu:
                var passwordVerifi = _passwordHasher.VerifyHashedPassword(account, account.Password, loginDto.Password);
                if (passwordVerifi == PasswordVerificationResult.Failed)
                    return BadRequest("Tên tài khoản hoặc mật khẩu không đúng");

                var role = await _context.Roles.FindAsync(account.RoleId);
                var token = _tokenService.CreateToken(account.AccName!, account.AccId, role?.RoleName ?? "Customer");
                return Ok(new NewUserDto
                {
                    Token = token,
                    UserName = account.AccName!,
                    Role = account.Role?.RoleName ?? "Customer",
                });
            }
            catch (Exception e)
            {

                return StatusCode(500, e);
            }

        }


    }
}
