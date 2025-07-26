using speedtype.DAL.Entities;

namespace speedtype.BLL.Interfaces;

public interface IAuthService
{
    Task<User?> AuthenticateAsync(string email, string password);
    Task<User> RegisterAsync(string username, string email, string password);
    Task<string> GenerateJwtTokenAsync(User user);
    Task<bool> ValidatePasswordAsync(string password, string hashedPassword);
    Task<string> HashPasswordAsync(string password);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> UsernameExistsAsync(string username);
} 