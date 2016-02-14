using System.Collections.Generic;
using System.Linq;
using BookService.Models;

namespace MyWebApi.Repository
{
    public class MyRepository : IRepository
    {
        private readonly MyContext _context;

        public MyRepository()
        {
            _context = new MyContext();
        }

        public Author GetAuthor(int i)
        {
            return _context.Authors.SingleOrDefault(a => a.Id == i);
        }

        public IEnumerable<Book> GetBooksByAuthor(int i)
        {
            return _context.Books.Where(b => b.AuthorId == i);
        }
    }

    public interface IRepository
    {
        Author GetAuthor(int i);
        IEnumerable<Book> GetBooksByAuthor(int i);
    }
}