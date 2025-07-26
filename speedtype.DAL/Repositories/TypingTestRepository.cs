using Microsoft.EntityFrameworkCore;
using speedtype.DAL.Data;
using speedtype.DAL.Entities;
using speedtype.DAL.IRepositories;

namespace speedtype.DAL.Repositories;

public class TypingTestRepository : GenericRepository<TypingTest>, ITypingTestRepository
{
    public TypingTestRepository(speedtypeContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TypingTest>> GetTestsByUserIdAsync(int userId)
    {
        return await _context.TypingTests
            .Where(t => t.UserId == userId)
            .Include(t => t.Book)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<TypingTest>> GetTestsByBookIdAsync(int bookId)
    {
        return await _context.TypingTests
            .Where(t => t.BookId == bookId)
            .Include(t => t.User)
            .OrderByDescending(t => t.WPM)
            .ToListAsync();
    }

    public async Task<TypingTest?> GetBestTestByUserAndBookAsync(int userId, int bookId)
    {
        return await _context.TypingTests
            .Where(t => t.UserId == userId && t.BookId == bookId)
            .OrderByDescending(t => t.WPM)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<TypingTest>> GetRecentTestsAsync(int userId, int count = 10)
    {
        return await _context.TypingTests
            .Where(t => t.UserId == userId)
            .Include(t => t.Book)
            .OrderByDescending(t => t.Date)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<TypingTest>> GetTopTestsGloballyAsync(int count = 10)
    {
        return await _context.TypingTests
            .Include(t => t.User)
            .Include(t => t.Book)
            .OrderByDescending(t => t.WPM)
            .Take(count)
            .ToListAsync();
    }

    public async Task<double> GetAverageWPMByUserAsync(int userId)
    {
        var tests = await _context.TypingTests
            .Where(t => t.UserId == userId)
            .ToListAsync();
        
        return tests.Any() ? tests.Average(t => t.WPM) : 0;
    }

    public async Task<double> GetAverageAccuracyByUserAsync(int userId)
    {
        var tests = await _context.TypingTests
            .Where(t => t.UserId == userId)
            .ToListAsync();
        
        return tests.Any() ? tests.Average(t => t.Accuracy) : 0;
    }
} 