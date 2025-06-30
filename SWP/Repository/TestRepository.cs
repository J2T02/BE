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
                .Include(x => x.Cus)
                .Include(x => x.TestType)
                .Include(x => x.Sd).ThenInclude(x => x.StatusNavigation)
                .Include(x => x.StatusNavigation).FirstOrDefaultAsync(x => x.TestId == id);
        }

        public async Task<Test?> UpdateTest(int id, UpdateTestDto request)
        {
            var exist = await GetTestById(id);
            if (exist == null)
            {
                throw new Exception("Không tìm thấy thông tin xét nghiệm");
            }
            //exist.Result = request.Result;
            exist.Note = request.Note;
            exist.FilePath = request.FilePath;
            exist.Status = request.Status;
            exist.TestTypeId = request.TestType;
            await _context.SaveChangesAsync();
            return exist;
        }
    }
}
