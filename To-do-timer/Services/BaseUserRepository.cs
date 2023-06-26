using Microsoft.EntityFrameworkCore;
using To_do_timer.DateBase;
using To_do_timer.Models.Book;

namespace To_do_timer.Services;

public class BaseUserRepository<T> : BaseRepository<T>, IRepositoryWithUser<T> where T : class, IEntityWithUser
{
    public BaseUserRepository(DbContext dbContext) : base(dbContext)
    {
    }
    
    public async Task<List<T>> GetAllByUser(Guid userId)
    {
        return await _dbSet.Where(curEntity => curEntity.UserId == userId).ToListAsync();
    }

    public Task<T?> GetByUser(Guid id, Guid userId) // по идей классно, сделать через func, и как linq операций, но пока думаю. сейчас просто код очень хорошо  себя документирует, но падает гибкость, да и плюч писать много случаев. хотя по идей в данном случае это не критично, так как мало условий
    {
        return Task.FromResult(_dbSet.FirstOrDefault(b => b.Id == id && b.UserId == userId)); // это выглядит странно
    }
    
}