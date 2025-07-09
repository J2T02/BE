using SWP.Dtos.Account;

namespace SWP.Dtos.Customer
{
    public class CustomerDto
    {
        public int CusId { get; set; }
        //public int? AccId { get; set; }
        public string? HusName { get; set; }

        public DateOnly? HusYob { get; set; }

        public string? WifeName { get; set; }

        public DateOnly? WifeYob { get; set; }
         public AccountDetailResponeDto AccCus { get; set; }


    }
}
