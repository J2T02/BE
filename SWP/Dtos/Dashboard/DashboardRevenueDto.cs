namespace SWP.Dtos.Dashboard
{
    public class DashboardRevenueDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalBookings { get; set; }
        public int TotalTreatmentPlans { get; set; }
        public List<RevenueChartData> RevenueChart { get; set; } = new List<RevenueChartData>();
    }

    public class RevenueChartData
    {
        public DateTime Date { get; set; }
        public string Label { get; set; }
        public decimal Revenue { get; set; }
        public int BookingCount { get; set; }
        public int TreatmentPlanCount { get; set; }
    }

    public class DashboardFilterDto
    {
        public int Period { get; set; }
        // 1 = Tuần hiện tại (Thứ 2 - Chủ nhật)
        // 2 = 1 tháng
        // 3 = 3 tháng
        // 6 = 6 tháng
        // 9 = 9 tháng
        // 12 = 12 tháng
    }
}
