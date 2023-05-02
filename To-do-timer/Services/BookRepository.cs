using Microsoft.EntityFrameworkCore;
using To_do_timer.DateBase;
using To_do_timer.Models.Book;

namespace To_do_timer.Services;

public class BookRepository: BaseRepository<Book>
{
    private BookContext _db;

    public BookRepository(BookContext db) : base(db)
    {
        _db = db;
    }
    public void Create(Guid userId, string name, string desc)
    {
        var bookId = Guid.NewGuid();
        _db.Books.Add(new Book() { UserId = userId, Desc = desc, Name = name, Id = bookId });
    }
    
    // TODO формально не нужно, но пока хз
    public async Task<Book?> GetByUser(Guid userId, string name)
    {
        return await _db.Books.FirstOrDefaultAsync(x => x.UserId == userId && x.Name == name);
    }

    public async Task<List<Book>> GetAllByUser(Guid userId)
    {
        return await _db.Books.Where(book => book.UserId == userId).ToListAsync();
    }

    public async void Delete(Book book)
    {
        _db.Books.Remove(book);
    }

    public void SaveChange()
    {
        _db.SaveChanges();
    }
}