using Microsoft.EntityFrameworkCore;
using SWP.Dtos.Dashboard;
using SWP.Interfaces;
using SWP.Models;

namespace SWP.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly HIEM_MUONContext _context;

        public DashboardRepository(HIEM_MUONContext context)
        {
            _context = context;
        }

        public async Task<DashboardRevenueDto> GetRevenueDashboardAsync(DashboardFilterDto filter)
        {
            var today = DateTime.Today;
            DateTime startDate, endDate;

            switch (filter.Period)
            {
                case 1: // Tuần hiện tại
                    startDate = GetStartOfWeek(today);
                    endDate = startDate.AddDays(6);
                    break;
                case 2: // Tháng hiện tại
                    startDate = new DateTime(today.Year, today.Month, 1);
                    endDate = startDate.AddMonths(1).AddDays(-1);
                    break;
                case 3: // 3 tháng: 2 tháng trước + tháng hiện tại
                    endDate = new DateTime(today.Year, today.Month, 1).AddMonths(1).AddDays(-1);
                    startDate = endDate.AddMonths(-2).AddDays(-endDate.Day + 1);
                    break;
                case 6: // 6 tháng: 5 tháng trước + tháng hiện tại
                    endDate = new DateTime(today.Year, today.Month, 1).AddMonths(1).AddDays(-1);
                    startDate = endDate.AddMonths(-5).AddDays(-endDate.Day + 1);
                    break;
                case 9: // 9 tháng: 8 tháng trước + tháng hiện tại
                    endDate = new DateTime(today.Year, today.Month, 1).AddMonths(1).AddDays(-1);
                    startDate = endDate.AddMonths(-8).AddDays(-endDate.Day + 1);
                    break;
                case 12: // 12 tháng: 11 tháng trước + tháng hiện tại
                    endDate = new DateTime(today.Year, today.Month, 1).AddMonths(1).AddDays(-1);
                    startDate = endDate.AddMonths(-11).AddDays(-endDate.Day + 1);
                    break;
                default:
                    startDate = GetStartOfWeek(today);
                    endDate = startDate.AddDays(6);
                    break;
            }

            // Tổng doanh thu
            var totalRevenue = await _context.Payments
                .Where(p => p.StatusId == 2 && p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .SumAsync(p => p.Amount ?? 0);

            // Tổng số booking
            var totalBookings = await _context.Bookings
                .Where(b => b.Status == 2 && b.CreateAt >= startDate && b.CreateAt <= endDate)
                .CountAsync();

            // Tổng số treatment plan
            var totalTreatmentPlans = await _context.TreatmentPlans
                .Where(tp => tp.Status == 1 && tp.StartDate >= startDate && tp.StartDate <= endDate)
                .CountAsync();

            // Dữ liệu biểu đồ
            var revenueChartData = await GetRevenueChartDataAsync(startDate, endDate, filter.Period);

            return new DashboardRevenueDto
            {
                TotalRevenue = totalRevenue,
                TotalBookings = totalBookings,
                TotalTreatmentPlans = totalTreatmentPlans,
                RevenueChart = revenueChartData
            };
        }

        private DateTime GetStartOfWeek(DateTime date)
        {
            var daysSinceMonday = ((int)date.DayOfWeek + 6) % 7;
            return date.AddDays(-daysSinceMonday);
        }

        private async Task<List<RevenueChartData>> GetRevenueChartDataAsync(DateTime startDate, DateTime endDate, int period)
        {
            var result = new List<RevenueChartData>();

            if (period == 1) // Tuần hiện tại - từng ngày
            {
                var dailyData = await _context.Payments
                    .Where(p => p.StatusId == 2 && p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                    .GroupBy(p => p.PaymentDate.Value.Date)
                    .Select(g => new { Date = g.Key, Revenue = g.Sum(p => p.Amount ?? 0) })
                    .OrderBy(x => x.Date)
                    .ToListAsync();

                var dailyBookings = await _context.Bookings
                    .Where(b => b.Status == 2 && b.CreateAt >= startDate && b.CreateAt <= endDate)
                    .GroupBy(b => b.CreateAt.Value.Date)
                    .Select(g => new { Date = g.Key, BookingCount = g.Count() })
                    .ToListAsync();

                var dailyTreatmentPlans = await _context.TreatmentPlans
                    .Where(tp => tp.Status == 1 && tp.StartDate >= startDate && tp.StartDate <= endDate)
                    .GroupBy(tp => tp.StartDate.Value.Date)
                    .Select(g => new { Date = g.Key, TreatmentPlanCount = g.Count() })
                    .ToListAsync();

                for (int i = 0; i < 7; i++)
                {
                    var currentDate = startDate.AddDays(i);
                    var dayData = dailyData.FirstOrDefault(d => d.Date == currentDate);
                    var bookingData = dailyBookings.FirstOrDefault(b => b.Date == currentDate);
                    var treatmentPlanData = dailyTreatmentPlans.FirstOrDefault(tp => tp.Date == currentDate);

                    var dayName = currentDate.DayOfWeek switch
                    {
                        DayOfWeek.Monday => "Thứ 2",
                        DayOfWeek.Tuesday => "Thứ 3",
                        DayOfWeek.Wednesday => "Thứ 4",
                        DayOfWeek.Thursday => "Thứ 5",
                        DayOfWeek.Friday => "Thứ 6",
                        DayOfWeek.Saturday => "Thứ 7",
                        DayOfWeek.Sunday => "Chủ nhật",
                        _ => currentDate.ToString("dd/MM")
                    };

                    result.Add(new RevenueChartData
                    {
                        Date = currentDate,
                        Label = $"{dayName} ({currentDate:dd/MM})",
                        Revenue = dayData?.Revenue ?? 0,
                        BookingCount = bookingData?.BookingCount ?? 0,
                        TreatmentPlanCount = treatmentPlanData?.TreatmentPlanCount ?? 0
                    });
                }
            }
            else // period = 2, 3, 6, 9, 12: chỉ trả về 1 record tổng
            {
                var totalRevenue = await _context.Payments
                    .Where(p => p.StatusId == 2 && p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                    .SumAsync(p => p.Amount ?? 0);

                var totalBookings = await _context.Bookings
                    .Where(b => b.Status == 2 && b.CreateAt >= startDate && b.CreateAt <= endDate)
                    .CountAsync();

                var totalTreatmentPlans = await _context.TreatmentPlans
                    .Where(tp => tp.Status == 1 && tp.StartDate >= startDate && tp.StartDate <= endDate)
                    .CountAsync();

                string label = period switch
                {
                    2 => $"{startDate:MM/yyyy}",
                    3 => $"3 Tháng ({startDate:MM/yyyy} - {endDate:MM/yyyy})",
                    6 => $"6 Tháng ({startDate:MM/yyyy} - {endDate:MM/yyyy})",
                    9 => $"9 Tháng ({startDate:MM/yyyy} - {endDate:MM/yyyy})",
                    12 => $"12 Tháng ({startDate:MM/yyyy} - {endDate:MM/yyyy})",
                    _ => $"{startDate:MM/yyyy} - {endDate:MM/yyyy}"
                };

                result.Add(new RevenueChartData
                {
                    Date = startDate,
                    Label = label,
                    Revenue = totalRevenue,
                    BookingCount = totalBookings,
                    TreatmentPlanCount = totalTreatmentPlans
                });
            }

            return result;
        }
    }
}
