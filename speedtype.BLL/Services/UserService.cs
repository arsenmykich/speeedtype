using speedtype.BLL.Interfaces;
using speedtype.DAL.Entities;
using speedtype.DAL.IConfiguration;

namespace speedtype.BLL.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _unitOfWork.Users.GetAllAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _unitOfWork.Users.GetByIdAsync(id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _unitOfWork.Users.GetUserByEmail(email);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        if (await EmailExistsAsync(user.Email))
        {
            throw new InvalidOperationException("Email already exists");
        }

        user.Id = 0; // Ensure new entity
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CompleteAsync();
        return user;
    }

    public async Task<User> UpdateUserAsync(int id, User user)
    {
        var existingUser = await _unitOfWork.Users.GetByIdAsync(id);
        if (existingUser == null)
        {
            throw new ArgumentException("User not found");
        }

        // Update properties
        existingUser.Username = user.Username;
        existingUser.Email = user.Email;
        
        await _unitOfWork.Users.UpdateAsync(id, existingUser);
        await _unitOfWork.CompleteAsync();
        return existingUser;
    }

    public async Task DeleteUserAsync(int id)
    {
        if (!await UserExistsAsync(id))
        {
            throw new ArgumentException("User not found");
        }

        await _unitOfWork.Users.DeleteAsync(id);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<bool> UserExistsAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        return user != null;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var user = await _unitOfWork.Users.GetUserByEmail(email);
        return user != null;
    }
} 