using System.Net;
using BBServer;
using BBServer.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using To_do_timer.Controllers.Response;
using To_do_timer.Models;
using To_do_timer.Models.Book;
using To_do_timer.Services;
using Vostok.Logging.Abstractions;

namespace To_do_timer.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes =
    JwtBearerDefaults.AuthenticationScheme)] // помни тесты тоже нужно писать)))
public class
    UserController : Controller // todo название такое себе имхо, здесь вообще другое тоже имхо..... реализовать отдельные контроллы под каждую задачу book, status event, а так слишком много кода в одном месте))
{
    private ManageBook _manageBook;
    private ILog _logger;

    public UserController(ManageBook manageBook, ILog logger)
    {
        _logger = logger;
        _manageBook = manageBook;
    }

    // todo реализация получение книжки по id; можно попрожбовать использовать чистый sql для таких запросв
    [HttpPost("book/create")]
    public async Task<Result<Book>> CreateBook([FromBody] RequestBook requestBook)
    {
        var userId = new Guid(User.FindFirst("id")?.Value!); // выглядит как кринж код!
        if (await _manageBook.BookRepository.FirstOrDefaultAsync(b => b.UserId ==  userId && b.Name == requestBook.Name) != null)
            return HttpContext.WithError<Book>(HttpStatusCode.Conflict, "такая книга уже есть");

        var book = new Book()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Desc = requestBook.Desc,
            Name = requestBook.Name
        };

        _manageBook.BookRepository.Add(book);
        _manageBook.BookRepository.SaveChange();

        book = await _manageBook.BookRepository.GetById(book.Id);
        return HttpContext.WithResult(HttpStatusCode.OK, book);
    }

    // они идентичны, но я хз может потом буду усложнять эти таблицы
    [HttpPost("status/create")]
    public async Task<Result<Status>>
        CreateStatus([FromBody] StatusRequest statusRequest) // todo Какое должно быть возращаемое значиен??
    {
        var userId = new Guid(User.FindFirst("id")?.Value!);
        var status = await _manageBook.StatusRepository.FirstOrDefaultAsync(s => s.Name == statusRequest.Name && s.UserId == userId);
        if (status != null)
            return HttpContext.WithError<Status>(HttpStatusCode.Conflict, "с таким именем уже есть");

        status = new Status()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Desc = statusRequest.Desc,
            Name = statusRequest.Name
        };


        _manageBook.StatusRepository.Add(status);
        _manageBook.StatusRepository.SaveChange();

        var statusSave = await _manageBook.StatusRepository.GetById(status.Id);

        if (statusSave == null)
            return HttpContext.WithError<Status>(HttpStatusCode.Conflict, "статус не сохранён");

        return HttpContext.WithResult(HttpStatusCode.OK, statusSave);
    }
    
    [HttpPost("event/create")]
    public async Task<Result<Event>>
        EventCreate([FromBody] EventRequest eventRequest) // здесь явно можно как то сделать проще и красивее)
    {
        var userId = new Guid(User.FindFirst("id")?.Value!);

        var book = (await GetBook(eventRequest.BookId)).Value;
        var status = (await GetStatus(eventRequest.StatusId)).Value;

        if (book == null || status == null)
            return HttpContext.WithError<Event>(HttpStatusCode.NotAcceptable, "либо нету такой книжки либо статуса");
        if (book.UserId != userId || status.UserId != userId)
            return HttpContext.WithError<Event>(HttpStatusCode.Forbidden, "эти данные другого юзера");

        var @event = new Event()
        {
            Id = Guid.NewGuid(),
            Book = book,
            BookId = book.Id,
            Status = status,
            StatusId = status.Id,
            Start = DateTime.UtcNow
        };
        
        _manageBook.EventRepository.Add(@event);
        _manageBook.EventRepository.SaveChange();

        return HttpContext.WithResult(HttpStatusCode.OK, @event);
    }

    [HttpDelete("books/delete")]
    public async Task<IActionResult> DeleteBooks()
    {
        return await RemoveEntitiesByUser(_manageBook.BookRepository);
    }

    [HttpGet("books")]
    public async Task<Result<List<Book>>> GetUserBooks()
    {
        return await GetAllEntityByUser(_manageBook.BookRepository);
    }

    [HttpGet("book/{id:guid}")]
    public async Task<Result<Book>> GetBook(Guid id)
    {
        return await GetEntityByUser(_manageBook.BookRepository, id);
    }

    [HttpDelete("book/delete/{id:guid}")] 
    public async Task<Result<Book>> DeleteBook(Guid id)
    {
        return await RemoveEntityByUser(_manageBook.BookRepository, id);
    }

    [HttpDelete("status/delete/{id:guid}")]
    public async Task<Result<Status>> DeleteStatus(Guid id)
    {
        return await RemoveEntityByUser(_manageBook.StatusRepository, id);
    }

    [HttpGet("status")]
    public async Task<Result<List<Status?>>> GetAllStatuses() // todo Какое должно быть возращаемое значиен??
    {
        return await GetAllEntityByUser(_manageBook.StatusRepository);
    }

    [HttpPost("status/{id:guid}")]
    public async Task<Result<Status>> GetStatus(Guid id)
    {
        return await GetEntityByUser(_manageBook.StatusRepository, id);
    }

    [HttpPost("end")] 
    public async Task<Result<Event>> End(BookIdRequest bookIdRequest) // формально тоже самое, что и выше, но 
    {
        var userId = new Guid(User.FindFirst("id")?.Value!);

        var book = (await GetBook(bookIdRequest.BookId)).Value;
        var status = await _manageBook.StatusRepository.GetById(AdminStatuses.UnknownStatusId); // можно вынести это за метод!!!
       
        if (status == null)
            return HttpContext.WithError<Event>(HttpStatusCode.NotFound, "Нету Статуса");
        if (book == null)
            return HttpContext.WithError<Event>(HttpStatusCode.NotFound, "Нету книжки");
        if (book.UserId != userId)
            return HttpContext.WithError<Event>(HttpStatusCode.Forbidden, "эти данные другого юзера");

        var @event = new Event()
        {
            Id = Guid.NewGuid(),
            Book = book,
            BookId = book.Id,
            Status = status,
            StatusId = status.Id,
            Start = DateTime.UtcNow
        };

        _manageBook.EventRepository.Add(@event);
        _manageBook.EventRepository.SaveChange();

        return HttpContext.WithResult(HttpStatusCode.OK, @event);
    }

    [HttpGet("Book/{id:guid}/events")]
    public async Task<Result<List<EventResponse>>> GetEventsFromBook(Guid id)
    {
        var userId = new Guid(User.FindFirst("id")?.Value!);
        var book = (await GetEntityByUser(_manageBook.BookRepository, id)).Value;

        if (book == null)
            return HttpContext.WithError<List<EventResponse>>(HttpStatusCode.NotFound, "Такой книжки нету");
        if (book.UserId != userId)
            return HttpContext.WithError<List<EventResponse>>(HttpStatusCode.NotFound,
                "эта книжка другого пользователя");

        var eventsRespons = _manageBook
            .EventRepository // пока выглядит как кринж, но думаю, потом по изучаю ef подробнее
            .Include(x => x.Book)
            .Include(x => x.Status)
            .Where(x => x.BookId == book.Id)
            .Select(x => x.ToResponse())
            .ToList();

        return HttpContext.WithResult(HttpStatusCode.OK, eventsRespons);
    }

    private async Task<IActionResult> RemoveEntitiesByUser<T>(IRepository<T> repository) where T : class, IUserConnect
    {
        var userId = new Guid(User.FindFirst("id")?.Value!); // ой опять кринж код!
        var entities = await repository.Where(e => e.UserId == userId);

        foreach (var entity in entities)
        {
            repository.Remove(entity);
        }

        repository.SaveChange();
        HttpContext.Response.StatusCode = StatusCodes.Status202Accepted;
        return Ok("все окей!!");
    }

    private async Task<Result<T>> RemoveEntityByUser<T>(IRepository<T> repository, Guid id) where T : class, IEntity
    {
        var entity = await repository.GetById(id); // сейчас любой юзер может удалить данные другого юзера

        if (entity == null)
            return HttpContext.WithError<T>(HttpStatusCode.NotFound, "ничего не нашел");

        repository.Remove(entity);
        repository.SaveChange();

        return HttpContext.WithResult(HttpStatusCode.Accepted, entity);
    }

    private async Task<Result<List<T?>>> GetAllEntityByUser<T>(IRepository<T> repositoryWithUser) where T : class, IUserConnect // todo Какое должно быть возращаемое значиен??
    {
        var userId = new Guid(User.FindFirst("id")?.Value!);

        var statuses = await repositoryWithUser.Where(e => e.UserId == userId);
        return  HttpContext.WithResult(HttpStatusCode.Accepted, statuses.ToList());
    }

    private async Task<Result<T>> GetEntityByUser<T>(IRepository<T> repository, Guid id) 
        where T : class, IUserConnect // сделать более универсальным GetenitytBy(Guid id)? тогда в гипотетический придется делать более унивелальнуым Repositoies 
    {
        var userId = new Guid(User.FindFirst("id")?.Value!);

        var entity = await repository.FirstOrDefaultAsync(e => e.Id == id && e.UserId  == userId);
        if (entity == null)
            return HttpContext.WithError<T>(HttpStatusCode.NotFound, "У данного юзера этих данных нет");

        return HttpContext.WithResult(HttpStatusCode.OK, entity);
    }
}