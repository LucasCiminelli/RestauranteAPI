using Xunit;
using Restaurants.Infrastructure.Authorization.Requirements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurants.Application.Users;
using Moq;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Respositories;
using Microsoft.AspNetCore.Authorization;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Restaurants.Infrastructure.Authorization.Requirements.Tests
{
    public class AtLeastTwoRestaurantsRequirementHandlerTests
    {
        [Fact()]
        public async Task HandleRequirementAsync_UserHasCreateAtLeastTwoRestaurants_ShouldSucceed()
        {

            //arrange
            var currentUser = new CurrentUser("1", "test@test.com", [], null, null);
            var userMockContext = new Mock<IUserContext>();
            var loggerMock = new Mock<ILogger<AtLeastTwoRestaurantsRequirementHandler>>();
            userMockContext.Setup(c => c.GetCurrentUser()).Returns(currentUser);

            var restaurants = new List<Restaurant>
            {
                new()
                {
                    OwnerId = currentUser.Id
                },
                 new()
                {
                    OwnerId = currentUser.Id
                },
                  new()
                {
                    OwnerId = "2"
                }
            };

            var restaurantRepositoyMock = new Mock<IRestaurantRepository>();
            restaurantRepositoyMock.Setup(r => r.GetAllAsync()).ReturnsAsync(restaurants);


            var requirement = new AtLeastTwoRestaurantsRequirement(2);
            var handler = new AtLeastTwoRestaurantsRequirementHandler(loggerMock.Object, userMockContext.Object, restaurantRepositoyMock.Object);

            var context = new AuthorizationHandlerContext([requirement], null!, null);

            // act

            await handler.HandleAsync(context);


            //assert

            context.HasSucceeded.Should().BeTrue();



        }

        [Fact()]
        public async Task HandleRequirementAsync_UserHasNotCreatedAtLeastTwoRestaurants_ShouldFail()
        {

            //arrange
            var currentUser = new CurrentUser("1", "test@test.com", [], null, null);
            var userMockContext = new Mock<IUserContext>();
            var loggerMock = new Mock<ILogger<AtLeastTwoRestaurantsRequirementHandler>>();
            userMockContext.Setup(c => c.GetCurrentUser()).Returns(currentUser);

            var restaurants = new List<Restaurant>
            {
                new()
                {
                    OwnerId = currentUser.Id
                },
                  new()
                {
                    OwnerId = "2"
                }
            };

            var restaurantRepositoyMock = new Mock<IRestaurantRepository>();
            restaurantRepositoyMock.Setup(r => r.GetAllAsync()).ReturnsAsync(restaurants);


            var requirement = new AtLeastTwoRestaurantsRequirement(2);
            var handler = new AtLeastTwoRestaurantsRequirementHandler(loggerMock.Object, userMockContext.Object, restaurantRepositoyMock.Object);

            var context = new AuthorizationHandlerContext([requirement], null!, null);

            // act

            await handler.HandleAsync(context);


            //assert

            context.HasSucceeded.Should().BeFalse();

        }
    }
}