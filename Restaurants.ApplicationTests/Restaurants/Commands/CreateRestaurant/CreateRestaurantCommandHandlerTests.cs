using Xunit;
using Restaurants.Application.Restaurants.Commands.CreateRestaurant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Respositories;
using Restaurants.Application.Users;
using FluentAssertions;

namespace Restaurants.Application.Restaurants.Commands.CreateRestaurant.Tests
{
    public class CreateRestaurantCommandHandlerTests
    {
        [Fact()]
        public async Task Handle_ForValidCommand_ReturnsCreateRestaurantId()
        {

            //arrange

            var loggerMock = new Mock<ILogger<CreateRestaurantCommandHandler>>();
            var mapperMock = new Mock<IMapper>();
            var restaurantRepositoryMock = new Mock<IRestaurantRepository>();
            var userContextMock = new Mock<IUserContext>();

            var command = new CreateRestaurantCommand();
            var restaurant = new Restaurant();
            var currentUser = new CurrentUser("owner-id", "test@test.com", [], null, null);


            mapperMock.Setup(m => m.Map<Restaurant>(command)).Returns(restaurant);
            restaurantRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Restaurant>())).ReturnsAsync(1);
            userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);


            var commandHandler = new CreateRestaurantCommandHandler
            (
                loggerMock.Object,
                mapperMock.Object,
                restaurantRepositoryMock.Object,
                userContextMock.Object
            );



            //act

            var result = await commandHandler.Handle(command,CancellationToken.None);



            //assert

            result.Should().Be(1);
            restaurant.OwnerId.Should().Be("owner-id");
            restaurantRepositoryMock.Verify(r => r.CreateAsync(restaurant), Times.Once);

        }
    }   
}