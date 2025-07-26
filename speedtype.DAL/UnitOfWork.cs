using System;
using System.Threading.Tasks;
using speedtype.DAL.Repositories;
using speedtype.DAL.IRepositories;
using speedtype.DAL.Entities;
using speedtype.DAL.Data;
using speedtype.DAL.IConfiguration;

namespace speedtype.DAL;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly speedtypeContext _context;
    public IUserRepository Users => new UserRepository(_context);
    public IBookRepository Books => new BookRepository(_context);
    public ITypingTestRepository TypingTests => new TypingTestRepository(_context);

    public UnitOfWork(speedtypeContext context)
    {
        _context = context;
    }

    public async Task CompleteAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}