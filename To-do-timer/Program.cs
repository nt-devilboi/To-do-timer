using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using To_do_timer.DateBase;
using To_do_timer.MiddleWares;
using To_do_timer.Models;
using To_do_timer.Models.Book;
using To_do_timer.Properties;
using To_do_timer.Services;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Console;
using Vostok.Logging.File;
using Vostok.Logging.File.Configuration;
using Vostok.Logging.Microsoft;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var consoleLog = builder.Environment.IsDevelopment()
    ? (ILog)new SynchronousConsoleLog()
    : new ConsoleLog();
var fileLog = new FileLog(new FileLogSettings()
{
    WaitIfQueueIsFull = true,
    RollingStrategy = new RollingStrategyOptions()
    {
        Period = RollingPeriod.Day,
        Type = RollingStrategyType.ByTime,
        MaxFiles = 7
    }
});

var log = new CompositeLog(consoleLog, fileLog);
builder.Logging.ClearProviders();
builder.Logging.AddVostok(log);
builder.Services.AddSingleton<ILog>(log);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<UserContext>();
builder.Services.AddDbContext<BookContext>();
builder.Services.AddScoped<ManageBook>();
builder.Services.AddScoped<IRepository<Book>, Repository<Book>>();
builder.Services.AddScoped<IRepository<Status>, Repository<Status>>();
builder.Services.AddScoped<IRepository<Event>, Repository<Event>>();
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<UserContext>()
    .AddDefaultTokenProviders();



builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore // хз насколько это правильно
    );
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = AuthOptions.ISSUER,
        ValidateAudience = true,
        ValidAudience = AuthOptions.AUDIENCE,
        ValidateLifetime = true,
        RequireExpirationTime = true,
        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
        ValidateIssuerSigningKey = true
    };
});
builder.Services.AddAuthorization();
builder.Services.AddScoped<AnalyzerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthentication();
app.UseAuthorization();
app.Map("/hello", [Authorize]() => "Hello World!");
app.Map("/", () => "Home Page");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();