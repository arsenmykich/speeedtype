using speedtype.DAL.Entities;

namespace speedtype.DAL.IRepositories;

public interface IBookRepository : IGenericRepository<Book>
{
    Task<IEnumerable<Book>> GetPublicBooksAsync();
    Task<IEnumerable<Book>> GetBooksByUserIdAsync(int userId);
    Task<Book?> GetBookWithTestsAsync(int bookId);
    Task<IEnumerable<Book>> GetBooksByDifficultyAsync(string difficulty);
    Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);
} 