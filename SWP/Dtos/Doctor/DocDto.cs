using SWP.Dtos.Account;
using SWP.Models;

namespace SWP.Dtos.Doctor
{
    public class DocDto
    {


        public string Gender { get; set; }

        public DateOnly? Yob { get; set; }

        public int? Experience { get; set; }

        public string EducationLevel { get; set; }

        public AccountDetailResponeDto AccDoc { get; set; }
    }
}
