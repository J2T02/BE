using SWP.Dtos.Test;
using SWP.Models;

namespace SWP.Interfaces
{
    public interface ITest
    {
        Task<Test?> GetTestById(int id);
        Task<Test?> CreateTest(Test test);
        Task<Test?> UpdateTest (int id, UpdateTestDto request);

        Task<List<Test>?> GetTestByStepDetailId(int stepDetailId);
    }
}
