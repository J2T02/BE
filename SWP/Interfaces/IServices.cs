
using SWP.Models;

namespace SWP.Interfaces
{
    public interface IServices
    {
        Task<SWP.Models.Service?> GetServiceById(int id);
        Task<List<SWP.Models.Service>?> GetAllServices();
    }
}
