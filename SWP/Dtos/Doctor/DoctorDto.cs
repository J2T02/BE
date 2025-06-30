using SWP.Dtos;
using SWP.Dtos.Account;
using SWP.Models;
namespace SWP.Dtos.Doctor
{
    public class DoctorDto
    {
        public int DocId { get; set; }

        public int? AccId { get; set; }

        public string Gender { get; set; }

        public DateOnly? Yob { get; set; }

        public int? Experience { get; set; }

        public int? Status { get; set; }

        public int? EduId { get; set; }

        public AccountDetailResponeDto AccountInfo { get; set; }
    }
}
