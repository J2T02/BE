namespace SWP.Dtos.Customer
{
    public class CustomerDto
    {
        //public int CusId { get; set; }
        public int? AccId { get; set; }

        public string AccName { get; set; }

        public string HusName { get; set; }

        public DateOnly? HusYob { get; set; }

        public string WifeName { get; set; }

        public DateOnly? WifeYob { get; set; }

        public string Phone { get; set; }

        public string Mail { get; set; }

    }
}
