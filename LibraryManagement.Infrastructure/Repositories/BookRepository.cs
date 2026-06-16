
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.RepositoriesContracts;
using LibraryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        //private feilds
        private readonly LibraryDbContext _dbContext;

        //constructor
        public BookRepository(LibraryDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        //Repository Methods

        //Add Method
        public async Task<Book> AddAsync(Book book)
        {
            await _dbContext.Books.AddAsync(book);
            await _dbContext.AddRangeAsync();
            return book;
        }

        //Delete Method
        public async Task<bool> DeleteAsync(Guid id)
        {
            Book? matchingBook = await _dbContext.Books.FirstOrDefaultAsync(b=>b.BookId ==id);

            if (matchingBook == null)
            {
                return false;
            }
            _dbContext.Books.Remove(matchingBook);
            await _dbContext.SaveChangesAsync();
            return true;

        }

        //GetAllBooks Method
        public async Task<List<Book>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _dbContext.Books
                .OrderBy(b => b.Title)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        //GetBookById Method
        public async Task<Book?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Books.FirstOrDefaultAsync(b=>b.BookId == id);
        }

        public async Task<List<Book>> GetFilteredBooksAsync(string searchBy, string searchString, int PageSize, int PageNumber)
        {
            IQueryable<Book> query = _dbContext.Books;

            switch (searchBy.ToLower())
            {
                case "title":
                    query = query.Where(b => b.Title.Contains(searchString));
                    break;

                case "author":
                    query = query.Where(b => b.Author.Contains(searchString));
                    break;

                case "price":
                    if (decimal.TryParse(searchString, out decimal price))
                    {
                        query = query.Where(b => b.Price == price);
                    }
                    break;

                case "publisheddate":
                    if (DateTime.TryParse(searchString, out DateTime publisheddate))
                    {
                        query = query.Where(b => b.PublishedDate.Date == publisheddate.Date);
                    }
                    break;
            }
            return await query
                .OrderBy(b => b.Title)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }


        public async Task<List<Book>> GetSortedBooksAsync(string sortBy, string sortOrder, int PageSize, int PageNumber)
        {
            IQueryable<Book> query = _dbContext.Books;

            query = (sortBy, sortOrder.ToLower()) switch
            {
                ("Title", "asc") => query.OrderBy(b=>b.Title),
                ("Title", "desc") => query.OrderByDescending(b =>b.Title),


                ("Author", "asc") => query.OrderBy(b => b.Author),
                ("Author", "desc") => query.OrderByDescending(b => b.Author),


                ("Price", "asc") => query.OrderBy(b => b.Price),
                ("Price", "desc") => query.OrderByDescending(b => b.Price),

                ("PublishedDate", "asc") => query.OrderBy(b => b.PublishedDate),
                ("PublishedDate", "desc") => query.OrderByDescending(b => b.PublishedDate),
            };

            return await query
                .Skip((PageNumber-1)*PageSize)
                .Take(PageSize)
                .ToListAsync();

        }

        //SaveAllChanges to Database
        public async Task SaveChangesAsync()
        {
           await _dbContext.SaveChangesAsync();
        }

        public async Task<Book?> UpdateAsync(Book book)
        {
            Book? matchingBook = await _dbContext.Books.FirstOrDefaultAsync(b=>b.BookId == book.BookId);
            if (matchingBook == null)
            {
                return null;
            }
            matchingBook.Title = book.Title;
            matchingBook.Author = book.Author;
            matchingBook.Price = book.Price;
            matchingBook.PublishedDate = book.PublishedDate;

            await _dbContext.SaveChangesAsync();

            return matchingBook;
        }
    }
}
