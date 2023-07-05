using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using To_do_timer.DateBase;
using To_do_timer.Models.Book;

namespace To_do_timer.Services;

public class Repository<T> : IRepository<T> where T : class, IEntity
{
    private BookContext _db;

    public Repository(BookContext db)
    {
        _db = db;
    }

    public IIncludableQueryable<T, TGet> Include<TGet>(Expression<Func<T, TGet>> predict)
    {
        return _db.Set<T>().Include(predict);
    }
    // TODO формально не нужно, но пока хз
    public async Task<T?> GetById(Guid id)
    {
        return await _db.Set<T>().FirstOrDefaultAsync(e => e.Id == id);
    }
    

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predict)
    {
        return await _db.Set<T>().FirstOrDefaultAsync(predict);
    }

    public void Add(T entity)
    {
        _db.Set<T>().Add(entity);
    }

    public void Remove(T entity)
    {
        _db.Set<T>().Remove(entity);
    }

    public Task<IEnumerable<T?>> Where(Expression<Func<T, bool>> predict)
    {
        return Task.FromResult<IEnumerable<T?>>(_db.Set<T>().Where(predict));
    }

    public void SaveChange()
    {
        _db.SaveChanges();
    }
}