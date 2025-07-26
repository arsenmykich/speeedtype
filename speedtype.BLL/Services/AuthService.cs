using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using speedtype.BLL.Interfaces;
using speedtype.DAL.Entities;
using speedtype.DAL.IConfiguration;

namespace speedtype.BLL.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<User?> AuthenticateAsync(string email, string password)
    {
        var user = await _unitOfWork.Users.GetUserByEmail(email);
        if (user == null) return null;

        var isValidPassword = await ValidatePasswordAsync(password, user.Password);
        return isValidPassword ? user : null;
    }

    public async Task<User> RegisterAsync(string username, string email, string password)
    {
        if (await EmailExistsAsync(email))
        {
            throw new InvalidOperationException("Email already exists");
        }

        if (await UsernameExistsAsync(username))
        {
            throw new InvalidOperationException("Username already exists");
        }

        var hashedPassword = await HashPasswordAsync(password);
        
        var user = new User
        {
            Username = username,
            Email = email,
            Password = hashedPassword
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CompleteAsync();
        
        return user;
    }

    public async Task<string> GenerateJwtTokenAsync(User user)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? "SpeedTypeAPI";
        var jwtAudience = _configuration["Jwt:Audience"] ?? "SpeedTypeAPI";

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<bool> ValidatePasswordAsync(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

    public async Task<string> HashPasswordAsync(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var user = await _unitOfWork.Users.GetUserByEmail(email);
        return user != null;
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        return users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }
} 