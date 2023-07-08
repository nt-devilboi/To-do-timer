using System.Net;
using BBServer;
using BBServer.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using To_do_timer.Services;
using Vostok.Logging.Abstractions;

namespace To_do_timer.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes =
    JwtBearerDefaults.AuthenticationScheme)]
[Route("statistics")]
public class StatisticsController : Controller
{
    private readonly ManageBook _manageBook;
    private readonly ILog _logger;
    private readonly ParserStatsService _parserStats;

    public StatisticsController(ManageBook manageBook, ILog logger, ParserStatsService parserStats)
    {
        _logger = logger;
        _manageBook = manageBook;
        _parserStats = parserStats;
    }

    [HttpGet("/{bookId:guid}")]
    public async Task<Result<ResponseBookStats>> GetStatsDay(Guid bookId, DateTime? dateTime, Guid? statusId) // по идей можно сделать диапаозон из 2 dateTime !!
    {
        dateTime ??= DateTime.Now;

        var book = _manageBook.BookRepository.Include(b => b.Events)
            .FirstOrDefault(b => b.Id == bookId);
        
        if (book == null)
        {
            return HttpContext.WithError<ResponseBookStats>(HttpStatusCode.NotFound, "Такой книги нету");
        }
        
        var events = book?.Events
            .Where(e => e.Start.Date.DayOfYear == dateTime!.Value.Date.DayOfYear);
        
        var resStats = new ResponseBookStats()
        {
            Stats = _parserStats.GetActiveTime(events.ToList())
        };

        return HttpContext.WithResult(HttpStatusCode.OK, resStats);
    }

    [HttpGet("/{bookId:guid}/detail")]
    public async Task<Result<List<ResponseEventStats>>> GetDetailStats(Guid bookId) // по идей этот запрос кидается при запуске книжки
    {
        var dateTime = DateTime.Now;
        
        var book = _manageBook.BookRepository.Include(b => b.Events)
            .FirstOrDefault(b => b.Id == bookId);
        
        if (book == null)
        {
            return HttpContext.WithError<List<ResponseEventStats>>(HttpStatusCode.NotFound, "Такой книги нету");
        }

        var events = book.Events
            .Where(e => e.Start.Date.DayOfYear == dateTime.Date.DayOfYear).ToList();
      
        
        return  HttpContext.WithResult(HttpStatusCode.OK,_parserStats.GetActiveTimeWithStatus(events));
    }
    

}// todo когда реализуешь логику, сделай рефаторинг, а то уже куча повторов)