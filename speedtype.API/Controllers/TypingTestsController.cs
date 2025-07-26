using Microsoft.AspNetCore.Mvc;
using speedtype.BLL.Interfaces;
using speedtype.DAL.Entities;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace speedtype.API.Controllers;

public class CreateTypingTestRequest
{
    [Required]
    public int WPM { get; set; }
    
    [Required]
    [Range(0, 100)]
    public float Accuracy { get; set; }
    
    [Required]
    public int Errors { get; set; }
    
    [Required]
    public int Time { get; set; }
    
    [Required]
    public int CharactersTyped { get; set; }
    
    [Required]
    public int BookId { get; set; }
    
    [Required]
    public int UserId { get; set; }
}

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TypingTestsController : ControllerBase
{
    private readonly ITypingTestService _typingTestService;

    public TypingTestsController(ITypingTestService typingTestService)
    {
        _typingTestService = typingTestService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get all typing tests")]
    [SwaggerResponse(200, "Success", typeof(IEnumerable<TypingTest>))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> GetAllTests()
    {
        var tests = await _typingTestService.GetAllTestsAsync();
        return Ok(tests);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get a typing test by id")]
    [SwaggerResponse(200, "Success", typeof(TypingTest))]
    [SwaggerResponse(404, "Not Found", typeof(string))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> GetTest(int id)
    {
        var test = await _typingTestService.GetTestByIdAsync(id);
        if (test == null)
        {
            return NotFound("Test not found");
        }
        return Ok(test);
    }

    [HttpGet("user/{userId}")]
    [SwaggerOperation(Summary = "Get typing tests by user id")]
    [SwaggerResponse(200, "Success", typeof(IEnumerable<TypingTest>))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> GetTestsByUser(int userId)
    {
        var tests = await _typingTestService.GetTestsByUserIdAsync(userId);
        return Ok(tests);
    }

    [HttpGet("book/{bookId}")]
    [SwaggerOperation(Summary = "Get typing tests by book id")]
    [SwaggerResponse(200, "Success", typeof(IEnumerable<TypingTest>))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> GetTestsByBook(int bookId)
    {
        var tests = await _typingTestService.GetTestsByBookIdAsync(bookId);
        return Ok(tests);
    }

    [HttpGet("user/{userId}/recent")]
    [SwaggerOperation(Summary = "Get recent typing tests by user")]
    [SwaggerResponse(200, "Success", typeof(IEnumerable<TypingTest>))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> GetRecentTests(int userId, [FromQuery] int count = 10)
    {
        var tests = await _typingTestService.GetRecentTestsAsync(userId, count);
        
        // Remove full book content to reduce response size
        var response = tests.Select(t => new
        {
            t.Id,
            t.WPM,
            t.Accuracy,
            t.Errors,
            t.Time,
            t.Date,
            t.CharactersTyped,
            Book = new
            {
                t.Book.Id,
                t.Book.Title,
                t.Book.Author,
                t.Book.Description,
                t.Book.PersonalBest
                // Content excluded to reduce size
            }
        });
        
        return Ok(response);
    }

    [HttpGet("leaderboard")]
    [SwaggerOperation(Summary = "Get top typing tests globally")]
    [SwaggerResponse(200, "Success", typeof(IEnumerable<TypingTest>))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> GetLeaderboard([FromQuery] int count = 10)
    {
        var tests = await _typingTestService.GetTopTestsGloballyAsync(count);
        
        // Remove full book content to reduce response size
        var response = tests.Select(t => new
        {
            t.Id,
            t.WPM,
            t.Accuracy,
            t.Errors,
            t.Time,
            t.Date,
            t.CharactersTyped,
            User = new
            {
                t.User.Id,
                t.User.Username
            },
            Book = new
            {
                t.Book.Id,
                t.Book.Title,
                t.Book.Author
                // Content excluded to reduce size
            }
        });
        
        return Ok(response);
    }

    [HttpGet("top")]
    [SwaggerOperation(Summary = "Get top typing tests globally")]
    [SwaggerResponse(200, "Success", typeof(IEnumerable<TypingTest>))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> GetTopTests([FromQuery] int count = 10)
    {
        var tests = await _typingTestService.GetTopTestsGloballyAsync(count);
        return Ok(tests);
    }

    [HttpGet("user/{userId}/best/{bookId}")]
    [SwaggerOperation(Summary = "Get best test for user and book")]
    [SwaggerResponse(200, "Success", typeof(TypingTest))]
    [SwaggerResponse(404, "Not Found", typeof(string))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> GetBestTest(int userId, int bookId)
    {
        var test = await _typingTestService.GetBestTestByUserAndBookAsync(userId, bookId);
        if (test == null)
        {
            return NotFound("No test found for this user and book");
        }
        return Ok(test);
    }

    [HttpGet("user/{userId}/stats")]
    [SwaggerOperation(Summary = "Get user statistics")]
    [SwaggerResponse(200, "Success")]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> GetUserStats(int userId)
    {
        var avgWpm = await _typingTestService.GetAverageWPMByUserAsync(userId);
        var avgAccuracy = await _typingTestService.GetAverageAccuracyByUserAsync(userId);
        
        var stats = new
        {
            UserId = userId,
            AverageWPM = avgWpm,
            AverageAccuracy = avgAccuracy
        };
        
        return Ok(stats);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new typing test")]
    [SwaggerResponse(201, "Created", typeof(TypingTest))]
    [SwaggerResponse(400, "Bad Request", typeof(ModelStateDictionary))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> CreateTest(CreateTypingTestRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var test = new TypingTest
            {
                WPM = request.WPM,
                Accuracy = request.Accuracy,
                Errors = request.Errors,
                Time = request.Time,
                CharactersTyped = request.CharactersTyped,
                BookId = request.BookId,
                UserId = request.UserId,
                Date = DateTime.UtcNow
            };
            
            var createdTest = await _typingTestService.CreateTestAsync(test);
            return CreatedAtAction(nameof(GetTest), new { id = createdTest.Id }, createdTest);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update a typing test by id")]
    [SwaggerResponse(200, "Success", typeof(TypingTest))]
    [SwaggerResponse(400, "Bad Request", typeof(ModelStateDictionary))]
    [SwaggerResponse(404, "Not Found", typeof(string))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> UpdateTest(int id, TypingTest test)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updatedTest = await _typingTestService.UpdateTestAsync(id, test);
            return Ok(updatedTest);
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
    [SwaggerOperation(Summary = "Delete a typing test by id")]
    [SwaggerResponse(200, "Success", typeof(string))]
    [SwaggerResponse(404, "Not Found", typeof(string))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> DeleteTest(int id)
    {
        try
        {
            await _typingTestService.DeleteTestAsync(id);
            return Ok("Test deleted successfully");
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
} 