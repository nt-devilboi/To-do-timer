using To_do_timer.Models.Book;

namespace To_do_timer.Controllers;

public class ParserStats
{
    private List<Event> EventStatsList { get; set; }

    public ParserStats()
    {
        EventStatsList = new List<Event>();
    }

    public List<ResponseEventStats> InResponseEventStats()
    {
        var result = new List<ResponseEventStats>();

        return result;
    }
}