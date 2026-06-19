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
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
