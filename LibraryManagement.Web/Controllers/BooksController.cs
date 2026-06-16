using LibraryManagement.Application.DTO;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService) 
        {
            _bookService = bookService;
        }

        //POST BOOK(ADD BOOK) CONTROLLER ACTION
        [HttpPost]
        public async Task<ActionResult<BookResponse>> AddBook( BookAddRequest? bookAddRequest) 
        {
            BookResponse addedBook = await  _bookService.AddBookAsync(bookAddRequest);

            return CreatedAtAction(nameof(GetBookById),new { id = addedBook.BookId },addedBook);
        }

        //GET BOOK BY ID CONTROLLER ACTION
        [HttpGet("{id:guid}")]

        public async Task<ActionResult<BookResponse>> GetBookById(Guid id) 
        {
            BookResponse? book = await _bookService.GetBookByIdAsync(id);

            return Ok(book);
        }

        //GET ALL BOOK CONTROLLER ACTION
        [HttpGet("{PageSize:int}/{PageNumber:int}")]
        public async Task<ActionResult> GetAllBooks(int PageSize, int PageNumber) 
        {
            var books = await _bookService.GetAllBookAsync(PageSize, PageNumber);

            return Ok(books);
        }

        //GET FILTERED BOOKS CONTROLLER ACTION
        [HttpGet("filter")]

        public async Task<ActionResult> GetFilteredBooks(string searchBy, string searchString, int PageSize, int PageNumber) 
        {
            PagedResult<BookResponse> filteredBooks = await _bookService.GetFilteredBooksAsync(searchBy, searchString,PageSize, PageNumber);

            return Ok(filteredBooks);
        }

        //GET Sorted BOOKS CONTROLLER ACTION
        [HttpGet("sort")]

        public async Task<ActionResult> GetSortedBooksAsync(string sortBy, string sortOrder, int PageSize, int PageNumber)
        {
            PagedResult<BookResponse> sortedBooks = await _bookService.GetSortedBooksAsync(sortBy, sortOrder, PageSize, PageNumber);

            return Ok(sortedBooks);
        }


        //PUT BOOK(UPDATE BOOK) CONTROLLER ACTION
        [HttpPut("{id:guid}")]
        public async Task<ActionResult> UpdateBook(Guid id, [FromBody] BookUpdateRequest bookUpdateRequest)
        { 
            BookResponse? updatedBook = await _bookService.UpdateBookAsync(id, bookUpdateRequest);

            return Ok(updatedBook);
        }

        //DELETE BOOK(DELETE BOOK) CONTROLLER ACTION
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteBook(Guid id) 
        {
            bool isdeleted  = await _bookService.DeleteBookAsync(id);

            return Ok($"Book Object with BookId {id} deleted Sucessfully...");
        }
    }
}
