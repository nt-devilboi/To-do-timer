using Microsoft.EntityFrameworkCore;
using To_do_timer.DateBase;
using To_do_timer.Models.Book;

namespace To_do_timer.Services;

public class EventRepository : BaseRepository<Event>
{
    private BookContext _dbcontext;
    
    public EventRepository(BookContext dbContext) : base(dbContext)
    {
        _dbcontext = dbContext;
    }
}