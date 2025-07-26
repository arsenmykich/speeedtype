using Microsoft.AspNetCore.Mvc;
using speedtype.BLL.Interfaces;
using speedtype.DAL.Entities;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text;

namespace speedtype.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get all public books")]
    [SwaggerResponse(200, "Success", typeof(IEnumerable<Book>))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> GetAllPublicBooks()
    {
        try
        {
            var books = await _bookService.GetAllPublicBooksAsync();
            
            // Return books without content for list view (content is too large)
            var booksList = books.Select(b => new 
            {
                b.Id,
                b.Title,
                b.Author,
                b.Description,
                b.IsPublic,
                b.UserId,
                b.PersonalBest,
                b.AddedAt,
                ContentLength = b.Content?.Length ?? 0
            });
            
            return Ok(booksList);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get a book by id")]
    [SwaggerResponse(200, "Success", typeof(Book))]
    [SwaggerResponse(404, "Not Found", typeof(string))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> GetBook(int id)
    {
        try
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound("Book not found");
            }
            return Ok(book);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id}/with-tests")]
    [SwaggerOperation(Summary = "Get a book with its typing tests")]
    [SwaggerResponse(200, "Success", typeof(Book))]
    [SwaggerResponse(404, "Not Found", typeof(string))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> GetBookWithTests(int id)
    {
        try
        {
            var book = await _bookService.GetBookWithTestsAsync(id);
            if (book == null)
            {
                return NotFound("Book not found");
            }
            return Ok(book);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("user/{userId}")]
    [SwaggerOperation(Summary = "Get books by user")]
    [SwaggerResponse(200, "Success", typeof(IEnumerable<Book>))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> GetBooksByUser(int userId)
    {
        try
        {
            var books = await _bookService.GetBooksByUserAsync(userId);
            
            // Return books without full content to reduce response size
            var response = books.Select(b => new
            {
                b.Id,
                b.Title,
                b.Author,
                b.Description,
                b.ImageUrl,
                b.PersonalBest,
                b.IsPublic,
                b.AddedAt,
                b.UserId,
                ContentLength = b.Content?.Length ?? 0
                // Content excluded to reduce size
            });
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("search")]
    [SwaggerOperation(Summary = "Search books by title or author")]
    [SwaggerResponse(200, "Success", typeof(IEnumerable<Book>))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> SearchBooks([FromQuery] string query)
    {
        try
        {
            var books = await _bookService.SearchBooksAsync(query);
            return Ok(books);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new book")]
    [SwaggerResponse(201, "Created", typeof(Book))]
    [SwaggerResponse(400, "Bad Request", typeof(ModelStateDictionary))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> CreateBook([FromBody] CreateBookRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var book = new Book
            {
                Title = request.Title,
                Author = request.Author,
                Description = request.Description,
                Content = request.Content,
                IsPublic = request.IsPublic,
                UserId = request.UserId,
                AddedAt = DateTime.UtcNow,
                PersonalBest = 0
            };

            var createdBook = await _bookService.CreateBookAsync(book);
            return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, createdBook);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("upload")]
    [SwaggerOperation(Summary = "Upload a text file and create a book")]
    [SwaggerResponse(201, "Created", typeof(Book))]
    [SwaggerResponse(400, "Bad Request", typeof(string))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> UploadFile([FromForm] FileUploadRequest request)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        // Validate file type
        var allowedExtensions = new[] { ".txt", ".doc", ".docx" };
        var fileExtension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
        
        if (!allowedExtensions.Contains(fileExtension))
        {
            return BadRequest("Only .txt, .doc, and .docx files are allowed");
        }

        // Validate file size (max 5MB)
        if (request.File.Length > 5 * 1024 * 1024)
        {
            return BadRequest("File size cannot exceed 5MB");
        }

        try
        {
            string content;
            
            // Read file content based on type
            using (var stream = request.File.OpenReadStream())
            {
                if (fileExtension == ".txt")
                {
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        content = await reader.ReadToEndAsync();
                    }
                }
                else
                {
                    // For .doc and .docx files, we'll need to implement proper parsing
                    // For now, we'll treat them as text files
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        content = await reader.ReadToEndAsync();
                    }
                }
            }

            // Clean the content
            content = CleanTextContent(content);

            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest("File content is empty or invalid");
            }

            // Create the book
            var book = new Book
            {
                Title = request.Title ?? Path.GetFileNameWithoutExtension(request.File.FileName),
                Author = request.Author ?? "Unknown",
                Description = request.Description,
                Content = content,
                IsPublic = request.IsPublic,
                UserId = request.UserId,
                AddedAt = DateTime.UtcNow,
                PersonalBest = 0
            };

            var createdBook = await _bookService.CreateBookAsync(book);
            return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, createdBook);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error processing file: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update a book")]
    [SwaggerResponse(200, "Success", typeof(Book))]
    [SwaggerResponse(400, "Bad Request", typeof(ModelStateDictionary))]
    [SwaggerResponse(404, "Not Found", typeof(string))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> UpdateBook(int id, [FromBody] CreateBookRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var book = new Book
            {
                Id = id,
                Title = request.Title,
                Author = request.Author,
                Description = request.Description,
                Content = request.Content,
                IsPublic = request.IsPublic,
                UserId = request.UserId
            };

            var updatedBook = await _bookService.UpdateBookAsync(id, book);
            return Ok(updatedBook);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a book")]
    [SwaggerResponse(200, "Success")]
    [SwaggerResponse(404, "Not Found", typeof(string))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> DeleteBook(int id)
    {
        try
        {
            await _bookService.DeleteBookAsync(id);
            return Ok("Book deleted successfully");
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    private string CleanTextContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return string.Empty;

        // Remove excessive whitespace and normalize line endings
        var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                          .Select(line => line.Trim())
                          .Where(line => !string.IsNullOrWhiteSpace(line));

        return string.Join(" ", lines);
    }
}

public class CreateBookRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Author { get; set; }
    public string? Description { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public int UserId { get; set; }
}

public class FileUploadRequest
{
    public IFormFile File { get; set; } = null!;
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public int UserId { get; set; }
} 