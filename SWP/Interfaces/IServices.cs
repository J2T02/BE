
namespace SWP.Interfaces
{
    public interface IServices
    {
        Task<SWP.Models.Service?> GetServiceById(int id);
    }
}
