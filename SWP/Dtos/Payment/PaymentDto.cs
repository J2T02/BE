namespace SWP.Dtos.Payment
{
    public class PaymentDto
    {
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Method { get; set; }
        public string Status { get; set; }
    }
}
