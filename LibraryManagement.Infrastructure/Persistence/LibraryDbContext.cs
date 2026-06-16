using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Infrastructure.Persistence
{
    public class LibraryDbContext :DbContext
    {
        //Intialializing base contructor
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options): base(options)
        { 
        }

        //Backup contructor
        public LibraryDbContext() 
        {
        }
        
        public DbSet<Book> Books { get; set; }
    }
}
