using FluentValidation;
using LibraryManagement.API.ExceptionHandlers;
using LibraryManagement.API.Validators;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Application.Services;
using LibraryManagement.Domain.RepositoriesContracts;
using LibraryManagement.Infrastructure.Persistence;
using LibraryManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
