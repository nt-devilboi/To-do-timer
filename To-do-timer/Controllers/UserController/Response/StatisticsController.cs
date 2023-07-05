using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using To_do_timer.Services;
using Vostok.Logging.Abstractions;

namespace To_do_timer.Controllers.Response;

[ApiController]
[Authorize(AuthenticationSchemes =
    JwtBearerDefaults.AuthenticationScheme)] 
public class StatisticsController : Controller
{
    private ManageBook _manageBook;
    private ILog _logger;

    public StatisticsController(ManageBook manageBook, ILog logger)
    {
        _logger = logger;
        _manageBook = manageBook;
    }
    
    
    
}