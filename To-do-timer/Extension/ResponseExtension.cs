using To_do_timer.Controllers.Response;
using To_do_timer.Models.Book;

namespace BBServer.Extensions;

public static class ResponseExtension
{
    public static EventResponse ToResponse(this Event @event)
    {
        return new EventResponse()
        {
            Id = @event.Id,
            Book = @event.Book.ToResponse(),
            Start = @event.Start,
            Status = @event.Status.ToResponse()
        };
    }
    
    public static StatusResponse ToResponse(this Status status)
    {
        return new StatusResponse()
        {
            Id = status.Id,
            Desc = status.Desc,
            Name = status.Name,
        };
    }
    // реализовано отдельно, ибо не хочу фигачить, все через оди методы, а потом хоп таблица status меняеться)
    public static BookResponse ToResponse(this Book book)
    {
        return new BookResponse()
        {
            Id = book.Id,
            Desc = book.Desc,
            Name = book.Name,
        };
    }
}