using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using SWP.Data;
using SWP.Dtos.Account;
using SWP.Interfaces;
using SWP.Models;

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
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (await _context.Accounts.AnyAsync(a => a.AccName == registerDto.AccName))
                {
                    return BadRequest("Username is already taken.");
                }

                if (await _context.Customers.AnyAsync(c => c.Mail == registerDto.Mail))
                {
                    return BadRequest("Mail is already registered.");
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

                return Ok(new NewUserDto
                {
                    Token = token,
                    UserName = account.AccName!,
                    Role = role?.RoleName ?? "Customer",
                });

            }
            catch (Exception e)
            {
                return StatusCode(500, e);

            }
        }

        
    }
}
