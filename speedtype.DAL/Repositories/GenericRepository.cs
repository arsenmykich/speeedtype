using speedtype.DAL.Data;
using speedtype.DAL.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace speedtype.DAL.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly speedtypeContext _context;

    private readonly DbSet<T> _dbSet;

    public GenericRepository(speedtypeContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public virtual async Task<bool> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return true;
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }   

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }       

    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null) 
        {
            _dbSet.Remove(entity);
        }
        return true;
    }

    public virtual async Task<bool> UpdateAsync(int id, T entity)
    {
        var existingEntity = await GetByIdAsync(id);
        if (existingEntity == null)
        {
            return false;
        }
        _context.Entry(existingEntity).CurrentValues.SetValues(entity);
        return true;
    }

    public virtual async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
