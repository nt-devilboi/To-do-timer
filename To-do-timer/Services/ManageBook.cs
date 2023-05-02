namespace To_do_timer.Services;

public class ManageBook
{
    public readonly BookRepository BookRepository;
    public readonly StatusRepository StatusRepository;
    
    public ManageBook(BookRepository bookRepository, StatusRepository statusRepository)
    {
        BookRepository = bookRepository;
        StatusRepository = statusRepository;
    }
    
}