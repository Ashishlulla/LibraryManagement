using FluentValidation;
using LibraryManagement.Application.DTO;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.RepositoriesContracts;
using Microsoft.Extensions.Logging;


namespace LibraryManagement.Application.Services 
{
    public class BookService : IBookService
    {
        //Private Fields
        private readonly IBookRepository _bookRepository;

        private readonly IValidator<BookAddRequest> _addValidator;
        private readonly IValidator<BookUpdateRequest> _updateValidator;

        private readonly ILogger<BookService> _logger;

        //Constructor
        public BookService(IBookRepository bookRepository, IValidator<BookAddRequest> addValidator, IValidator<BookUpdateRequest> updateValidator, ILogger<BookService> logger) 
        {
            _bookRepository = bookRepository;
            _addValidator = addValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }


        //Adding BOOK SERVICE METHODS

        //METHOD -- ADD BOOK SERVICE METHOD
        public async Task<BookResponse> AddBookAsync(BookAddRequest? bookAddRequest)
        {
            if (bookAddRequest == null)
            {
                throw new BadRequestException("Request to book cannot be null");
            }

            var validationResult = await _addValidator.ValidateAsync(bookAddRequest);
            
            if (!validationResult.IsValid)
            {
                throw new BadRequestException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                
            }

            //converting BookAddRequest Object to Book Object
            Book bookRequest = bookAddRequest.ToBook();
            
            //Generating BookId New Book
            bookRequest.BookId = Guid.NewGuid();

            //calling repository method to add new Book Object
            Book addedBook = await _bookRepository.AddAsync(bookRequest);

            _logger.LogInformation($"created Book with Title : {addedBook.Title}");
            //returning Book response Object
            return addedBook.ToBookResponse();
        }

        //METHOD -- DELETE BOOK SERVICE METHOD
        public async Task<bool> DeleteBookAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Book id cannot be empty");

            var result = await _bookRepository.DeleteAsync(id);

            if (result == false)
            {
                _logger.LogWarning($"Book Not found. BookId : {id}");
                throw new NotFoundException($"Book with Id = {id} not found Operation UnSucessfull...");
            }
             
            _logger.LogInformation($"Deleted the book with Id = {id}");
            return result;
        }

        //METHOD -- GETALLBOOK BOOK SERVICE METHOD
        public async Task<PagedResult<BookResponse>> GetAllBookAsync(int PageSize, int PageNumber)
        {
            List<Book> books = await _bookRepository.GetAllAsync(PageSize, PageNumber);
            int TotalCount = books.Count;
            return new PagedResult<BookResponse>
            {
                items = books.Select(b => new BookResponse
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Author = b.Author,
                    Price = b.Price,
                    PublishedDate = b.PublishedDate

                }).ToList(),
                PageNumber = PageNumber,
                PageSize = PageSize,
                TotalCount = TotalCount,
                TotalPages =(TotalCount+PageSize -1)/PageSize
            };
        }

        //METHOD -- GETBOOKBYID SERVICE METHOD
        public async Task<BookResponse?> GetBookByIdAsync(Guid BookId)
        {
            if (BookId == Guid.Empty)
            {
                throw new BadRequestException("Book Id must not be emppty...");
            }

            Book? booktoFind = await _bookRepository.GetByIdAsync(BookId);
            
            if (booktoFind is null)
            {
                _logger.LogWarning($"Book Not found.BookId : {BookId}");
                throw new NotFoundException($"Book with {BookId} not found. Please provide valid BookId");
                
            }
            return booktoFind.ToBookResponse();
        }

        //METHOD --GET FILTERED BOOK SERVICE METHOD
        public async Task<PagedResult<BookResponse>> GetFilteredBooksAsync(string searchBy, string searchString, int PageSize, int PageNumber)
        {
            List<Book> filteredBooks = await _bookRepository.GetFilteredBooksAsync(searchBy, searchString, PageSize, PageNumber);
            int totalCount = filteredBooks.Count();

            if (totalCount == 0)
            {
                throw new NotFoundException($"No Data exist for provided input searchBy = {searchBy} and searchString = {searchString} ");
            }

            return new PagedResult<BookResponse>
            {
                items = filteredBooks.Select(b => new BookResponse
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Author = b.Author,
                    Price = b.Price,
                    PublishedDate = b.PublishedDate
                }).ToList(),

                PageSize = PageSize,
                PageNumber = PageNumber,
                TotalCount = totalCount,
                TotalPages = (totalCount+PageSize-1)/PageSize
            };
        }


        //METHOD --GET SORTED BOOK SERVICE METHOD
        public async Task<PagedResult<BookResponse>> GetSortedBooksAsync(string sortBy, string sortOrder, int PageSize, int PageNumber)
        {
            if (sortOrder.ToLower() != "asc" && sortOrder.ToLower() != "desc") 
            {
                throw new BadRequestException("sortOrder must be either 'asc' or 'desc' no other values are alowed...");
            }
            List<Book> sortedBooks = await _bookRepository.GetSortedBooksAsync(sortBy, sortOrder, PageSize, PageNumber);
            int totalCount = sortedBooks.Count();

            return new PagedResult<BookResponse>
            {
                items = sortedBooks.Select(b=> new BookResponse 
                {
                    BookId = b.BookId,
                    Title =b.Title,
                    Author = b.Author,
                    Price = b.Price,
                    PublishedDate = b.PublishedDate
                }).ToList(),

                PageSize = PageSize,
                PageNumber = PageNumber,
                TotalCount = totalCount,
                TotalPages = (totalCount+PageSize-1)/PageSize

            };
        }

        

        //METHOD --UPDATEBOOK SERVICE METHOD
        public async Task<BookResponse?> UpdateBookAsync(Guid id, BookUpdateRequest? request)
        {
            
            if (request == null)
            {
                throw new BadRequestException("request to update cannot be null");
            }

            if (request.BookId == Guid.Empty || id == Guid.Empty)
            {
                throw new BadRequestException("Book Id cannot be empty");
            }

            var validationResult = await _updateValidator.ValidateAsync(request);
            
            if (!validationResult.IsValid)
            {
                throw new BadRequestException(string.Join(", ", validationResult.Errors.Select(e=>e.ErrorMessage)));                
            }
            Book? existingbook = await _bookRepository.GetByIdAsync(id);

            if (existingbook == null)
            {
                _logger.LogWarning($"Book Not found.BookId : {id}");

                throw new NotFoundException($"No Book existing with id = {id},Please provide valid book id.");
            }
            existingbook.Title = request.Title;
            existingbook.Author = request.Author;
            existingbook.Price = request.Price;
            existingbook.PublishedDate = request.PublishedDate;

            Book? updatedBook = await _bookRepository.UpdateAsync(existingbook);
            if (updatedBook == null)
            {
                return null;
            }
            
            _logger.LogInformation($"Updated the Book with id = {id}");
            return updatedBook.ToBookResponse();
        }        
    }
}