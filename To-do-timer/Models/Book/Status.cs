namespace To_do_timer.Models.Book;

public class Status : IUserConnect
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Desc { get; set; }

    public List<Event> Events { get; set; }
}