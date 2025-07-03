using SWP.Dtos;
using SWP.Dtos.Account;
using SWP.Models;
namespace SWP.Dtos.Doctor
{
    public class DoctorDto
    {
        public int DocId { get; set; }

        public string Gender { get; set; }

        public DateOnly? Yob { get; set; }

        public int? Experience { get; set; }

        public StatusInfoDto? Status { get; set; }

        public EduInfoDto? EduInfo { get; set; }

        public List<CertificateDto?>? CertificateInfo { get; set; }

        public DateTime? CreateAt { get; set; }

        public string? Img { get; set; }


        public AccountDetailResponeDto AccountInfo { get; set; }
    }
}
