using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using BBServer;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using To_do_timer.Controllers;
using To_do_timer.Models;
using To_do_timer.Models.Book;
using To_do_timer.Services;
using Vostok.Logging.Abstractions;

namespace To_do_timer.Tests
{
    [TestFixture]
    public class ServiceStatsTests
    {
        private ManageBook _fakeManageBook;
        private ILog _fakeLogger;
        private StatisticsController _controller;
        private AnalyzerService _analyzerService;

        [SetUp]
        public void Setup()
        {
            var fakeStatusReposotory = A.Fake<IRepository<Status>>();
            var fakeBookRepository = A.Fake<IRepository<Book>>();
            var fakeEventRep = A.Fake<IRepository<Event>>();
            _analyzerService = new AnalyzerService();
            _fakeManageBook = new ManageBook(fakeBookRepository, fakeStatusReposotory, fakeEventRep);
            _fakeLogger = A.Fake<ILog>();
            _controller = new StatisticsController(_fakeManageBook, _fakeLogger, new AnalyzerService());
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void TestRanges(List<Event> events, TimeSpan expectedTime)
        {
            _analyzerService.CalculationTime(events).Minutes.Should().Be(expectedTime.Minutes);
            _analyzerService.CalculationTime(events).Hours.Should().Be(expectedTime.Hours);
        }

        public static IEnumerable<TestCaseData> TestCases()
        {
            yield return new TestCaseData(new List<Event>
            {
                OneDayEvents(10, 20, 00),
                OneDayEvents(10, 25, 00),
                OneDayEvents(10, 30, 00)
            }, new TimeSpan(0, 0, 10, 0));

            yield return new TestCaseData(new List<Event>()
            {
                OneDayEvents(11, 20, 00),
                OneDayEvents(12, 20, 00),
                OneDayEvents(13, 20, 00),
            }, new TimeSpan(0, 2, 0, 0));

            yield return new TestCaseData(new List<Event>()
            {
                OneDayEvents(11, 20, 04),
                OneDayEvents(11, 20, 44),
                OneDayEvents(11, 20, 55),
            }, new TimeSpan(0, 0, 0, 51));
            
            yield return new TestCaseData(new List<Event>()
            {
                OneDayEvents(13, 21, 21), 
                OneDayEvents(14, 20, 55), // 0, 59, 34
                OneDayEvents(16, 42, 30), // 2, 21, 26 
            }, new TimeSpan(0, 3, 21, 0));
        }


        private static Event OneDayEvents(int hour, int minute, int second)
        {
            return new Event()
            {
                Start = new DateTime(2023, 7, 6, hour, minute, second)
            };
        }
    }

    [TestFixture]
    public class UserControllerTests
    {
        private ManageBook _fakeManageBook;
        private ILog _fakeLogger;
        private UserController _controller;

        [SetUp]
        public void Setup()
        {
            var statusRepository = A.Fake<IRepository<Status>>();
            var fakeBookRepository = A.Fake<IRepository<Book>>();
            var fakeEventRep = A.Fake<IRepository<Event>>();
            _fakeManageBook = new ManageBook(fakeBookRepository, statusRepository, fakeEventRep);
            _fakeLogger = A.Fake<ILog>();
            _controller = new UserController(_fakeManageBook, _fakeLogger);
        }

        [Test]
        public void CreateBook_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var requestBook = new RequestBook
            {
                Name = "Book Name",
                Desc = "Book Description"
            };

            var userId = Guid.NewGuid().ToString();
            SetupUserClaims(userId);

            var book = new Book
            {
                Id = Guid.NewGuid(),
                UserId = new Guid(userId),
                Name = requestBook.Name,
                Desc = requestBook.Desc
            };
            AddNewBook(book);

            var result = _controller.CreateBook(requestBook).Result;
            // Assert

            _controller.ShouldEquivalentResponse(result, HttpStatusCode.OK, new Result<Book>() { Value = book });
        }

        private void AddNewBook(Book book)
        {
            Book returnValue = null;
            A.CallTo(() => _fakeManageBook.BookRepository.FirstOrDefaultAsync(A<Expression<Func<Book, bool>>>._))
                .Returns(returnValue);
            A.CallTo(() => _fakeManageBook.BookRepository.Add(A<Book>._));
            A.CallTo(() => _fakeManageBook.BookRepository.SaveChange());
            A.CallTo(() => _fakeManageBook.BookRepository.GetById(A<Guid>._))
                .Returns(book);
        }

        [Test]
        public void CreateStatus_ValidRequest_ReturnsAcceptedResult()
        {
            // Arrange
            var statusRequest = new StatusRequest
            {
                Name = "Status Name",
                Desc = "Status Description"
            };
            var userId = Guid.NewGuid().ToString();
            SetupUserClaims(userId);
            var status = new Status
            {
                Id = Guid.NewGuid(),
                UserId = new Guid(userId),
                Name = statusRequest.Name,
                Desc = statusRequest.Desc
            };
            Status returnValue = null;
            AddNewStatus(returnValue, status);

            // Act
            var result = _controller.CreateStatus(statusRequest).Result;
            // Assert

            _controller.ShouldEquivalentResponse(result, HttpStatusCode.OK, new Result<Status>()
            {
                Value = status
            });
        }

        private void AddNewStatus(Status? returnValue, Status status)
        {
            A.CallTo(() => _fakeManageBook.StatusRepository.FirstOrDefaultAsync(A<Expression<Func<Status, bool>>>._))
                .Returns(returnValue);
            A.CallTo(() => _fakeManageBook.StatusRepository.Add(A<Status>._));
            A.CallTo(() => _fakeManageBook.StatusRepository.SaveChange());
            A.CallTo(() => _fakeManageBook.StatusRepository.GetById(A<Guid>._))
                .Returns(status);
        }

        // Add more test methods for other actions...

        private void SetupUserClaims(string userId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("id", userId)
            }));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
    }
}