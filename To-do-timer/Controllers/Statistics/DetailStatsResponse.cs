using To_do_timer.Controllers.Response;

namespace To_do_timer.Controllers;

public class ResponseEventStats
{
    public StatusResponse Status { get; set; }
    public TimeSpan Time { get; set; }
}