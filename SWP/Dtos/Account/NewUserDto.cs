namespace SWP.Dtos.Account
{
    public class NewUserDto
    {
        public string Token { get; set; }

        public int AccId { get; set; }
        public string Mail { get; set; }

        public int RoleId { get; set; }

        public string FullName { get; set; }

        public string phone { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreateAt { get; set; }

        public string img { get; set; }

        
    }
}
