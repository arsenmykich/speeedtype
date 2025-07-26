using Microsoft.EntityFrameworkCore;
using speedtype.DAL.Data;
using speedtype.DAL.Entities;
using speedtype.DAL.IRepositories;

namespace speedtype.DAL.Repositories;

public class BookRepository : GenericRepository<Book>, IBookRepository
{
    public BookRepository(speedtypeContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Book>> GetPublicBooksAsync()
    {
        return await _context.Books
            .Where(b => b.IsPublic)
            .Include(b => b.User)
            .OrderByDescending(b => b.AddedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetBooksByUserIdAsync(int userId)
    {
        return await _context.Books
            .Where(b => b.UserId == userId)
            .Include(b => b.TypingTests)
            .OrderByDescending(b => b.AddedAt)
            .ToListAsync();
    }

    public async Task<Book?> GetBookWithTestsAsync(int bookId)
    {
        return await _context.Books
            .Include(b => b.TypingTests)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == bookId);
    }

    public async Task<IEnumerable<Book>> GetBooksByDifficultyAsync(string difficulty)
    {
        // This is a placeholder - you might want to add a Difficulty property to Book
        return await _context.Books
            .Where(b => b.IsPublic)
            .Include(b => b.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
    {
        return await _context.Books
            .Where(b => b.IsPublic && 
                       (b.Title.Contains(searchTerm) || 
                        b.Author.Contains(searchTerm) || 
                        b.Description.Contains(searchTerm)))
            .Include(b => b.User)
            .OrderByDescending(b => b.AddedAt)
            .ToListAsync();
    }
} 