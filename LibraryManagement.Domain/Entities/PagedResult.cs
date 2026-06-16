using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Domain.Entities
{
    public class PagedResult<T>
    {
        public List<T> items { get; set; } = new List<T>();
        public int PageNumber { get; set; } = 0;
        public int PageSize { get; set; } = 0;
        public int TotalCount { get; set; }
        public int  TotalPages { get; set; }
    }
}
