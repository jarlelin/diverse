using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BookService.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyWebApi.Controllers;
using MyWebApi.Repository;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace MyWebApi.Tests
{
    [TestFixture]
    public class AuthorControllerTests
    {
        private Mock<IMyLogger> _logMock;
        private Mock<IRepository> _repoMock;
        private AuthorController _controller;
        private Mock<IGoogleBookApi> _googleMock;

        [SetUp]
        public void Initialize()
        {
            _logMock = new Mock<IMyLogger>();
            _repoMock = new Mock<IRepository>();
            _googleMock = new Mock<IGoogleBookApi>();
            _controller = new AuthorController(_logMock.Object, _repoMock.Object, _googleMock.Object);
        }


        [Test]
        public void GetAuthorReturnsNoAuthorThrows404()
        {
            try
            {
                _controller.GetAuthor(-1);
                Assert.Fail();
            }
            catch (HttpResponseException e)
            {
                Assert.AreEqual(HttpStatusCode.NotFound, e.Response.StatusCode);
            }
        }

        [Test]
        public void GetRestrictedAuthorAttemptIsLogged()
        {
            _repoMock.Setup(r => r.GetAuthor(1)).Returns(new Author(){RestrictedAuthor = true});

            var res = _controller.GetAuthor(1);

            Assert.AreEqual(true, res.RestrictedAuthor);
            _logMock.Verify(l=>l.Warn(It.IsAny<string>()));
        }


        [Test]
        public void GetDetailedAuthorReturnsAveragBookScore()
        {
            _repoMock.Setup(r => r.GetAuthor(1)).Returns(new Author() );
            _repoMock.Setup(r => r.GetBooksByAuthor(1)).Returns(new List<Book>() { {new Book()}, {new Book()}, {new Book()} });
            _googleMock.SetupSequence(g => g.GetBookScore(It.IsAny<int>()))
                .Returns(4)
                .Returns(3)
                .Returns(2);

            var res = _controller.GetAuthorDetailed(1);
            Assert.AreEqual(3, res.AverageBookScore);


            _logMock.Verify(l => l.Warn(It.IsAny<string>()),Times.Never);

        }


        [Test]
        public void GetDetailedAuthorReturnsAveragBookScore_ExcludesBooksNoutFound()
        {
            _repoMock.Setup(r => r.GetAuthor(2)).Returns(new Author());
            _repoMock.Setup(r => r.GetBooksByAuthor(2)).Returns(new List<Book>() { { new Book() }, { new Book() }, { new Book() } });
            _googleMock.SetupSequence(g => g.GetBookScore(It.IsAny<int>()))
                .Returns(4)
                .Returns(3)
                .Throws(new HttpResponseException(new HttpResponseMessage(statusCode: HttpStatusCode.NotFound)));



            var res = _controller.GetAuthorDetailed(2);
            Assert.AreEqual(3.5, res.AverageBookScore);
        }

    }



    public class TestLogger : IMyLogger
    {
        public void Debug(string message)
        {
            Console.WriteLine(message   );
        }

        public void Error(string message)
        {
            Console.WriteLine(message);
        }

        public void Warn(string message)
        {
            Console.WriteLine(message);
        }
    }
}