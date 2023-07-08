using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using BBServer;
using BBServer.Extensions;
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
        private ParserStatsService _parserStatsService;

        [SetUp]
        public void Setup()
        {
            var fakeStatusReposotory = A.Fake<IRepository<Status>>();
            var fakeBookRepository = A.Fake<IRepository<Book>>();
            var fakeEventRep = A.Fake<IRepository<Event>>();
            _parserStatsService = new ParserStatsService();
            _fakeManageBook = new ManageBook(fakeBookRepository, fakeStatusReposotory, fakeEventRep);
            _fakeLogger = A.Fake<ILog>();
            _controller = new StatisticsController(_fakeManageBook, _fakeLogger, new ParserStatsService());
        }

        [Test]
        [TestCaseSource(nameof(TestDetailActiveTime))]
        public void TestWithStatuses(List<Event> events, List<ResponseEventStats> expectedResponse)
        {

            var responseEventsStats = _parserStatsService.GetActiveTimeWithStatus(events);
            responseEventsStats.Should().BeEquivalentTo(expectedResponse);
            
        }

        public static IEnumerable<TestCaseData> TestDetailActiveTime()
        {
            yield return new TestCaseData(new List<Event>()
            {
                OneDayDefaultEvents(10,20,00, DataMemory.ExampleStatusCSharp),
                OneDayDefaultEvents(10,25,00, DataMemory.ExampleStatusTypeScript),
                OneDayDefaultEvents(10,30,00, DataMemory.ExampleStatusChill)
            }, new List<ResponseEventStats>()
            {
                new ResponseEventStats()
                {
                    Status = DataMemory.ExampleStatusCSharp.ToResponse(), Time = new TimeSpan(0,5,0)
                },
                new ResponseEventStats()
                {
                    Status = DataMemory.ExampleStatusTypeScript.ToResponse(), Time = new TimeSpan(0,5,0)
                },
                new ResponseEventStats()
                {
                    Status = DataMemory.ExampleStatusChill.ToResponse(), Time = new TimeSpan(0,5,0)
                }
            });
        }
        
        [Test]
        [TestCaseSource(nameof(TestActiveTime))]
        public void TestCalculate(List<Event> events, TimeSpan expectedTime)
        {
            var time = _parserStatsService.GetActiveTime(events);
            time.Should().Be(expectedTime);
        }

        public static IEnumerable<TestCaseData> TestActiveTime()
        {
            yield return new TestCaseData(new List<Event>
            {
                OneDayDefaultEvents(10, 20, 00),
                OneDayDefaultEvents(10, 25, 00),
                OneDayDefaultEvents(10, 30, 00)
            }, new TimeSpan(0, 0, 10, 0)).SetName("only minutes");

            yield return new TestCaseData(new List<Event>()
            {
                OneDayDefaultEvents(11, 20, 00),
                OneDayDefaultEvents(12, 20, 00),
                OneDayDefaultEvents(13, 20, 00),
            }, new TimeSpan(0, 2, 0, 0)).SetName("only hours");

            yield return new TestCaseData(new List<Event>()
            {
                OneDayDefaultEvents(11, 20, 04),
                OneDayDefaultEvents(11, 20, 44),
                OneDayDefaultEvents(11, 20, 55),
            }, new TimeSpan(0, 0, 0, 51)).SetName("only seconds");

            yield return new TestCaseData(new List<Event>()
            {
                OneDayDefaultEvents(13, 21, 21),
                OneDayDefaultEvents(14, 20, 55), // 0, 59, 34
                OneDayDefaultEvents(16, 42, 30), // 2, 21, 26 
            }, new TimeSpan(0, 3, 21, 9)).SetName("All");
        }


        private static Event OneDayDefaultEvents(int hour, int minute, int second)
        {
            return new Event()
            {
                Start = new DateTime(2023, 7, 6, hour, minute, second)
            };
        }
        
        private static Event OneDayDefaultEvents(int hour, int minute, int second, Status status)
        {
            return new Event()
            {
                StatusId = status.Id,
                Status = status,
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