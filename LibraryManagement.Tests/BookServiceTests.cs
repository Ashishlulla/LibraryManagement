using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using LibraryManagement.Application.DTO;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Application.Services;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.RepositoriesContracts;
using Microsoft.Extensions.Logging;
using Moq;


namespace LibraryManagement.Tests
{
    public class BookServiceTests
    {
        //Private Fields
        private readonly IBookService _bookservice;

        //Mocking private feilds
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        
        //Mock Validators
        private readonly Mock<IValidator<BookAddRequest>> _addValidatorMock;
        private readonly Mock<IValidator<BookUpdateRequest>> _updateValidatorMock;
        
        //Mocking ILogger
        private readonly Mock<ILogger<BookService>> _loggerMock;

        public BookServiceTests() 
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _addValidatorMock = new Mock<IValidator<BookAddRequest>>();
            _updateValidatorMock = new Mock<IValidator<BookUpdateRequest>>();
            _loggerMock = new Mock<ILogger<BookService>>();

            _bookservice = new BookService(_bookRepositoryMock.Object, _addValidatorMock.Object, _updateValidatorMock.Object,_loggerMock.Object);
        }

        #region AddBook
        [Fact]
        public async Task CreateBookAsync_ShouldReturnBookResponse_WhenBookIsCreated() 
        {
            //Arrange
            BookAddRequest request = new BookAddRequest
            {
                Title = "test Book",
                Author = "Test Name",
                Price = 599,
                PublishedDate = DateTime.UtcNow,
            };

            ValidationResult validationResult = new ValidationResult();
            
           _addValidatorMock.Setup(v=>v.ValidateAsync(request, default)).ReturnsAsync(validationResult);

            Book expected = request.ToBook();

            _bookRepositoryMock.Setup(b => b.AddAsync(It.IsAny<Book>())).ReturnsAsync(expected);

            //Act
            BookResponse actual = await _bookservice.AddBookAsync(request);

            //Assert
            actual.Should().NotBeNull();
            
            expected.Title.Should().Be(actual.Title);
            expected.Author.Should().Be(actual.Author);
            expected.Price.Should().Be(actual.Price);
            expected.PublishedDate.Should().Be(actual.PublishedDate);

            //Verify Validator only called once
            _addValidatorMock.Verify(v => v.ValidateAsync(request, default), Times.Once);

            //Verify Book Repository only called Once
            _bookRepositoryMock.Verify(v => v.AddAsync(It.IsAny<Book>()), Times.Once);
        }

        [Fact]
        public async Task AddBookAsync_ShouldThrowBadRequestException_WhenRequestIsNull() 
        {
            //Arrange
            BookAddRequest? request = null;

            //Act + Assert
            BadRequestException exception = await Assert.ThrowsAsync<BadRequestException>(()=>_bookservice.AddBookAsync(request));

            exception.Message.Should().Be("Request to book cannot be null");
        }

        [Fact]
        public async Task AddBookAsync_ShouldThrowBadRequestException_WhenRequestIsNotValid()
        {
            //Arrange
            BookAddRequest? request = new BookAddRequest
            {
                Title ="",
                Author = "T",
                Price = 599,
                PublishedDate = DateTime.UtcNow,
            };

            List<ValidationFailure> failures = new List<ValidationFailure>
            {
                new ValidationFailure("Title", "Title cannot be empty"),
                new ValidationFailure("Author", "Author cannot be empty")
            };

            ValidationResult validationResult = new ValidationResult(failures);

             _addValidatorMock.Setup(b => b.ValidateAsync(request, default)).ReturnsAsync(validationResult);

            //Act + Assert
            BadRequestException exception = await Assert.ThrowsAsync<BadRequestException>(async () => await _bookservice.AddBookAsync(request));

            exception.Message.Should().Contain("Title cannot be empty");
            exception.Message.Should().Contain("Author cannot be empty");


            //Verifications

            _addValidatorMock.Verify(v => v.ValidateAsync(request, default), Times.Once);

            _bookRepositoryMock.Verify(b => b.AddAsync(It.IsAny<Book>()), Times.Never);


        }
        #endregion

