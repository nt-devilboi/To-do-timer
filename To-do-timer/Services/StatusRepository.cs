using Microsoft.EntityFrameworkCore;
using To_do_timer.DateBase;
using To_do_timer.Models.Book;

namespace To_do_timer.Services;

public class StatusRepository : BaseRepository<Status>
{
    private DbContext _dbContext;
    private DbSet<Status> _dbSet;
    public StatusRepository(BookContext bookContext) : base(bookContext)
    {
        _dbContext = bookContext;
        _dbSet = bookContext.Statuses;
    }

    public async Task<Status?> FirstOrDefault(string nameStatus, Guid userId)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Name == nameStatus && e.IdUser == userId);
    }
}