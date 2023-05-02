using Microsoft.EntityFrameworkCore;
using To_do_timer.DateBase;
using To_do_timer.Models.Book;

namespace To_do_timer.Services;

public class StatusRepository
{
    private DbSet<Status> _status = new BookContext(new DbContextOptions<BookContext>()).Statuses;

    public StatusRepository()
    {
    }
}