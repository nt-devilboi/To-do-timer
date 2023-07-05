namespace To_do_timer.Controllers;

public class EventRequest
{
    public Guid BookId { get; set; }
    public Guid StatusId { get; set; }
}

public class BookIdRequest
{
    public Guid BookId { get; set; }
}