using SWP.Dtos.Account;

namespace SWP.Dtos.Doctor
{
    public class DoctorAccountDto
    {
        public int DocId { get; set; }
        public StatusInfoDto DoctorStatus { get; set; }
        public AccountDetailResponeDto AccountInfo { get; set; }
    }
}
