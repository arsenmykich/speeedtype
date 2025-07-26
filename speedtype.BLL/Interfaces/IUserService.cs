using speedtype.DAL.Entities;

namespace speedtype.BLL.Interfaces;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User> CreateUserAsync(User user);
    Task<User> UpdateUserAsync(int id, User user);
    Task DeleteUserAsync(int id);
    Task<bool> UserExistsAsync(int id);
    Task<bool> EmailExistsAsync(string email);
} 