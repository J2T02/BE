namespace SWP.Dtos.Account
{
    public class ResetPasswordDto
    {
        public string EmailOrPhone { get; set; }
        public string NewPassword { get; set; }
    }
}
