using Microsoft.EntityFrameworkCore;
using To_do_timer.Models.Book;

namespace To_do_timer.Services;

public abstract class BaseRepository<T> where T : class, IEntity
{
    protected DbSet<T> _dbSet;
    protected DbContext _dbContext;

    public BaseRepository(DbContext dbContext)
    {
        _dbSet = dbContext.Set<T>();
        _dbContext = dbContext;
    }

    public async void Add(T entity)
    {
        _dbSet.Add(entity);
    }
    
    public async void Delete(T book) // todo сейчас можно удалить и по id и по целому элементу, а нужно ли это?
    {
        _dbSet.Remove(book);
    }
    
    public async Task<T?> Get(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async void Delete(Guid id) // крутой delete как понять, что мы реально удалили?
    {
        var result = await _dbSet.FindAsync(id);
        if (result == null) return;

        _dbSet.Remove(result);
    }

    public async void SaveChange()
    {
        await _dbContext.SaveChangesAsync();
    }
}