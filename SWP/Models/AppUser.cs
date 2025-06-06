using Microsoft.AspNetCore.Identity;

namespace SWP.Models
{
    public class AppUser : IdentityUser
    {
        public int Risk { get; set; } = 0;
    }
}
