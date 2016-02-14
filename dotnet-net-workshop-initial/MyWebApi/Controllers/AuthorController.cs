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
    public class AuthorController : ApiController
    {
        private readonly IMyLogger _logger;
        private readonly IRepository _repository;
        private readonly IGoogleBookApi _googleBookApi;

        public AuthorController(IMyLogger logger, IRepository repository, IGoogleBookApi _googleBookApi)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            _logger = logger;
            _repository = repository;
            this._googleBookApi = _googleBookApi;
        }


        public Author GetAuthor(int id)
        {
            _logger.Debug($"Getauthor called for id {id}");
            var author = _repository.GetAuthor(id);
            if(author== null)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));

            if (author.RestrictedAuthor) { _logger.Warn($"Author {author.Name} accessed."); }

            return author;
        }



        public Author GetAuthorDetailed(int i)
        {
            var author = GetAuthor(i);
            var books = _repository.GetBooksByAuthor(i);

            var avg = GetAverageBookScoreForAuthor(i, books);
            author.AverageBookScore = avg;

            return author;

        }

        private double GetAverageBookScoreForAuthor(int authorId, IEnumerable<Book> books)
        {
            _logger.Debug("henter og regner ut gjennomsnittscore for bøkene til forfatter");
            double scoreSum = 0;
            int count = books.Count();
            foreach (var book in books)
            {
                try
                {
                    scoreSum += _googleBookApi.GetBookScore(book.GoogleBookId);
                }
                catch (HttpResponseException e)
                {
                    _logger.Warn("google api svarte med feil");
                    count--;
                }
            }
            var avg = scoreSum/count;
            return avg;
        }
    }
}
