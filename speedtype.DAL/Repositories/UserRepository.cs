using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using speedtype.DAL.IRepositories;
using speedtype.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using speedtype.DAL.Data;

namespace speedtype.DAL.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(speedtypeContext context) : base(context)
    {
    }   

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public override async Task<bool> AddAsync(User entity)
    {
        try 
        {
            return await base.AddAsync(entity);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }   

    public override async Task<IEnumerable<User>> GetAllAsync()
    {
        try 
        {
            return await base.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public override async Task<bool> UpdateAsync(int id, User entity)
    {
        try 
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (existingUser == null)
            {
                throw new Exception("User not found");
            }
            
            existingUser.Username = entity.Username;
            existingUser.Email = entity.Email;
            existingUser.Password = entity.Password;
            
            return await base.UpdateAsync(id, existingUser);
        }
        catch (Exception ex)
        {
            throw new Exception($"Update error: {ex.Message}");
        }
    }       

    public override async Task<bool> DeleteAsync(int id)
    {
        try 
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (existingUser == null)
            {
                return false;
            }
            
            _context.Users.Remove(existingUser);
            return true;
        }   
        catch (Exception ex)        
        {
            throw new Exception($"Delete error: {ex.Message}");
        }
    }   

    public override async Task<bool> SaveAsync() 
    {
        try 
        {
            return await base.SaveAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}