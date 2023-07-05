namespace To_do_timer.Controllers.Response;

public class EventResponse
{
    public Guid Id { get; set; }
    public BookResponse Book { get; set; }
    public StatusResponse Status { get; set; }
    public DateTime Start { get; set; }
}