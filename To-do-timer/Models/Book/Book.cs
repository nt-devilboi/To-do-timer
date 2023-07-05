namespace To_do_timer.Models.Book;

public class Book : IUserConnect
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Desc { get; set; }
    
    public List<Event> Events { get; set; }
    public Guid UserId { get; set; }
}