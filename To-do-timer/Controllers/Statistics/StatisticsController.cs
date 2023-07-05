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
public class StatisticsController : Controller
{
    private ManageBook _manageBook;
    private ILog _logger;
    private AnalyzerService _analyzer;
    public StatisticsController(ManageBook manageBook, ILog logger, AnalyzerService analyzer)
    {
        _logger = logger;
        _manageBook = manageBook;
        _analyzer = analyzer;
    }

    [HttpGet("Statistics")]
    public async Task<Result<ResponseStats>> GetStats(Guid bookId, DateTime? dateTime)
    {
        dateTime ??= DateTime.Now;

        var book = _manageBook.BookRepository.Include(b => b.Events)
            .FirstOrDefault(b => b.Id == bookId);
        
        var events = book.Events.Where(e => e.Start.Date.DayOfYear == dateTime.Value.Date.DayOfYear);
        var resStats = new ResponseStats()
        {
            Stats = _analyzer.CalculationTime(events.ToList())
        };
        
        return HttpContext.WithResult(HttpStatusCode.OK, resStats);
    }


}