namespace To_do_timer.Models.Book;

public class Event
{
    public Guid Id { get; set; }
    public Guid StatusId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Desc { get; set; }
}