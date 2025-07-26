using speedtype.BLL.Interfaces;
using speedtype.DAL.Entities;
using speedtype.DAL.IConfiguration;

namespace speedtype.BLL.Services;

public class TypingTestService : ITypingTestService
{
    private readonly IUnitOfWork _unitOfWork;

    public TypingTestService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<TypingTest>> GetAllTestsAsync()
    {
        return await _unitOfWork.TypingTests.GetAllAsync();
    }

    public async Task<TypingTest?> GetTestByIdAsync(int id)
    {
        return await _unitOfWork.TypingTests.GetByIdAsync(id);
    }

    public async Task<IEnumerable<TypingTest>> GetTestsByUserIdAsync(int userId)
    {
        return await _unitOfWork.TypingTests.GetTestsByUserIdAsync(userId);
    }

    public async Task<IEnumerable<TypingTest>> GetTestsByBookIdAsync(int bookId)
    {
        return await _unitOfWork.TypingTests.GetTestsByBookIdAsync(bookId);
    }

    public async Task<TypingTest?> GetBestTestByUserAndBookAsync(int userId, int bookId)
    {
        return await _unitOfWork.TypingTests.GetBestTestByUserAndBookAsync(userId, bookId);
    }

    public async Task<IEnumerable<TypingTest>> GetRecentTestsAsync(int userId, int count = 10)
    {
        return await _unitOfWork.TypingTests.GetRecentTestsAsync(userId, count);
    }

    public async Task<IEnumerable<TypingTest>> GetTopTestsGloballyAsync(int count = 10)
    {
        return await _unitOfWork.TypingTests.GetTopTestsGloballyAsync(count);
    }

    public async Task<TypingTest> CreateTestAsync(TypingTest test)
    {
        test.Id = 0; // Ensure new entity
        test.Date = DateTime.UtcNow;
        
        await _unitOfWork.TypingTests.AddAsync(test);
        await _unitOfWork.CompleteAsync();
        
        // Update personal best for the book if this is better
        await UpdatePersonalBestAsync(test.UserId, test.BookId, test.WPM);
        
        return test;
    }

    public async Task<TypingTest> UpdateTestAsync(int id, TypingTest test)
    {
        var existingTest = await _unitOfWork.TypingTests.GetByIdAsync(id);
        if (existingTest == null)
        {
            throw new ArgumentException("Test not found");
        }

        // Update properties
        existingTest.WPM = test.WPM;
        existingTest.Accuracy = test.Accuracy;
        existingTest.Errors = test.Errors;
        existingTest.Time = test.Time;
        existingTest.CharactersTyped = test.CharactersTyped;
        
        await _unitOfWork.TypingTests.UpdateAsync(id, existingTest);
        await _unitOfWork.CompleteAsync();
        return existingTest;
    }

    public async Task DeleteTestAsync(int id)
    {
        var test = await _unitOfWork.TypingTests.GetByIdAsync(id);
        if (test == null)
        {
            throw new ArgumentException("Test not found");
        }

        await _unitOfWork.TypingTests.DeleteAsync(id);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<double> GetAverageWPMByUserAsync(int userId)
    {
        return await _unitOfWork.TypingTests.GetAverageWPMByUserAsync(userId);
    }

    public async Task<double> GetAverageAccuracyByUserAsync(int userId)
    {
        return await _unitOfWork.TypingTests.GetAverageAccuracyByUserAsync(userId);
    }

    public async Task<int> CalculateWPMAsync(string text, int timeInSeconds)
    {
        if (timeInSeconds <= 0) return 0;
        
        // Standard calculation: (characters typed / 5) / (time in minutes)
        var wordsTyped = text.Length / 5.0;
        var timeInMinutes = timeInSeconds / 60.0;
        
        return (int)Math.Round(wordsTyped / timeInMinutes);
    }

    public async Task<float> CalculateAccuracyAsync(string originalText, string typedText)
    {
        if (string.IsNullOrEmpty(originalText) || string.IsNullOrEmpty(typedText))
            return 0;

        var maxLength = Math.Max(originalText.Length, typedText.Length);
        var correctChars = 0;

        for (int i = 0; i < maxLength; i++)
        {
            if (i < originalText.Length && i < typedText.Length)
            {
                if (originalText[i] == typedText[i])
                    correctChars++;
            }
        }

        return (float)correctChars / originalText.Length * 100;
    }

    private async Task UpdatePersonalBestAsync(int userId, int bookId, int wpm)
    {
        var book = await _unitOfWork.Books.GetByIdAsync(bookId);
        if (book != null && book.UserId == userId && wpm > book.PersonalBest)
        {
            book.PersonalBest = wpm;
            await _unitOfWork.Books.UpdateAsync(bookId, book);
            await _unitOfWork.CompleteAsync();
        }
    }
} 