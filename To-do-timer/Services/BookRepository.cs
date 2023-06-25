using Microsoft.EntityFrameworkCore;
using To_do_timer.DateBase;
using To_do_timer.Models.Book;

namespace To_do_timer.Services;

public class BookRepository: BaseUserRepository<Book>
{
    private BookContext _db;

    public BookRepository(BookContext db) : base(db)
    {
        _db = db;
    }
    public void Create(Guid userId, string name, string desc)
    {
        var bookId = Guid.NewGuid();
        _dbSet.Add(new Book() { UserId = userId, Desc = desc, Name = name, Id = bookId });
    }
    
    // TODO формально не нужно, но пока хз
    public async Task<Book?> GetByUser(Guid userId, string name)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.UserId == userId && x.Name == name);
    }
    

    public void SaveChange()
    {
        _db.SaveChanges();
    }
}