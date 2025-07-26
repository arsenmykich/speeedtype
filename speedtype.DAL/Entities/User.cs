namespace speedtype.DAL.Entities;

public class User : IEntity
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }

    public ICollection<TypingTest> TypingTests { get; set; } = new List<TypingTest>();
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
