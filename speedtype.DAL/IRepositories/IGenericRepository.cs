using System.Threading.Tasks;
using System.Collections.Generic;

namespace speedtype.DAL.IRepositories;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<bool> AddAsync(T entity);
    Task<bool> DeleteAsync(int id);
    Task<bool> UpdateAsync(int id, T entity);
    Task<bool> SaveAsync();
}
