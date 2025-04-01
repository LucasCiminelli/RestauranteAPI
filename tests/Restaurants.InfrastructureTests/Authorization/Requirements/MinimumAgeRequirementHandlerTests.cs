using Xunit;
using Restaurants.Infrastructure.Authorization.Requirements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Restaurants.Application.Users;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using FluentAssertions;

namespace Restaurants.Infrastructure.Authorization.Requirements.Tests
{
    public class MinimumAgeRequirementHandlerTests
    {
        private readonly Mock<ILogger<MinimumAgeRequirementHandler>> _loggerMock;
        private readonly Mock<IUserContext> _userContextMock;
        private readonly MinimumAgeRequirementHandler _handler;

        public MinimumAgeRequirementHandlerTests()
        {

            _loggerMock = new Mock<ILogger<MinimumAgeRequirementHandler>>();
            _userContextMock = new Mock<IUserContext>();
            _handler = new MinimumAgeRequirementHandler(_loggerMock.Object, _userContextMock.Object);

        }

        [Fact()]
        public async Task HandleRequirement_MinimumAgeRequirement_ShouldSucceed()
        {

            var requirement = new MinimumAgeRequirement(18);
            var currentUser = new CurrentUser("1", "test@email.com", new[] { "Admin" }, null, new DateOnly(1995, 12, 18));


            _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

            var authorizationContext = new AuthorizationHandlerContext(
                new[] { requirement },
                null!,
                null
            );


            await _handler.HandleAsync( authorizationContext );

            authorizationContext.HasSucceeded.Should().BeTrue();

        }

        [Fact()]
        public async Task HandleRequirement_MinimumAgeRequirement_ShouldFail()
        {

            var requirement = new MinimumAgeRequirement(18);
            var currentUser = new CurrentUser("1", "test@email.com", new[] { "Admin" }, null, new DateOnly(2015, 12, 18));


            _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

            var authorizationContext = new AuthorizationHandlerContext(
                new[] { requirement },
                null!,
                null
            );


            await _handler.HandleAsync(authorizationContext);

            authorizationContext.HasSucceeded.Should().BeFalse();

        }
    }
}