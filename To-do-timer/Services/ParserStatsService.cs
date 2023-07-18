using BBServer.Extensions;
using To_do_timer.Controllers;
using To_do_timer.Models.Book;

namespace To_do_timer.Services;

public class ParserStatsService
{
    public TimeSpan GetActiveTime(List<Event> events) // охх как назвать это я пока хз
    {
        var result = new TimeSpan();

        var prevEvent = events[0];
        for (int i = 1; i < events.Count; ++i)
        {
            result += events[i].Start - prevEvent.Start;
            prevEvent = events[i];
        }

        return result;
    }

    public List<ResponseEventStats>
        GetActiveTimeWithStatus(List<Event> events)
    {
        var result = new Dictionary<Guid, ResponseEventStats>();
        var prevEvent = events[0];
        for (int i = 1; i < events.Count; i++)
        {
            if (result.ContainsKey(prevEvent.StatusId))
            {
                result[prevEvent.StatusId].Time += events[i].Start - prevEvent.Start;
            }
            else
            {
                result.Add(prevEvent.StatusId, new ResponseEventStats()
                {
                    Status = prevEvent.Status.ToResponse(),
                    Time = events[i].Start - prevEvent.Start
                });
            }

            prevEvent = events[i];
        }

        return result.Values.ToList();
    }
}