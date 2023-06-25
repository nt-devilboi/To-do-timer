namespace To_do_timer.Models.Book;

public interface IEntityWithUser : IEntity // сложное название
{ 
    Guid UserId { get; set; }
}