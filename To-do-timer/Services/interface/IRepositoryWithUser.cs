namespace To_do_timer.Services;

public interface IRepositoryWithUser<T> : IRepository<T>
{
    public Task<List<T>> GetAllByUser(Guid userId);
    public Task<T?> GetByUser(Guid id, Guid userId);
}