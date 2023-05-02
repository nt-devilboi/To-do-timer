using Microsoft.EntityFrameworkCore;

namespace To_do_timer.Services;

public abstract class BaseRepository<T> where T : class
{
    private DbSet<T> _dbSet;
    private DbContext _dbContext;
    public BaseRepository(DbContext dbContext)
    {
        _dbSet = dbContext.Set<T>();
        _dbContext = dbContext;
    }

    public async void Add(T entity)
    {
        _dbSet.Add(entity);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task<T?> Get(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async void Delete(Guid id) // крутой delete как понять, что мы реально удалили?
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null) return;
        
        _dbSet.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }
}