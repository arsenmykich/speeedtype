using speedtype.BLL.Interfaces;
using speedtype.DAL.Entities;
using speedtype.DAL.IConfiguration;

namespace speedtype.BLL.Services;

public class BookService : IBookService
{
    private readonly IUnitOfWork _unitOfWork;

    public BookService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        return await _unitOfWork.Books.GetAllAsync();
    }

    public async Task<IEnumerable<Book>> GetAllPublicBooksAsync()
    {
        return await _unitOfWork.Books.GetPublicBooksAsync();
    }

    public async Task<IEnumerable<Book>> GetPublicBooksAsync()
    {
        return await _unitOfWork.Books.GetPublicBooksAsync();
    }

    public async Task<IEnumerable<Book>> GetBooksByUserIdAsync(int userId)
    {
        return await _unitOfWork.Books.GetBooksByUserIdAsync(userId);
    }

    public async Task<IEnumerable<Book>> GetBooksByUserAsync(int userId)
    {
        return await _unitOfWork.Books.GetBooksByUserIdAsync(userId);
    }

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        return await _unitOfWork.Books.GetByIdAsync(id);
    }

    public async Task<Book?> GetBookWithTestsAsync(int bookId)
    {
        return await _unitOfWork.Books.GetBookWithTestsAsync(bookId);
    }

    public async Task<Book> CreateBookAsync(Book book)
    {
        book.Id = 0; // Ensure new entity
        book.AddedAt = DateTime.UtcNow;
        
        await _unitOfWork.Books.AddAsync(book);
        await _unitOfWork.CompleteAsync();
        return book;
    }

    public async Task<Book> UpdateBookAsync(int id, Book book)
    {
        var existingBook = await _unitOfWork.Books.GetByIdAsync(id);
        if (existingBook == null)
        {
            throw new ArgumentException("Book not found");
        }

        // Update properties
        existingBook.Title = book.Title;
        existingBook.Author = book.Author;
        existingBook.Description = book.Description;
        existingBook.ImageUrl = book.ImageUrl;
        existingBook.Content = book.Content;
        existingBook.IsPublic = book.IsPublic;
        
        await _unitOfWork.Books.UpdateAsync(id, existingBook);
        await _unitOfWork.CompleteAsync();
        return existingBook;
    }

    public async Task DeleteBookAsync(int id)
    {
        if (!await BookExistsAsync(id))
        {
            throw new ArgumentException("Book not found");
        }

        await _unitOfWork.Books.DeleteAsync(id);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
    {
        return await _unitOfWork.Books.SearchBooksAsync(searchTerm);
    }

    public async Task<bool> BookExistsAsync(int id)
    {
        var book = await _unitOfWork.Books.GetByIdAsync(id);
        return book != null;
    }

    public async Task<bool> UserCanAccessBookAsync(int userId, int bookId)
    {
        var book = await _unitOfWork.Books.GetByIdAsync(bookId);
        if (book == null) return false;
        
        return book.IsPublic || book.UserId == userId;
    }
} 