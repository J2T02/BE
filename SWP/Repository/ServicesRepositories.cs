using Microsoft.EntityFrameworkCore;
using SWP.Interfaces;
using SWP.Models;

namespace SWP.Repository
{
    public class ServicesRepositories : IServices
    {
        private readonly HIEM_MUONContext _context;

        public ServicesRepositories(HIEM_MUONContext context)
        {
            _context = context;
        }
        public async Task<Models.Service?> GetServiceById(int id)
        {
            return await _context.Services.FirstOrDefaultAsync(x => x.SerId == id);
        }
    }
}
