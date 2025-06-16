using SWP.Models;

namespace SWP.Dtos.Doctor
{
    public class DoctorDto
    {
        public int DocId { get; set; }


        public int? AccId { get; set; }
        public string DocName { get; set; }

        public string Gender { get; set; }

        public DateOnly? Yob { get; set; }

        public string Mail { get; set; }

        public string Phone { get; set; }

        public int? Experience { get; set; }

        public string Certification { get; set; }
    }
}
