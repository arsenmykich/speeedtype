using Microsoft.AspNetCore.Mvc;
using speedtype.BLL.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace speedtype.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [SwaggerOperation(Summary = "Authenticate user and return JWT token")]
    [SwaggerResponse(200, "Success")]
    [SwaggerResponse(401, "Unauthorized", typeof(string))]
    [SwaggerResponse(400, "Bad Request", typeof(ModelStateDictionary))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var user = await _authService.AuthenticateAsync(request.Email, request.Password);
            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            var token = await _authService.GenerateJwtTokenAsync(user);
            
            return Ok(new
            {
                Token = token,
                User = new
                {
                    user.Id,
                    user.Username,
                    user.Email
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("register")]
    [SwaggerOperation(Summary = "Register a new user")]
    [SwaggerResponse(201, "Created")]
    [SwaggerResponse(400, "Bad Request", typeof(string))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (request == null)
        {
            return BadRequest("Request body is required");
        }
        
        // Log the incoming request for debugging
        Console.WriteLine($"Register request received: Username={request.Username}, Email={request.Email}, Password length={request.Password?.Length}");
        
        if (!ModelState.IsValid)
        {
            Console.WriteLine("ModelState is invalid:");
            foreach (var error in ModelState)
            {
                Console.WriteLine($"  {error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
            }
            return BadRequest(ModelState);
        }

        try
        {
            var user = await _authService.RegisterAsync(request.Username, request.Email, request.Password);
            var token = await _authService.GenerateJwtTokenAsync(user);
            
            return CreatedAtAction(nameof(Login), new
            {
                Token = token,
                User = new
                {
                    user.Id,
                    user.Username,
                    user.Email
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("check-email")]
    [SwaggerOperation(Summary = "Check if email exists")]
    [SwaggerResponse(200, "Success")]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> CheckEmail([FromBody] string email)
    {
        try
        {
            var exists = await _authService.EmailExistsAsync(email);
            return Ok(new { Exists = exists });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("check-username")]
    [SwaggerOperation(Summary = "Check if username exists")]
    [SwaggerResponse(200, "Success")]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> CheckUsername([FromBody] string username)
    {
        try
        {
            var exists = await _authService.UsernameExistsAsync(username);
            return Ok(new { Exists = exists });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
} 