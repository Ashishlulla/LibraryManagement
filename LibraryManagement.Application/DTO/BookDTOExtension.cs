using LibraryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace LibraryManagement.Application.DTO
{
    public static class BookDTOExtension
    {
        public static Book ToBook(this BookAddRequest bookAddRequest) 
        {
            return new Book
            {
               Title = bookAddRequest.Title,
               Author = bookAddRequest.Author,
               Price = bookAddRequest.Price,
               PublishedDate = bookAddRequest.PublishedDate,
            };
        }

        public static Book ToBook(this BookUpdateRequest bookUpdateRequest)
        {
            return new Book
            {
                Title = bookUpdateRequest.Title,
                Author = bookUpdateRequest.Author,
                Price = bookUpdateRequest.Price,
                PublishedDate = bookUpdateRequest.PublishedDate,
            };
        }

        public static BookResponse ToBookResponse(this Book book) 
        {
            return new BookResponse
            {
                BookId = book.BookId,
                Title = book.Title,
                Author = book.Author,
                Price = book.Price,
                PublishedDate = book.PublishedDate
            };
          
        }
    }
}
