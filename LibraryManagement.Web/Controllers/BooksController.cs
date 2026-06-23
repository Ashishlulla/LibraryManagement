using LibraryManagement.Application.DTO;
using LibraryManagement.Application.Exceptions;
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
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookService bookService,ILogger<BooksController> logger) 
        {
            _bookService = bookService;
            _logger = logger;
        }

        //POST BOOK(ADD BOOK) CONTROLLER ACTION

        /// <summary>
        /// Add valid BookAdd request object to database
        /// </summary>
        /// <param name="bookAddRequest">Object contains book values</param>
        /// <returns>Return Book Respose Object of Added Book if succeed</returns>

        [HttpPost]
        [ProducesResponseType(typeof(BookResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookResponse>> AddBook( BookAddRequest? bookAddRequest) 
        {
            BookResponse addedBook = await  _bookService.AddBookAsync(bookAddRequest);

            return CreatedAtAction(nameof(GetBookById),new { id = addedBook.BookId },addedBook);
        }

        //GET BOOK BY ID CONTROLLER ACTION

        /// <summary>
        /// Retrieves Book by its unique Identifier.
        /// </summary>
        /// <param name="id">The Unique Identifier of Book</param>
        /// <returns>Returns the requested Book</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BookResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookResponse>> GetBookById(Guid id) 
        {
            Console.WriteLine($"Controller received: {id}");

            BookResponse? book = await _bookService.GetBookByIdAsync(id);

            return Ok(book);
        }

        //GET ALL BOOK CONTROLLER ACTION

        /// <summary>
        /// Retrives All Books
        /// </summary>
        /// <param name="PageSize">Feild use to number of Book Object on sinfle page</param>
        /// <param name="PageNumber">Feild use to show data of specigic page number</param>
        /// <returns>Returns All the Books from Database </returns>

        [HttpGet("{PageSize:int}/{PageNumber:int}")]
        [ProducesResponseType(typeof(PagedResult<BookResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAllBooks(int PageSize, int PageNumber) 
        {
            var books = await _bookService.GetAllBookAsync(PageSize, PageNumber);

            return Ok(books);
        }

        //GET FILTERED BOOKS CONTROLLER ACTION

        /// <summary>
        /// return the filtered based parametere passed by user
        /// </summary>
        /// <param name="searchBy">Field used for filtering (Title, Author, Price, PublishedDate).</param>
        /// <param name="searchString">Field used for filtering value</param>
        /// <param name="PageSize" >Feild used for showing number of values in on single.</param>
        /// <param name="PageNumber">Feild Used to see value of specific Page number</param>
        /// <returns>Returns Filtered Data</returns>
        
        [HttpGet("filter")]
        [ProducesResponseType(typeof(PagedResult<BookResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]

        public async Task<ActionResult> GetFilteredBooks(string searchBy, string searchString, int PageSize, int PageNumber) 
        {
            PagedResult<BookResponse?> filteredBooks = await _bookService.GetFilteredBooksAsync(searchBy, searchString,PageSize, PageNumber);

            return Ok(filteredBooks);
        }

        //GET Sorted BOOKS CONTROLLER ACTION

        /// <summary>
        /// Retrieves Book in Ascending or Descending as per user requirement.
        /// </summary>
        /// <param name="sortBy">Field used for Sorting (Title, Author, Price, PublishedDate).</param>
        /// <param name="sortOrder">Feild use decide sort order.</param>
        /// <param name="PageSize">Feild used for showing number of values in on single.</param>
        /// <param name="PageNumber">Feild Used to see value of specific Page number.</param>
        /// <returns>Return  Books in Sorted order</returns>
        
        [HttpGet("sort")]
        [ProducesResponseType(typeof(PagedResult<BookResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]

        public async Task<ActionResult> GetSortedBooksAsync(string sortBy, string sortOrder, int PageSize, int PageNumber)
        {
            PagedResult<BookResponse> sortedBooks = await _bookService.GetSortedBooksAsync(sortBy, sortOrder, PageSize, PageNumber);

            return Ok(sortedBooks);
        }


        //PUT BOOK(UPDATE BOOK) CONTROLLER ACTION

        /// <summary>
        /// Updates an existing book
        /// </summary>
        /// <param name="id">The Unique Identifier to retrieve book from Database</param>
        /// <param name="bookUpdateRequest">This BookUpdateRequest Object contains  updated values</param>
        /// <returns> Return BookResponse object with updated values</returns>
       
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(BookResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]

        public async Task<ActionResult> UpdateBook(Guid id, [FromBody] BookUpdateRequest bookUpdateRequest)
        { 
            BookResponse? updatedBook = await _bookService.UpdateBookAsync(id, bookUpdateRequest);

            return Ok(updatedBook);
        }

        //DELETE BOOK(DELETE BOOK) CONTROLLER ACTION

        /// <summary>
        /// Delete an existing book by its uniqie identifier
        /// </summary>
        /// <param name="id">The unique identifier of book</param>
        /// <returns>Returns Ok Content showing success meesage</returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteBook(Guid id) 
        {
            bool isdeleted  = await _bookService.DeleteBookAsync(id);

            return Ok($"Book Object with BookId {id} deleted Sucessfully...");
        }
    }
}
