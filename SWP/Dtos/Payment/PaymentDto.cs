namespace SWP.Dtos.Payment
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }

        public int? PaymentTypeId { get; set; }

        public int? BookingId { get; set; }

        public int? TreatmentPlansId { get; set; }

        public decimal? Amount { get; set; }

        public DateTime? PaymentDate { get; set; }

        public int? MethodId { get; set; }

        public int? StatusId { get; set; }

        public string TransactionId { get; set; }
    }
}
