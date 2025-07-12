using SWP.Dtos.Account;
using SWP.Models;

namespace SWP.Dtos.Doctor
{
    public class DocDto
    {

        public string DocId { get; set; }
        //public string? AccId { get; set; }
        public AccountDetailResponeDto AccDoc { get; set; }
    }
}
