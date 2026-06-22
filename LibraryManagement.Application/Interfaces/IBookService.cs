using LibraryManagement.Application.DTO;
using LibraryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Interfaces
{
    public interface IBookService
    {
        Task<BookResponse> AddBookAsync(BookAddRequest? bookAddRequest);
        Task<PagedResult<BookResponse>> GetAllBookAsync(int PageSize, int PageNumber);
        Task<BookResponse?> GetBookByIdAsync(Guid BookId);
        Task<BookResponse?> UpdateBookAsync(Guid id, BookUpdateRequest? bookUpdateRequest);

        Task<PagedResult<BookResponse?>> GetFilteredBooksAsync(string searchBy, string searchString, int PageSize, int PageNumber);
        Task<PagedResult<BookResponse>> GetSortedBooksAsync(string sortBy, string sortOrder, int PageSize, int PageNumber);
        Task<bool> DeleteBookAsync(Guid id);
    }
}
