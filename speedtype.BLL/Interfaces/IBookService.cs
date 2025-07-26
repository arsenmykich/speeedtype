using speedtype.DAL.Entities;

namespace speedtype.BLL.Interfaces;

public interface IBookService
{
    Task<IEnumerable<Book>> GetAllBooksAsync();
    Task<IEnumerable<Book>> GetAllPublicBooksAsync();
    Task<IEnumerable<Book>> GetPublicBooksAsync();
    Task<IEnumerable<Book>> GetBooksByUserIdAsync(int userId);
    Task<IEnumerable<Book>> GetBooksByUserAsync(int userId);
    Task<Book?> GetBookByIdAsync(int id);
    Task<Book?> GetBookWithTestsAsync(int bookId);
    Task<Book> CreateBookAsync(Book book);
    Task<Book> UpdateBookAsync(int id, Book book);
    Task DeleteBookAsync(int id);
    Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);
    Task<bool> BookExistsAsync(int id);
    Task<bool> UserCanAccessBookAsync(int userId, int bookId);
} 