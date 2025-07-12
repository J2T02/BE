using SWP.Dtos.Dashboard;

namespace SWP.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardRevenueDto> GetRevenueDashboardAsync(DashboardFilterDto filter);

    }
}
