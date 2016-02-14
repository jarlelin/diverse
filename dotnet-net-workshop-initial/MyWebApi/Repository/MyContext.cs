using System.Data.Entity;
using BookService.Models;

namespace MyWebApi.Repository
{
    internal class MyContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
    }
}