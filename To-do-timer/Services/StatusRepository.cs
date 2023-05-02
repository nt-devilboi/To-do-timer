using Microsoft.EntityFrameworkCore;
using To_do_timer.DateBase;
using To_do_timer.Models.Book;

namespace To_do_timer.Services;

public class StatusRepository : BaseRepository<Status>
{
    private DbContext _dbContext;

    public StatusRepository(BookContext bookContext) : base(bookContext)
    {
        _dbContext = bookContext;
    }
}