using System.Net;
using BBServer;
using BBServer.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using To_do_timer.Models;
using To_do_timer.Models.Book;
using To_do_timer.Services;

namespace To_do_timer.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserController : Controller // todo название такое себе имхо, здесь вообще другое тоже имхо..... реализовать отдельные контроллы под каждую задачу book, status event, а так слишком много кода в одном месте))
{
    private ManageBook _manageBook;

    public UserController(ManageBook manageBook)
    {
        _manageBook = manageBook;
    }

    // todo реализация получение книжки по id; можно попрожбовать использовать чистый sql для таких запросв
    [HttpPost("book/create")]
    public async Task<IActionResult> CreateBook([FromBody] RequestBook requestBook)
    {
        var userId = new Guid(User.FindFirst("id")?.Value!); // выглядит как кринж код!
        if (await _manageBook.BookRepository.GetByUser(userId, requestBook.Name) != null)
            return Ok("Такая уже есть");

        var book = new Book()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Desc = requestBook.Desc,
            Name = requestBook.Name
        };


        _manageBook.BookRepository.Add(book);
        _manageBook.BookRepository.SaveChange();

        book = await _manageBook.BookRepository.GetByUser(userId, requestBook.Name);
        return Ok($"{book}");
    }

    // они идентичны, но я хз может потом буду усложнять эти таблицы
    [HttpPost("status/create")]
    public async Task<Result<Status>> CreateStatus([FromBody] StatusRequest statusRequest) // todo Какое должно быть возращаемое значиен??
    {
        var userId = new Guid(User.FindFirst("id")?.Value!);
        var status = await _manageBook.StatusRepository.FirstOrDefault(statusRequest.Name, userId);
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

        var statusSave = await _manageBook.StatusRepository.Get(status.Id);

        if (status == null)
            return HttpContext.WithError<Status>(HttpStatusCode.Conflict, "статус не сохранён");

        return HttpContext.WithResult(HttpStatusCode.Accepted, statusSave);
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

    [HttpDelete("book/delete/{id:guid}")] // можно удалить книгу другого пользователя   
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
    public async Task<Result<List<Status>>> GetAllStatuses() // todo Какое должно быть возращаемое значиен??
    {
        return await GetAllEntityByUser(_manageBook.StatusRepository);
    }

    [HttpPost("status/{id:guid}")]
    public async Task<Result<Status>> GetStatus(Guid id)
    {
        return await GetEntityByUser(_manageBook.StatusRepository, id);
    }

    [HttpPost("event/create")]
    public async Task<Result<Event>> EventCreate([FromBody] EventRequest eventRequest) // здесь явно можно как то сделать проще и красивее)
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
        
        // а что насчёт книги и статуса. мы не добавляем в них этот event
        _manageBook.EventRepository.Add(@event);
        _manageBook.EventRepository.SaveChange();
        
        return HttpContext.WithResult(HttpStatusCode.OK,@event);
    }

    /*[HttpGet("Book/{id:guid}/events")]  сделать более универальным!!)()
    public async Task<Result<List<Event>>> GetEventsFromBook(Guid id) 
    {
        var book = (await GetEntityByUser(_manageBook.BookRepository, id)).Value;
        
        
    }*/
    private async Task<IActionResult> RemoveEntitiesByUser<T>(IRepositoryWithUser<T> repository)
    {
        var userId = new Guid(User.FindFirst("id")?.Value!); // ой опять кринж код!
        var entities = await repository.GetAllByUser(userId);

        foreach (var entity in entities)
        {
            repository.Remove(entity);
        }
        
        repository.SaveChange();
        HttpContext.Response.StatusCode = StatusCodes.Status202Accepted;
        return Ok("все окей!!");
    }

    private async Task<Result<T>> RemoveEntityByUser<T>(IRepositoryWithUser<T> repository, Guid id) where T : class
    {
        var entity = await repository.Get(id); // сейчас любой юзер может удалить данные другого юзера

        if (entity == null)
            return HttpContext.WithError<T>(HttpStatusCode.NotFound, "ничего не нашел");

        HttpContext.Response.StatusCode = StatusCodes.Status202Accepted;
        repository.Remove(entity);
        repository.SaveChange();
        
        return HttpContext.WithResult(HttpStatusCode.Accepted, entity);
    }

    private async Task<Result<List<T>>>
        GetAllEntityByUser<T>(IRepositoryWithUser<T> repositoryWithUser) // todo Какое должно быть возращаемое значиен??
    {
        var userId = new Guid(User.FindFirst("id")?.Value!);

        var statuses = await repositoryWithUser.GetAllByUser(userId);
        return HttpContext.WithResult(HttpStatusCode.Accepted, statuses);
    }

    private async Task<Result<T>> GetEntityByUser<T>(IRepositoryWithUser<T> repository, Guid id) where T : class // сделать более универсальным GetenitytBy(Guid id)? тогда в гипотетический придется делать более унивелальнуым Repositoies 
    {
        var userId = new Guid(User.FindFirst("id")?.Value!);

        var book = await repository.GetByUser(id, userId);
        if (book == null)
            return HttpContext.WithError<T>(HttpStatusCode.NotFound, "У данного юзера этих данных нет");

        return HttpContext.WithResult(HttpStatusCode.OK, book);
    }
}