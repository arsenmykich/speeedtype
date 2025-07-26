using System.Threading.Tasks;
using speedtype.DAL.IRepositories;

namespace speedtype.DAL.IConfiguration;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IBookRepository Books { get; }
    ITypingTestRepository TypingTests { get; }

    Task CompleteAsync();
}
