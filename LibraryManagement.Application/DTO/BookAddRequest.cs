using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.DTO
{
    public class BookAddRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime PublishedDate { get; set; }
    }
}
