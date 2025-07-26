using speedtype.DAL.Entities;

namespace speedtype.DAL.IRepositories;

public interface ITypingTestRepository : IGenericRepository<TypingTest>
{
    Task<IEnumerable<TypingTest>> GetTestsByUserIdAsync(int userId);
    Task<IEnumerable<TypingTest>> GetTestsByBookIdAsync(int bookId);
    Task<TypingTest?> GetBestTestByUserAndBookAsync(int userId, int bookId);
    Task<IEnumerable<TypingTest>> GetRecentTestsAsync(int userId, int count = 10);
    Task<IEnumerable<TypingTest>> GetTopTestsGloballyAsync(int count = 10);
    Task<double> GetAverageWPMByUserAsync(int userId);
    Task<double> GetAverageAccuracyByUserAsync(int userId);
} 