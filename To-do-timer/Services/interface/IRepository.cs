using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using To_do_timer.Models.Book;

namespace To_do_timer.Services;

public interface IRepository<T> where T: class, IEntity
{
    public void Add(T entity);
    public void Remove(T book);
    public Task<T?> GetById(Guid id);

    public IIncludableQueryable<T, TGet> Include<TGet>(Expression<Func<T, TGet>> predict);
   
    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>>  predict);
    public Task<IEnumerable<T?>> Where(Expression<Func<T, bool>> predict);
    public void SaveChange();
}