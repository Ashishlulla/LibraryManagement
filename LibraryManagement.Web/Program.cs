using FluentValidation;
using LibraryManagement.API.ExceptionHandlers;
using LibraryManagement.API.Validators;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Application.Services;
using LibraryManagement.Domain.RepositoriesContracts;
using LibraryManagement.Infrastructure.Persistence;
using LibraryManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//Adding controllers
builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddScoped<IBookService, BookService>();

//Adding Repository to the container
builder.Services.AddScoped<IBookRepository, BookRepository>();

//Adding dbcontext to the container
builder.Services.AddDbContext<LibraryDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

//Adding Exception Handler Services
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


//Adding FluentValidation Validators
builder.Services.AddValidatorsFromAssemblyContaining<BookAddRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<BookUpdateRequestValidator>();

//Adding Serilog configurations
Log.Logger = new LoggerConfiguration()
    .WriteTo.Debug()
    .WriteTo.Console()
    .WriteTo.File("Logs/AppLog-.txt",
     rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

var app = builder.Build();

Log.Logger.Information("App started"); 
// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
