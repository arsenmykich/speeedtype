namespace speedtype.DAL.Entities;

public class Book : IEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Author { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int PersonalBest { get; set; }
    public string Content { get; set; }
    public bool IsPublic { get; set; }
    public DateTime AddedAt { get; set; }
    
    public User? User { get; set; }
    public int? UserId { get; set; }
    public ICollection<TypingTest> TypingTests { get; set; } = new List<TypingTest>();
}
