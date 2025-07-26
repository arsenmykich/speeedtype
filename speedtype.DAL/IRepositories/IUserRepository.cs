using System.Threading.Tasks;
using speedtype.DAL.Entities;

namespace speedtype.DAL.IRepositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetUserByEmail(string email);
}
