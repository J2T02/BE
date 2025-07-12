using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SWP.Data;

//using SWP.Data;
using SWP.Dtos.Account;
using SWP.Dtos.Booking;
using SWP.Dtos.Customer;
using SWP.Interfaces;
using SWP.Mapper;
using SWP.Models;
using SWP.Repository;
using SWP.Service.Email;
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
        private readonly IMemoryCache _memoryCache;
        private readonly EmailService _emailService;

        public AccountController(ITokenService tokenService, HIEM_MUONContext context, IAccountRepository accountRepo, IMemoryCache memoryCache, EmailService emailService)
        {
            _tokenService = tokenService;
            _context = context;
            _accountRepo = accountRepo;
            _passwordHasher = new PasswordHasher<Account>();
            _memoryCache = memoryCache;
            _emailService = emailService;
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
                    CreateAt = DateTime.Now,
                    Img = "https://cdn-icons-png.flaticon.com/512/149/149071.png" // Đặt ảnh đại diện mặc định
                };
                //test
                account.Password = _passwordHasher.HashPassword(account, registerDto.Password);
                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();

                var customer = new Customer
                {
                    AccId = account.AccId,
                    HusName = null,
                    WifeName = null,
                    HusYob = null,
                    WifeYob = null,
                };
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                var role = await _context.Roles.FindAsync(account.RoleId);

                var token = _tokenService.CreateToken(account.FullName!, account.AccId, role?.RoleName ?? "Customer");

                var newUser = new NewUserDto
                {
                    Token = null,
                    AccId = account.AccId,
                    Mail = account.Mail,
                    RoleId = account.RoleId ?? 0,
                    FullName = account.FullName!,
                    phone = account.Phone,
                    IsActive = account.IsActive ?? true,
                    CreateAt = account.CreateAt ?? DateTime.Now,
                    img = account.Img ?? string.Empty,

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
                    AccId = account.AccId,
                    Mail = account.Mail,
                    RoleId = account.RoleId ?? 0,
                    FullName = account.FullName!,
                    phone = account.Phone,
                    IsActive = account.IsActive ?? true,
                    CreateAt = account.CreateAt ?? DateTime.Now,
                    img = account.Img ?? string.Empty,
                };
                return new BaseRespone<NewUserDto>(newUser, "Đăng nhập thành công", HttpStatusCode.OK);
            }
            catch (Exception e)
            {

                return new BaseRespone<NewUserDto>(HttpStatusCode.InternalServerError, $"Lỗi hệ thống: {e.Message}");
            }

        }

        [Authorize(Roles = "Customer")]
        [HttpPut]
        public async Task<IActionResult> UpdateAccount([FromBody] AccountUpdateRequestDto updateAccountDto)
        {
            try
            {
                if (updateAccountDto == null)
                {
                    return BadRequest(new BaseRespone<string>(HttpStatusCode.BadRequest, "Dữ liệu không hợp lệ (null)."));
                }
                var accountIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (accountIdClaim == null)
                {
                    return BadRequest(new BaseRespone<string>(HttpStatusCode.BadRequest, "Không tìm thấy thông tin tài khoản."));
                }
                int accountId = int.Parse(accountIdClaim);
                var account = await _accountRepo.GetAccountAsync(accountId);
                if (account == null)
                {
                    return NotFound(new BaseRespone<string>(HttpStatusCode.NotFound, "Tài khoản không tồn tại."));
                }
                // Cập nhật thông tin tài khoản
                var updatedAccount = await _accountRepo.UpdateAccount(accountId, updateAccountDto);
                if (updatedAccount == null)
                {
                    return NotFound(new BaseRespone<string>(HttpStatusCode.NotFound, "Không tìm thấy tài khoản để cập nhật."));
                }
                var responeDto = updatedAccount.ToAccountDetailResponeDto();
                return Ok(BaseRespone<AccountDetailResponeDto>.SuccessResponse(responeDto, "Cập nhật tài khoản thành công"));

            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseRespone<string>(HttpStatusCode.InternalServerError, $"Lỗi hệ thống: {e.Message}"));
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
                    AccId = account.AccId,
                    Mail = account.Mail,
                    RoleId = account.RoleId ?? 0,
                    FullName = account.FullName!,
                    phone = account.Phone,
                    IsActive = account.IsActive ?? true,
                    CreateAt = account.CreateAt ?? DateTime.Now,
                    img = account.Img ?? string.Empty,
                };

                return new BaseRespone<NewUserDto>(newUser, "Đăng nhập thành công", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new BaseRespone<NewUserDto>(HttpStatusCode.InternalServerError, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAccountsById()
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

        [HttpGet("getAllAccounts")]
        public async Task<IActionResult> GetAllAccounts()
        {

            var accounts = await _accountRepo.GetAllAccoun();

            if (accounts == null || accounts.Count == 0)
            {
                return NotFound(new BaseRespone<List<AccountDetailResponeDto>>(HttpStatusCode.NotFound, "Không tìm thấy tài khoản nào."));
            }

            var accountResponse = accounts.ToAccountDetailResponeDto();

            return Ok(BaseRespone<List<AccountDetailResponeDto>>.SuccessResponse(
                accountResponse, "Lấy danh sách tài khoản thành công"));


        }

        [HttpPost("forgot-password/request")]
        public async Task<IActionResult> ForgotPasswordRequest([FromBody] ForgotPasswordRequestDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new BaseRespone<string>(
                        HttpStatusCode.BadRequest,
                        "Dữ liệu không hợp lệ (null)."));
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new BaseRespone<List<string>>(
                        HttpStatusCode.BadRequest,
                        "Dữ liệu không hợp lệ.",
                        errors));
                }

                // Kiểm tra email/phone
                if (string.IsNullOrEmpty(dto.EmailOrPhone))
                {
                    return BadRequest(new BaseRespone<string>(
                        HttpStatusCode.BadRequest,
                        "Email hoặc số điện thoại không được để trống."));
                }

                // Tìm tài khoản
                var account = await _accountRepo.GetAccountByEmailAsync(dto.EmailOrPhone);
                if (account == null)
                {
                    return NotFound(new BaseRespone<string>(
                        HttpStatusCode.NotFound,
                        "Tài khoản không tồn tại."));
                }

                // Tạo OTP
                var verificationCode = new Random().Next(100000, 999999).ToString();

                // Kiểm tra MemoryCache và EmailService có null không
                if (_memoryCache == null)
                    throw new NullReferenceException("IMemoryCache chưa được khởi tạo.");
                if (_emailService == null)
                    throw new NullReferenceException("EmailService chưa được khởi tạo.");

                // Lưu OTP vào bộ nhớ tạm
                _memoryCache.Set(dto.EmailOrPhone, verificationCode, TimeSpan.FromMinutes(5));

                // Gửi OTP qua email
                await _emailService.SendOtpEmailAsync(dto.EmailOrPhone, verificationCode);

                return Ok(new BaseRespone<string>(
                    data: null,
                    message: "Mã xác nhận đã được gửi đến email.",
                    statusCode: HttpStatusCode.OK
                ));
            }
            catch (Exception e)
            {
                // In log lỗi chi tiết ra Console hoặc logger nếu có
                Console.WriteLine("Lỗi ForgotPasswordRequest: " + e.ToString());

                return StatusCode(StatusCodes.Status500InternalServerError,
                    new BaseRespone<string>(
                        HttpStatusCode.InternalServerError,
                        $"Lỗi hệ thống: {e.Message}"
                    ));
            }
        }


        [HttpPost("forgot-password/reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.EmailOrPhone) || string.IsNullOrEmpty(dto.NewPassword) || string.IsNullOrEmpty(dto.Otp))
                {
                    return BadRequest(new BaseRespone<string>(HttpStatusCode.BadRequest, "Thông tin không được để trống."));
                }

                var account = await _accountRepo.GetAccountByEmailAsync(dto.EmailOrPhone);
                if (account == null)
                {
                    return NotFound(new BaseRespone<string>(HttpStatusCode.NotFound, "Tài khoản không tồn tại."));
                }

                // Lấy mã OTP đã lưu
                if (!_memoryCache.TryGetValue(dto.EmailOrPhone, out string cachedOtp))
                {
                    return BadRequest(new BaseRespone<string>(HttpStatusCode.BadRequest, "Mã xác nhận đã hết hạn hoặc không hợp lệ."));
                }

                if (cachedOtp != dto.Otp)
                {
                    return BadRequest(new BaseRespone<string>(HttpStatusCode.BadRequest, "Mã xác nhận không chính xác."));
                }

                // Cập nhật mật khẩu
                account.Password = _passwordHasher.HashPassword(account, dto.NewPassword);
                await _accountRepo.UpdatePasswordAsync(account, account.Password);

                // Xóa OTP sau khi dùng
                _memoryCache.Remove(dto.EmailOrPhone);

                return Ok(BaseRespone<string>.SuccessResponse(
                    data: null,
                    message: "Mật khẩu đã được cập nhật thành công.",
                    statusCode: HttpStatusCode.OK
                ));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new BaseRespone<string>(HttpStatusCode.InternalServerError, $"Lỗi hệ thống: {e.Message}"));
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassworđto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new BaseRespone<string>(HttpStatusCode.BadRequest, "Dữ liệu không hợp lệ (null)."));
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new BaseRespone<List<string>>(HttpStatusCode.BadRequest, "Dữ liệu không hợp lệ.", errors));
                }
                var accountIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (accountIdClaim == null)
                {
                    return BadRequest(new BaseRespone<string>(HttpStatusCode.BadRequest, "Không tìm thấy thông tin tài khoản."));
                }
                int accountId = int.Parse(accountIdClaim);
                var account = await _accountRepo.GetAccountByIdAsync(accountId);
                if (account == null)
                {
                    return NotFound(new BaseRespone<string>(HttpStatusCode.NotFound, "Tài khoản không tồn tại."));
                }
                // Kiểm tra mật khẩu cũ
                var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(account, account.Password, dto.CurrentPassword);
                if (passwordVerificationResult == PasswordVerificationResult.Failed)
                {
                    return BadRequest(new BaseRespone<string>(HttpStatusCode.BadRequest, "Mật khẩu cũ không đúng."));
                }
                // Cập nhật mật khẩu mới
                account.Password = _passwordHasher.HashPassword(account, dto.NewPassword);
                await _accountRepo.UpdatePasswordAsync(account, account.Password);
                return Ok(BaseRespone<string>.SuccessResponse(null, "Đổi mật khẩu thành công", HttpStatusCode.OK));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseRespone<string>(HttpStatusCode.InternalServerError, $"Lỗi hệ thống: {e.Message}"));
            }
        }
    }
}
