using To_do_timer.Models.Book;

namespace To_do_timer.Services;

public class ManageBook
{
    public readonly BookRepository BookRepository;
    public readonly StatusRepository StatusRepository;
    public readonly EventRepository EventRepository;
 
    public ManageBook(BookRepository bookRepository, StatusRepository statusRepository, EventRepository eventRepository)
    {
        BookRepository = bookRepository;
        StatusRepository = statusRepository;
        EventRepository = eventRepository;
    }
    
}