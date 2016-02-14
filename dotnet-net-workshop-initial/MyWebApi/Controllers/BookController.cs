using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BookService.Models;
using MyWebApi.Repository;

namespace MyWebApi.Controllers
{
    public class BookController : ApiController
    {

        public IEnumerable<Book> GetAllProducts()
        {
            var db = new MyContext();
            return db.Books.Take(100);
        }



    }
}
