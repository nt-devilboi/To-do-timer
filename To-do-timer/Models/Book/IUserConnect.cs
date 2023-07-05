namespace To_do_timer.Models.Book;

public interface IUserConnect : IEntity // сложное название
{ 
    Guid UserId { get; set; }
}