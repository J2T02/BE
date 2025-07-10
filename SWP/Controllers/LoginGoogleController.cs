using System.Net;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SWP.Data;
using SWP.Dtos.Account;
using SWP.Interfaces;
using SWP.Models;

namespace SWP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginGoogleController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly HIEM_MUONContext _context;

        private readonly ITokenService _tokenService;
        public LoginGoogleController(IConfiguration config, HIEM_MUONContext context, ITokenService tokenService)
        {
            _config = config;
            _context = context;
            _tokenService = tokenService;
        }

        [HttpGet("login-url")]
        public IActionResult GetLoginUrl()
        {
            var clientId = _config["Google:ClientId"];
            var redirectUri = _config["Google:RedirectUri"];

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["client_id"] = clientId;
            query["redirect_uri"] = redirectUri;
            query["response_type"] = "code";
            query["scope"] = "openid email profile";
            query["access_type"] = "offline";
            query["prompt"] = "consent";

            var url = $"https://accounts.google.com/o/oauth2/v2/auth?{query}";

            return Ok(new { url });
        }

        [HttpGet("google-callback")]
        public async Task<BaseRespone<NewUserDto>> GoogleCallback([FromQuery] string code)
        {
            try
            {
                var clientId = _config["Google:ClientId"];
                var clientSecret = _config["Google:ClientSecret"];
                var redirectUri = _config["Google:RedirectUri"];

                using var client = new HttpClient();
                var content = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("redirect_uri", redirectUri),
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
        });

                var response = await client.PostAsync("https://oauth2.googleapis.com/token", content);
                var body = await response.Content.ReadAsStringAsync();
                var tokenObj = JObject.Parse(body);

                var idToken = tokenObj["id_token"]?.ToString();
                if (idToken == null)
                    return new BaseRespone<NewUserDto>(HttpStatusCode.BadRequest, "Không nhận được id_token từ Google");

                var payload = await Google.Apis.Auth.GoogleJsonWebSignature.ValidateAsync(idToken);
                if (payload == null || !payload.EmailVerified)
                    return new BaseRespone<NewUserDto>(HttpStatusCode.BadRequest, "Google xác thực thất bại");

                var context = HttpContext.RequestServices.GetRequiredService<HIEM_MUONContext>();
                var tokenService = HttpContext.RequestServices.GetRequiredService<ITokenService>();

                // Tìm account theo email
                var account = await context.Accounts.Include(a => a.Role).FirstOrDefaultAsync(a => a.Mail == payload.Email);

                if (account == null)
                {
                    // Mặc định RoleId = 3 (Customer)
                    account = new Account
                    {
                        Mail = payload.Email,
                        FullName = payload.Name,
                        Img = payload.Picture,
                        IsActive = true,
                        CreateAt = DateTime.Now,
                        Password = "GOOGLE",
                        RoleId = 4, // Giả sử RoleId 4 là Customer
                    };

                    context.Accounts.Add(account);
                    await context.SaveChangesAsync();
                }

                // Đảm bảo có RoleId sau khi tạo mới
                var role = await context.Roles.FindAsync(account.RoleId);
                if (role == null)
                    return new BaseRespone<NewUserDto>(HttpStatusCode.BadRequest, "Không tìm thấy vai trò người dùng");

                var jwt = tokenService.CreateToken(account.FullName!, account.AccId, role.RoleName);

                var newUser = new NewUserDto
                {
                    Token = jwt,
                    AccId = account.AccId,
                    Mail = account.Mail,
                    RoleId = account.RoleId ?? 0,
                    FullName = account.FullName!,
                    phone = account.Phone,
                    IsActive = account.IsActive ?? true,
                    CreateAt = account.CreateAt ?? DateTime.Now,
                    img = account.Img ?? string.Empty,
                };

                return new BaseRespone<NewUserDto>(newUser, "Đăng nhập Google thành công", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new BaseRespone<NewUserDto>(HttpStatusCode.InternalServerError, $"Lỗi hệ thống: {ex.Message}");
            }
        }

    }
}
