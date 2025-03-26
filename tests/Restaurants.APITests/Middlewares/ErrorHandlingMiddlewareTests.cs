using Xunit;
using Restaurants.API.Middlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Entities;
using FluentAssertions;

namespace Restaurants.API.Middlewares.Tests
{
    public class ErrorHandlingMiddlewareTests
    {
        [Fact()]
        public async Task InvokeAsync_WhenNoExceptionThrown_ShouldCallNextDelegate()
        {
            //arrange

            var loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
            var middleware = new ErrorHandlingMiddleware(loggerMock.Object);
            var context = new DefaultHttpContext();
            var nextDelegateMock = new Mock<RequestDelegate>();


            //act

            await middleware.InvokeAsync(context, nextDelegateMock.Object);


            //assert

            nextDelegateMock.Verify(m => m.Invoke(context), Times.Once);

        }

        [Fact()]
        public async Task InvokeAsync_WhenNotFoundExceptionThrown_ShouldSetStatusCode404()
        {

            //arrange

            var loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
            var middleware = new ErrorHandlingMiddleware(loggerMock.Object);
            var context = new DefaultHttpContext();
            var notFoundException = new NotFoundException(nameof(Restaurant), "1");

            //act

            await middleware.InvokeAsync(context, _ => throw notFoundException);


            //assert

            context.Response.StatusCode.Should().Be(404);

        }

        [Fact()]
        public async Task InvokeAsync_WhenForbidExceptionThrown_ShouldSetStatusCode403()
        {

            //arrange

            var loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
            var middleware = new ErrorHandlingMiddleware(loggerMock.Object);
            var context = new DefaultHttpContext();
            var forbidException = new ForbidException();

            //act

            await middleware.InvokeAsync(context, _ => throw forbidException);


            //assert

            context.Response.StatusCode.Should().Be(403);

        }

        [Fact()]
        public async Task InvokeAsync_WhenGenericExceptionThrown_ShouldSetStatusCode500()
        {

            //arrange

            var loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
            var middleware = new ErrorHandlingMiddleware(loggerMock.Object);
            var context = new DefaultHttpContext();
            var exception = new Exception();

            //act

            await middleware.InvokeAsync(context, _ => throw exception);


            //assert

            context.Response.StatusCode.Should().Be(500);

        }
    }
}