        #region GetBookById
        [Fact]
        public async Task GetBookByIdAsync_ShouldReturnBookResponse_WhenBookExists()
        {
            // Arrange
            Book book = new Book
            {
                BookId = Guid.NewGuid(),
                Title = "Test Book",
                Author = "Test Name",
                Price = 599,
                PublishedDate = DateTime.UtcNow,
            };

            _bookRepositoryMock.Setup(b => b.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(book);
            
            // Act
            BookResponse? actual = await _bookservice.GetBookByIdAsync(book.BookId);

            // Assert
            actual.Should().NotBeNull();

            actual.Title.Should().Be(book.Title);
            actual.Author.Should().Be(book.Author);
            actual.Price.Should().Be(book.Price);
            actual.PublishedDate.Should().Be(book.PublishedDate);

            //Verify
            _bookRepositoryMock.Verify(v => v.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task GetBookByIdAsync_ShouldThrowBadRequestException_WhenBookIdIsEmpty() 
        {
            //Arrange
            Book book = new Book
            {
                BookId = Guid.Empty,
                Title = "Test Book",
                Author = "Test Name",
                Price = 599,
                PublishedDate = DateTime.UtcNow,
            };

            //Act + Assert
            BadRequestException exception = await Assert.ThrowsAnyAsync<BadRequestException>(async () =>
            {
                await _bookservice.GetBookByIdAsync(book.BookId);
            });

            exception.Message.Should().Be("Book Id must not be emppty...");

            //Verify
            _bookRepositoryMock.Verify(v => v.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetBookByIdAsync_ShouldThrowNotFoundException_WhenBookDoesNotExists() 
        {
            //Arrange
            Book book = new Book
            {
                BookId = Guid.Parse("D6C4243B-6F19-4271-8E2E-C1E38B9B16B8"),
                Title = "Test Book",
                Author = "Test Name",
                Price = 599,
                PublishedDate = DateTime.UtcNow,
            };

            Guid idToFind = Guid.NewGuid();

            _bookRepositoryMock.Setup(b => b.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Book?) null);

            //Act + Assert
            NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await _bookservice.GetBookByIdAsync(idToFind);
            });

            exception.Message.Should().Be($"Book with {idToFind} not found. Please provide valid BookId");

            //Verify
            _bookRepositoryMock.Verify(v => v.GetByIdAsync(idToFind),Times.Once);
        }
        #endregion

        #region DeleteBookById
        [Fact]
        public async Task DeleteBookById_ShouldDeleteBook_WhenExist() 
        {
            //Arrange
            Book book = new Book
            {
                BookId = Guid.Parse("D6C4243B-6F19-4271-8E2E-C1E38B9B16B8"),
                Title = "Test Book",
                Author = "Test Name",
                Price = 599,
                PublishedDate = DateTime.UtcNow,
            };
            Guid id = book.BookId;

            _bookRepositoryMock.Setup(b => b.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            //Act
            bool isSucceed = await _bookservice.DeleteBookAsync(id);

            //Assert
            isSucceed.Should().BeTrue();

            //Verify
            _bookRepositoryMock.Verify(v=>v.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteBookAsync_ShouldThrowBadRequestException_WhenBookIdIsEmpty()
        {
            // Arrange
            Guid id = Guid.Empty;

            // Act + Assert
            BadRequestException exception =
                await Assert.ThrowsAsync<BadRequestException>(
                    () => _bookservice.DeleteBookAsync(id));

            exception.Message.Should().Be("Book id cannot be empty");
        }

        [Fact]
        public async Task DeleteBookAsync_ShouldThrowNotFoundException_WhenBookDoesNotExist()
        {
            // Arrange
            Guid id = Guid.NewGuid();

            _bookRepositoryMock
                .Setup(b => b.DeleteAsync(It.IsAny<Guid>()))
                .ReturnsAsync(false);

            // Act + Assert
            NotFoundException exception =
                await Assert.ThrowsAsync<NotFoundException>(
                    () => _bookservice.DeleteBookAsync(id));

            exception.Message.Should()
                .Be($"Book with Id = {id} not found Operation UnSucessfull...");
        }
        #endregion

        #region UpdateBook

        [Fact]
        public async Task UpdateBookAsync_ShouldThrowBadRequestException_WhenBookIsNull()
        {
            //Arrange
            BookUpdateRequest? request = null;
            Guid id = Guid.NewGuid();

            //Act + Assert
            BadRequestException exception = await Assert.ThrowsAsync<BadRequestException>(async () => 
            {
                await _bookservice.UpdateBookAsync(id, request);
            });

            exception.Message.Should().Be("request to update cannot be null");

            //Verify
            _bookRepositoryMock.Verify(v=>v.UpdateAsync(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task UpdateBookAsync_ShouldThrowBadRequestException_WhenBookIdIsEmpty()
        {
            //Arrange

            BookUpdateRequest request = new BookUpdateRequest
            {
                BookId = Guid.Empty,
                Title = "Title Updated",
                Author = "Author Updated",
                Price = 989,
                PublishedDate = DateTime.UtcNow,
            };

            //Act + Assert
            BadRequestException exception = await Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await _bookservice.UpdateBookAsync(Guid.NewGuid(), request);
            });

            exception.Message.Should().Be("Book Id cannot be empty");

            //Verify
            _bookRepositoryMock.Verify(v=>v.UpdateAsync(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task UpdateBookAsync_ShouldThrowNotFoundException_WhenBookDoesNotExist()
        {
            //Arrange
            Book book = new Book
            {
                BookId = Guid.NewGuid(),
                Title = "Test Title",
                Author = "Test Author",
                Price = 899,
                PublishedDate = DateTime.UtcNow,
            };
            BookUpdateRequest request = new BookUpdateRequest 
            {
                BookId = Guid.Parse("526650CB-C242-49FE-B61E-396621BE8F55"),
                Title = "Updated Title",
                Author = "Updated Author",
                Price = 499,
                PublishedDate = DateTime.Now
            };

            Guid id = Guid.NewGuid();
            ValidationResult validationResult = new ValidationResult();

            _updateValidatorMock.Setup(v=>v.ValidateAsync(request, default)).ReturnsAsync(validationResult);

            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Book?)null);
            //Act + Assert
            NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(async () => 
            {
                await _bookservice.UpdateBookAsync(id, request);
            });

            exception.Message.Should().Be($"No Book existing with id = {id},Please provide valid book id.");

            //Verify
            _updateValidatorMock.Verify(v => v.ValidateAsync(request, default), Times.Once);
            _bookRepositoryMock.Verify(v=>v.GetByIdAsync(id), Times.Once);

            _bookRepositoryMock.Verify(v => v.UpdateAsync(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task UpdateBookAsync_ShouldReturnUpdatedBook_WhenBookExists()
        {
            //Arrange
            Book book = new Book
            {
                BookId = Guid.Parse("426650CB-C242-49FE-B61E-396621BE8F55"),
                Title = "Test Title",
                Author = "Test Author",
                Price = 899,
                PublishedDate = DateTime.Parse("2000-01-01"),
            };
            BookUpdateRequest request = new BookUpdateRequest
            {
                BookId = book.BookId,
                Title = "Updated Title",
                Author = "Updated Author",
                Price = 499,
                PublishedDate = DateTime.Parse("1990-01-01")
            };

            Guid id = book.BookId;
            ValidationResult validationResult = new ValidationResult();

            _updateValidatorMock.Setup(v=>v.ValidateAsync(request, default)).ReturnsAsync(validationResult);

            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(book);
            _bookRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Book>())).ReturnsAsync(book);

            //Act
            BookResponse? actual = await _bookservice.UpdateBookAsync(id, request);

            //Assert
            actual.Should().NotBeNull();

            actual.Title.Should().Be(request.Title);
            actual.Author.Should().Be(request.Author);
            actual.Price.Should().Be(request.Price);
            actual.PublishedDate.Should().Be(request.PublishedDate);

            //Verify
            _updateValidatorMock.Verify(v => v.ValidateAsync(request, default), Times.Once);

            _bookRepositoryMock.Verify(v => v.UpdateAsync(It.Is<Book>(b=>
            b.Title ==request.Title && 
            b.Author ==request.Author && 
            b.Price ==request.Price && 
            b.PublishedDate == request.PublishedDate))
            , Times.Once);

            _bookRepositoryMock.Verify(v => v.GetByIdAsync(id), Times.Once);
        }
        [Fact]
        public async Task UpdateBookAsync_ShouldThrowBadRequestException_WhenValidationFails() 
        {
            //Arrange
            BookUpdateRequest request = new BookUpdateRequest
            {
                BookId = Guid.NewGuid(),
                Title = "",
                Author = "",
                Price = 899,
                PublishedDate = DateTime.Now
            };


            List<ValidationFailure> failures = new List<ValidationFailure>
            {
                new ValidationFailure("Title", "Title cannot be empty"),
                new ValidationFailure("Author", "Author cannot be empty"),
            };

            ValidationResult validationResult = new ValidationResult(failures);
            _updateValidatorMock.Setup(v=>v.ValidateAsync(request, default)).ReturnsAsync(validationResult);
            //Act + Assert
            BadRequestException exception = await Assert.ThrowsAsync<BadRequestException>(async () => 
            {
                await _bookservice.UpdateBookAsync(request.BookId, request);
            });

            exception.Message.Should().Be(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

            //Verify
            _updateValidatorMock.Verify(v => v.ValidateAsync(request, default), Times.Once);
            _bookRepositoryMock.Verify(v=>v.UpdateAsync(It.IsAny<Book>()),Times.Never);
        }
        #endregion
    }
}
