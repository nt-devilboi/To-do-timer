using Microsoft.EntityFrameworkCore;
using To_do_timer.DateBase;
using To_do_timer.Models.Book;

namespace To_do_timer.Services;

public class BaseUserRepository<T> : BaseRepository<T> where T : class, IEntityWithUser
{
    public BaseUserRepository(DbContext dbContext) : base(dbContext)
    {
    }
    
    public async Task<List<T>> GetAllByUser(Guid userId)
    {
        return await _dbSet.Where(curEntity => curEntity.UserId == userId).ToListAsync();
    }

    
}