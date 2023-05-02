using Microsoft.EntityFrameworkCore;

namespace To_do_timer.Services;

public abstract class BaseRepository<T> where T : class
{
    private DbSet<T> _dbSet;
    private DbContext _dbContext;
    public BaseRepository(DbSet<T> dbSet, DbContext dbContext)
    {
        _dbSet = dbSet;
        _dbContext = dbContext;
    }
    
    public async Task<T?> Get(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async void Delete(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}