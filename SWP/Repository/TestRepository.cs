using Microsoft.EntityFrameworkCore;
using SWP.Dtos.Test;
using SWP.Interfaces;
using SWP.Models;

namespace SWP.Repository
{
    public class TestRepository : ITest
    {
        private readonly HIEM_MUONContext _context;

        public TestRepository(HIEM_MUONContext context)
        {
            _context = context;
        }

        public async Task<Test?> CreateTest(Test test)
        {
            await _context.Tests.AddAsync(test);
            await _context.SaveChangesAsync();
            var result = await GetTestById(test.TestId);
            return result;
        }

        public async Task<Test?> GetTestById(int id)
        {
            return await _context.Tests
                .Include(t => t.Tp).ThenInclude(tp => tp.Cus).ThenInclude(c => c.Acc)
                .Include(t => t.Tp).ThenInclude(tp => tp.Ser)
                .Include(t => t.Tp).ThenInclude(tp => tp.StatusNavigation)
                .Include(t => t.Sd).ThenInclude(sd => sd.Doc).ThenInclude(d => d.Acc)
                .Include(t => t.Sd).ThenInclude(sd => sd.StatusNavigation)
                .Include(t => t.TestType)
                .Include(t => t.StatusNavigation)
                .Include(t => t.Tqs)
                .FirstOrDefaultAsync(t => t.TestId == id);
        }

        public async Task<List<Test>?> GetTestByStepDetailId(int stepDetailId)
        {
            return await _context.Tests
                .Include(t => t.Tp).ThenInclude(tp => tp.Cus).ThenInclude(c => c.Acc)
                .Include(t => t.Tp).ThenInclude(tp => tp.Ser)
                .Include(t => t.Tp).ThenInclude(tp => tp.StatusNavigation)
                .Include(t => t.Sd).ThenInclude(sd => sd.Doc).ThenInclude(d => d.Acc)
                .Include(t => t.Sd).ThenInclude(sd => sd.StatusNavigation)
                .Include(t => t.TestType)
                .Include(t => t.StatusNavigation)
                .Include(t => t.Tqs)
                .Where(x => x.SdId == stepDetailId).ToListAsync();
        }

        public async Task<Test?> UpdateTest(int id, UpdateTestDto request)
        {
            var exist = await GetTestById(id);
            if (exist == null)
            {
                throw new Exception("Không tìm thấy thông tin xét nghiệm");
            }
            exist.ResultDay = DateOnly.Parse(request.ResultDate);
            exist.Note = request.Note;
            exist.FilePath = request.FilePath;
            exist.Status = request.Status;
            exist.TestTypeId = request.TestType;
            exist.TqsId = request.TestQualityStatus;
            await _context.SaveChangesAsync();
            return exist;
        }
    }
}
