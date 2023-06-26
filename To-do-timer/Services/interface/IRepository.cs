namespace To_do_timer.Services;

public interface IRepository<T>
{
    public void Add(T entity);
    public void Remove(T book);
    public void Remove(Guid id);
    public Task<T?> Get(Guid id);
    public void SaveChange();
}