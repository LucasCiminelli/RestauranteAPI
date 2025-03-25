using Xunit;
using Restaurants.Application.Dishes.Commands.DeleteAllDishesByRestaurantId;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Respositories;
using Restaurants.Domain.Interfaces;
using Restaurants.Application.Restaurants.Commands.CreateRestaurant;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Constants;
using FluentAssertions;
using Restaurants.Domain.Exceptions;

namespace Restaurants.Application.Dishes.Commands.DeleteAllDishesByRestaurantId.Tests
{
    public class DeleteAllDishesByRestaurantIdCommandHandlerTests
    {

        private readonly Mock<ILogger<DeleteAllDishesByRestaurantIdCommandHandler>> _loggerMock;
        private readonly Mock<IRestaurantRepository> _restaurantRepositoryMock;
        private readonly Mock<IDishRepository> _dishRepositoryMock;
        private readonly Mock<IRestaurantAuthorizationService> _restaurantAuthorizationServiceMock;
        private readonly DeleteAllDishesByRestaurantIdCommandHandler _handler;

        public DeleteAllDishesByRestaurantIdCommandHandlerTests()
        {

            _loggerMock = new Mock<ILogger<DeleteAllDishesByRestaurantIdCommandHandler>>();
            _restaurantRepositoryMock = new Mock<IRestaurantRepository>();
            _dishRepositoryMock = new Mock<IDishRepository>();
            _restaurantAuthorizationServiceMock = new Mock<IRestaurantAuthorizationService>();

            _handler = new DeleteAllDishesByRestaurantIdCommandHandler
                (
                    _loggerMock.Object,
                    _restaurantRepositoryMock.Object,
                    _dishRepositoryMock.Object,
                    _restaurantAuthorizationServiceMock.Object
                );
        }

        [Fact()]
        public async Task Handle_WithValidRequest_ShouldDeleteAllDishesForRestaurant()
        {

            //arrange

            var restaurantId = 1;

            var restaurant = new Restaurant()
            {
                Id = restaurantId,
                Dishes = new List<Dish>() { new Dish() { Id = 1, Name = "Pizza" } }
            };

            var command = new DeleteAllDishesByRestaurantIdCommand(restaurantId);


            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);
            _restaurantAuthorizationServiceMock.Setup(a => a.Authorize(restaurant, ResourceOperation.Delete)).Returns(true);


            //act

            await _handler.Handle(command, CancellationToken.None);


            //assert

            _dishRepositoryMock.Verify(d => d.RemoveAllDishes(restaurant.Dishes!), Times.Once());

        }

        [Fact()]
        public async Task Handle_WithNonExistingRestaurant_ShouldThrowNotFoundException()
        {

            //arrange

            var restaurantId = 1;

            var restaurant = new Restaurant()
            {
                Id = restaurantId,
                Dishes = new List<Dish>() { new Dish() { Id = 1, Name = "Pizza" } }
            };

            var command = new DeleteAllDishesByRestaurantIdCommand(restaurantId);

            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync((Restaurant?)null);


            //act

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);


            //assert

            await act.Should().ThrowAsync<NotFoundException>();
        }


        [Fact()]
        public async Task Handle_WithUnauthorizedUser_ShouldThrowForbidException()
        {

            //arrange

            var restaurantId = 1;

            var restaurant = new Restaurant()
            {
                Id = restaurantId,
                Dishes = new List<Dish>() { new Dish() { Id = 1, Name = "Pizza" } }
            };

            var command = new DeleteAllDishesByRestaurantIdCommand(restaurantId);

            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);
            _restaurantAuthorizationServiceMock.Setup(a => a.Authorize(restaurant, ResourceOperation.Delete)).Returns(false);


            //act

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);


            //assert

            await act.Should().ThrowAsync<ForbidException>();


        }
    }
}