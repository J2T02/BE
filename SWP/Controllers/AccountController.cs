using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Data;

//using SWP.Data;
using SWP.Dtos.Account;
using SWP.Dtos.Booking;
using SWP.Dtos.Customer;
using SWP.Interfaces;
using SWP.Mapper;
using SWP.Models;
using SWP.Repository;
using System.Data;
using System.Net;
using System.Security.Claims;

namespace SWP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly HIEM_MUONContext _context;
        private readonly IAccountRepository _accountRepo;
        private readonly PasswordHasher<Account> _passwordHasher;

        public AccountController(ITokenService tokenService, HIEM_MUONContext context, IAccountRepository accountRepo)
        {
            _tokenService = tokenService;
            _context = context;
            _accountRepo = accountRepo;
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

                if (await _context.Accounts.AnyAsync(a => a.Mail == registerDto.Mail))
                {
                    return new BaseRespone<NewUserDto>(HttpStatusCode.BadRequest, "Tên tài khoản đã tồn tại vui lòng thử lại.");
                }



                if (await _context.Accounts.AnyAsync(c => c.Phone == registerDto.Phone))
                {
                    return new BaseRespone<NewUserDto>(HttpStatusCode.BadRequest, "Số điện thoại hoặc Email đã tồn tại vui lòng thử lại");
                }





                var account = new Account
                {
                    Mail = registerDto.Mail,
                    Password = registerDto.Password,
                    FullName = registerDto.FullName,
                    Phone = registerDto.Phone,
                    RoleId = 4, // RoleId 4 là cho Customer
                    IsActive = true,
                };
                account.Password = _passwordHasher.HashPassword(account, registerDto.Password);
                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();



                var role = await _context.Roles.FindAsync(account.RoleId);

                var token = _tokenService.CreateToken(account.FullName!, account.AccId, role?.RoleName ?? "Customer");

                var newUser = new NewUserDto
                {
                    Token = null,
                    UserName = account.FullName!,
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
        public async Task<BaseRespone<NewUserDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var account = await _context.Accounts.Include(a => a.Role).FirstOrDefaultAsync(a => (a.Mail == loginDto.MailOrPhone || a.Phone == loginDto.MailOrPhone) && a.IsActive == true);
                if (account == null)
                    return new BaseRespone<NewUserDto>(HttpStatusCode.BadRequest, "Tên tài khoản hoặc mật khẩu không đúng");

                if (account.IsActive == false)
                    return new BaseRespone<NewUserDto>(HttpStatusCode.BadRequest, "Tài khoản đã bị khóa. Vui lòng liên hệ quản trị viên để biết thêm chi tiết.");

                //Xác thực mật khẩu:
                var passwordVerifi = _passwordHasher.VerifyHashedPassword(account, account.Password, loginDto.Password);
                if (passwordVerifi == PasswordVerificationResult.Failed)
                    return new BaseRespone<NewUserDto>(HttpStatusCode.BadRequest, "Tên tài khoản hoặc mật khẩu không đúng");

                var role = await _context.Roles.FindAsync(account.RoleId);

                var token = _tokenService.CreateToken(account.FullName!, account.AccId, role?.RoleName ?? "Customer");

                // Truy vấn Customer theo AccountId
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.AccId == account.AccId);

                var newUser = new NewUserDto
                {
                    Token = token,
                    UserName = account.FullName!,
                    Role = role?.RoleName ?? "Customer",
                    UserId = customer?.CusId ?? 0 // Lấy CusId từ Customer nếu có, nếu không thì 0
                };
                return new BaseRespone<NewUserDto>(newUser, "Đăng nhập thành công", HttpStatusCode.OK);
            }
            catch (Exception e)
            {

                return new BaseRespone<NewUserDto>(HttpStatusCode.InternalServerError, $"Lỗi hệ thống: {e.Message}");
            }

        }

        [HttpPost("loginDoctor")]
        public async Task<BaseRespone<NewUserDto>> LoginDoctor([FromBody] LoginDto loginDto)
        {
            try
            {
                // 1. Kiểm tra tài khoản
                var account = await _context.Accounts
                    .Include(a => a.Role)
                    .FirstOrDefaultAsync(a => a.Mail == loginDto.MailOrPhone || a.Phone == loginDto.MailOrPhone);

                if (account == null || account.Role?.RoleName != "Doctor")
                    return new BaseRespone<NewUserDto>(HttpStatusCode.BadRequest, "Tên tài khoản hoặc mật khẩu không đúng");

                // 2. Kiểm tra mật khẩu
                var passwordVerified = _passwordHasher.VerifyHashedPassword(account, account.Password, loginDto.Password);
                if (passwordVerified == PasswordVerificationResult.Failed)
                    return new BaseRespone<NewUserDto>(HttpStatusCode.BadRequest, "Tên tài khoản hoặc mật khẩu không đúng");

                // 3. Lấy role (phòng trường hợp Role null)
                var role = await _context.Roles.FindAsync(account.RoleId);
                var roleName = role?.RoleName ?? "Doctor";

                // 4. Tạo token
                var token = _tokenService.CreateToken(account.FullName!, account.AccId, roleName);

                var doctor = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.AccId == account.AccId);

                // 5. Trả về DTO
                var newUser = new NewUserDto
                {
                    Token = token,
                    UserName = account.FullName!,
                    Role = roleName,
                    UserId = doctor?.DocId ?? 0 // Lấy DocId từ Doctor nếu có, nếu không thì 0
                };

                return new BaseRespone<NewUserDto>(newUser, "Đăng nhập thành công", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new BaseRespone<NewUserDto>(HttpStatusCode.InternalServerError, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAccounts()
        {
            try
            {
                var accountIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (accountIdClaim == null)
                {
                    return BadRequest(new BaseRespone<List<Account>>(HttpStatusCode.BadRequest, "Không tìm thấy thông tin tài khoản."));
                }

                int accountId = int.Parse(accountIdClaim);

                var account = await _accountRepo.GetAccountAsync(accountId);

                if (account == null)
                {
                    return NotFound(new BaseRespone<List<Account>>(HttpStatusCode.NotFound, "Không tìm thấy tài khoản nào."));
                }
                var responeDto = account.ToAccountDetailResponeDto();
                return Ok(BaseRespone<AccountDetailResponeDto>.SuccessResponse(responeDto, "Đặt lịch thành công"));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseRespone<string>(HttpStatusCode.InternalServerError, $"Lỗi hệ thống: {e.Message}"));
            }


        }
    }
}
