using FluentValidation;
using LibraryManagement.API.ExceptionHandlers;
using LibraryManagement.API.Validators;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Application.Services;
using LibraryManagement.Domain.RepositoriesContracts;
using LibraryManagement.Infrastructure.Persistence;
using LibraryManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

//Adding controllers
builder.Services.AddControllers();

//Adding Swagger Service
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => 
{
    options.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "Library Management API",
        Version ="v1",
        Description = "API for Managing books",
        Contact = new OpenApiContact 
        {
            Name = "Ashish Lulla",
            Email = "Lullaashish2807@gmail.com"
        }, 
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

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

app.UseSwagger();
app.UseSwaggerUI();

app.UseExceptionHandler();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();
