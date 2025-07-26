using speedtype.DAL.Entities;

namespace speedtype.BLL.Interfaces;

public interface ITypingTestService
{
    Task<IEnumerable<TypingTest>> GetAllTestsAsync();
    Task<TypingTest?> GetTestByIdAsync(int id);
    Task<IEnumerable<TypingTest>> GetTestsByUserIdAsync(int userId);
    Task<IEnumerable<TypingTest>> GetTestsByBookIdAsync(int bookId);
    Task<TypingTest?> GetBestTestByUserAndBookAsync(int userId, int bookId);
    Task<IEnumerable<TypingTest>> GetRecentTestsAsync(int userId, int count = 10);
    Task<IEnumerable<TypingTest>> GetTopTestsGloballyAsync(int count = 10);
    Task<TypingTest> CreateTestAsync(TypingTest test);
    Task<TypingTest> UpdateTestAsync(int id, TypingTest test);
    Task DeleteTestAsync(int id);
    Task<double> GetAverageWPMByUserAsync(int userId);
    Task<double> GetAverageAccuracyByUserAsync(int userId);
    Task<int> CalculateWPMAsync(string text, int timeInSeconds);
    Task<float> CalculateAccuracyAsync(string originalText, string typedText);
} 