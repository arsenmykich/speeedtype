using speedtype.DAL.Entities;
using Microsoft.EntityFrameworkCore;


namespace speedtype.DAL.Data;

public class speedtypeContext : DbContext
{
    public speedtypeContext(DbContextOptions<speedtypeContext> options) : base(options)
    {   }
    public DbSet<Book> Books { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<TypingTest> TypingTests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Book>().HasMany(b => b.TypingTests).WithOne(t => t.Book).HasForeignKey(t => t.BookId);
        modelBuilder.Entity<User>().HasMany(u => u.TypingTests).WithOne(t => t.User).HasForeignKey(t => t.UserId);
        modelBuilder.Entity<User>().HasMany(u => u.Books).WithOne(b => b.User).HasForeignKey(b => b.UserId);
    }

}