using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Domain.RepositoriesContracts
{

    public interface IBookRepository
    {
        Task<Book?> GetByIdAsync(Guid id);

        Task<List<Book>> GetAllAsync(int PageSize, int PageNumber);

        Task<Book> AddAsync(Book book);

        Task<Book?> UpdateAsync(Book book);
        Task<bool> DeleteAsync(Guid id);
        Task<List<Book>> GetFilteredBooksAsync(string searchBy , string searchString, int PageSize, int PageNumber);
        Task<List<Book>> GetSortedBooksAsync(string sortBy, string sortOrder, int PageSize, int PageNumber);
        Task SaveChangesAsync();
    }
}