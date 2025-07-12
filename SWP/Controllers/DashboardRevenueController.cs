using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP.Data;
using SWP.Dtos.Dashboard;
using SWP.Interfaces;
using SWP.Repository;
using System.Net;

namespace SWP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardRevenueController : ControllerBase
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardRevenueController(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        [HttpGet("revenue")]
        public async Task<BaseRespone<DashboardRevenueDto>> GetRevenueDashboard([FromQuery] int period)
        {
            try
            {
                if (period != 1 && period != 2 && period != 3 && period != 6 && period != 9 && period != 12)
                {
                    return BaseRespone<DashboardRevenueDto>.ErrorResponse(
                        "Period must be 1 (1 week), 2 (1 month), 3, 6, 9, or 12 months",
                        null,
                        HttpStatusCode.BadRequest);
                }

                var filter = new DashboardFilterDto { Period = period };
                var result = await _dashboardRepository.GetRevenueDashboardAsync(filter);

                var message = period switch
                {
                    1 => "Lấy dữ liệu tuần hiện tại thành công",
                    2 => "Lấy dữ liệu tháng hiện tại thành công",
                    3 => "Lấy dữ liệu 3 tháng (2 tháng trước + tháng hiện tại) thành công",
                    6 => "Lấy dữ liệu 6 tháng (5 tháng trước + tháng hiện tại) thành công",
                    9 => "Lấy dữ liệu 9 tháng (8 tháng trước + tháng hiện tại) thành công",
                    12 => "Lấy dữ liệu 12 tháng (11 tháng trước + tháng hiện tại) thành công",
                    _ => "Lấy dữ liệu dashboard thành công"
                };

                return BaseRespone<DashboardRevenueDto>.SuccessResponse(
                    result,
                    message,
                    HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return BaseRespone<DashboardRevenueDto>.ErrorResponse(
                    $"Lỗi hệ thống: {ex.Message}",
                    ex,
                    HttpStatusCode.InternalServerError);
            }
        }
    }
}


