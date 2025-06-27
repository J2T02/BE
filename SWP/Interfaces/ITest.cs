using SWP.Models;

namespace SWP.Interfaces
{
    public interface ITest
    {
        Task<Test?> GetTestById(int id);
        Task<Test?> CreateTest(Test test);
    }
}
