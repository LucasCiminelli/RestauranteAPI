using Xunit;
using Restaurants.Application.Users.Commands.UpdateUserDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Restaurants.Domain.Entities;
using FluentAssertions;
using Restaurants.Domain.Exceptions;

namespace Restaurants.Application.Users.Commands.UpdateUserDetails.Tests
{
    public class UpdateUserDetailsCommandHandlerTests
    {

        private readonly Mock<ILogger<UpdateUserDetailsCommandHandler>> _loggerMock;
        private readonly Mock<IUserContext> _userContextMock;
        private readonly Mock<IUserStore<User>> _userStoreMock;
        private readonly UpdateUserDetailsCommandHandler _handler;

        public UpdateUserDetailsCommandHandlerTests()
        {
            _loggerMock = new Mock<ILogger<UpdateUserDetailsCommandHandler>>();
            _userContextMock = new Mock<IUserContext>();
            _userStoreMock = new Mock<IUserStore<User>>();

            _handler = new UpdateUserDetailsCommandHandler(_loggerMock.Object, _userContextMock.Object,_userStoreMock.Object);
        }


        [Fact()]
        public async Task Handle_ForValidCommand_ReturnUpdatedUserDetails()
        {
            var userId = "123";
            var user = new CurrentUser(userId, "testEmail@test.com", new[] { "User" }, null, null);

            var dbUser = new User
            {
                Id = userId,
                Nationality = "Argentina",
                DateOfBirth = new DateOnly(1995, 12, 18)
            };

            var command = new UpdateUserDetailsCommand()
            {
                Nationality = "Brazil",
                DateOfBirth = new DateOnly(1990, 12, 18)
            };

            _userContextMock.Setup(u => u.GetCurrentUser()).Returns(user);
            _userStoreMock.Setup(u => u.FindByIdAsync(userId,CancellationToken.None)).ReturnsAsync(dbUser);
            _userStoreMock.Setup(u => u.UpdateAsync(dbUser, CancellationToken.None));


             await _handler.Handle(command, CancellationToken.None);


            _userStoreMock.Verify(u => u.UpdateAsync(It.IsAny<User>(), CancellationToken.None),Times.Once);
                    
        }

        [Fact()]
        public async Task Handle_ForNonExistingUser_ReturnNotFoundException()
        {
            var userId = "123";
            var user = new CurrentUser(userId, "testEmail@test.com", new[] { "User" }, null, null);

            var dbUser = new User
            {
                Id = userId,
                Nationality = "Argentina",
                DateOfBirth = new DateOnly(1995, 12, 18)
            };

            var command = new UpdateUserDetailsCommand()
            {
                Nationality = "Brazil",
                DateOfBirth = new DateOnly(1990, 12, 18)
            };

            _userContextMock.Setup(u => u.GetCurrentUser()).Returns(user);
            _userStoreMock.Setup(u => u.FindByIdAsync(userId, CancellationToken.None)).ReturnsAsync((User?)null);


            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);


            await act.Should().ThrowAsync<NotFoundException>();

        }
    }
}