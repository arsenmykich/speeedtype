namespace speedtype.DAL.Entities;

public class TypingTest : IEntity
{
    public int Id { get; set; }
    public int WPM { get; set; }
    public float Accuracy { get; set; }
    public int Errors { get; set; }
    public int Time { get; set; }
    public DateTime Date { get; set; }
    public int CharactersTyped { get; set; }
    
    public Book Book { get; set; }
    public int BookId { get; set; }
    public User User { get; set; }
    public int UserId { get; set; }
}
