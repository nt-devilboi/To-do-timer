namespace To_do_timer.Models.Book;

public class Event : IEntity
{
    public Guid Id { get; set; }
    
    public Guid StatusId { get; set; }
    public Status Status { get; set; }
    
    public Guid BookId { get; set; }
    public Book  Book { get; set; }
    
    public DateTime Start { get; set; }
}