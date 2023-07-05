using To_do_timer.Models.Book;

namespace To_do_timer.Services;

public class AnalyzerService
{
    
    public TimeSpan CalculationTime(List<Event> events)
    {
        var result = new TimeSpan();

        var prevEvent = events[0];
        for (int i = 1; i < events.Count; i++)
        {
            result += events[i].Start - prevEvent.Start;
            prevEvent = events[i];
        }

        return result;
    }
}