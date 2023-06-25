using Microsoft.EntityFrameworkCore;
using To_do_timer.DateBase;
using To_do_timer.Models.Book;

namespace To_do_timer.Services;

public class StatusRepository : BaseUserRepository<Status>
{
    public StatusRepository(BookContext bookContext) : base(bookContext)
    {
    }

    public async Task<Status?> FirstOrDefault(string nameStatus, Guid userId) // можно вынести за абстракт!!
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Name == nameStatus && e.UserId == userId);
    }
}