using To_do_timer.Models.Book;

namespace To_do_timer.Services;

public class ManageBook
{
    public readonly IRepository<Book> BookRepository;
    public readonly IRepository<Status> StatusRepository;
    public readonly IRepository<Event> EventRepository;
 
    public ManageBook(IRepository<Book> repository, IRepository<Status> statusRepository, IRepository<Event> eventRepository)
    {
        BookRepository = repository;
        StatusRepository = statusRepository;
        EventRepository = eventRepository;
    }
    
}