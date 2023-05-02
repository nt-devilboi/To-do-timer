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
public class UserController : Controller
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

        _manageBook.BookRepository.Create(userId, requestBook.Name, requestBook.Desc);
        _manageBook.BookRepository.SaveChange();
        var book = await _manageBook.BookRepository.GetByUser(userId, requestBook.Name);
        return Ok($"{book}");
    }

    [HttpDelete("books/delete")]
    public async Task<IActionResult> DeleteBooks()
    {
        var userId = new Guid(User.FindFirst("id")?.Value!); // ой опять кринж код!
        var books = await _manageBook.BookRepository.GetAllByUser(userId);

        foreach (var book in books)
        {
            _manageBook.BookRepository.Delete(book);
            _manageBook.BookRepository.SaveChange();
        }

        HttpContext.Response.StatusCode = StatusCodes.Status202Accepted;

        return Ok("все окей!!");
    }

    [HttpGet("books")]
    public async Task<Result<Book[]>> GetUserBooks()
    {
        var userId = new Guid(User.FindFirst("id")?.Value!);
        var books = await _manageBook.BookRepository.GetAllByUser(userId);
        return new Result<Book[]>(books.ToArray());
    }

    [HttpDelete("book/delete/{id:guid}")]
    public async Task<Result<Book>> DeleteBook(Guid id)
    {
        var book =  await _manageBook.BookRepository.Get(id);
    
        if (book == null)
            return HttpContext.WithError<Book>(HttpStatusCode.NotAcceptable, "Такой книги нету ");
        HttpContext.Response.StatusCode = StatusCodes.Status202Accepted;
        _manageBook.BookRepository.Delete(book);
        _manageBook.BookRepository.SaveChange();
        return HttpContext.WithResult<Book>(HttpStatusCode.Accepted, null);
    }
}