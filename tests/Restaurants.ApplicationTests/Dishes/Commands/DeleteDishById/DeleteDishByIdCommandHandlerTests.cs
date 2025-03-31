using Xunit;
using Restaurants.Application.Dishes.Commands.DeleteDishById;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Restaurants.Application.Dishes.Commands.UpdateDishById;
using Restaurants.Domain.Interfaces;
using Restaurants.Domain.Respositories;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using FluentAssertions;
using Restaurants.Domain.Exceptions;

namespace Restaurants.Application.Dishes.Commands.DeleteDishById.Tests
{
    public class DeleteDishByIdCommandHandlerTests
    {
        private readonly Mock<IRestaurantRepository> _restaurantRepositoryMock;
        private readonly Mock<IDishRepository> _dishRepositoryMock;
        private readonly Mock<IRestaurantAuthorizationService> _restaurantAuthorizationServiceMock;
        private readonly Mock<ILogger<DeleteDishByIdCommandHandler>> _loggerMock;
        private readonly DeleteDishByIdCommandHandler _handler;

        public DeleteDishByIdCommandHandlerTests()
        {
            _loggerMock = new Mock<ILogger<DeleteDishByIdCommandHandler>>();
            _dishRepositoryMock = new Mock<IDishRepository>();
            _restaurantRepositoryMock = new Mock<IRestaurantRepository>();
            _restaurantAuthorizationServiceMock = new Mock<IRestaurantAuthorizationService>();
            _handler = new DeleteDishByIdCommandHandler
                (
                    _restaurantRepositoryMock.Object,
                    _dishRepositoryMock.Object,
                    _loggerMock.Object,
                    _restaurantAuthorizationServiceMock.Object
                );
        }

        [Fact()]
        public async Task Handle_WithValidRequest_ShouldDeleteDish()
        {

            //arrange

            var restaurantId = 1;

            var dish = new Dish
            {
                Id = 1,
                Name = "Pizza",
                Description = "Test",
                KiloCalories = 100,
                Price = 12.50M,
                RestaurantId = restaurantId
            };

            var restaurant = new Restaurant()
            {
                Id = restaurantId,
                Dishes = new List<Dish>()
                {
                    dish
                }
            };

            var deleteCommand = new DeleteDishByIdCommand(restaurantId,dish.Id);


            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);
            _restaurantAuthorizationServiceMock.Setup(a => a.Authorize(restaurant, ResourceOperation.Delete)).Returns(true);


            //act 

            await _handler.Handle(deleteCommand, CancellationToken.None);


            //assert

            _dishRepositoryMock.Verify(d => d.Delete(dish), Times.Once);

        }

        [Fact()]
        public async Task Handle_WithNonExistingRestaurant_ShouldReturnNotFoundException()
        {

            //arrange

            var restaurantId = 1;

            var dish = new Dish
            {
                Id = 1,
                Name = "Pizza",
                Description = "Test",
                KiloCalories = 100,
                Price = 12.50M,
                RestaurantId = restaurantId
            };

            var restaurant = new Restaurant()
            {
                Id = restaurantId,
                Dishes = new List<Dish>()
                {
                    dish
                }
            };

            var deleteCommand = new DeleteDishByIdCommand(restaurantId, dish.Id);


            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync((Restaurant?)null);


            //act 

            Func<Task> act = async () => await _handler.Handle(deleteCommand, CancellationToken.None);


            //assert

            await act.Should().ThrowAsync<NotFoundException>();

        }

        [Fact()]
        public async Task Handle_WithNonExistingDishes_ShouldReturnException()
        {

            //arrange

            var restaurantId = 1;

            var dish = new Dish
            {
                Id = 1,
                Name = "Pizza",
                Description = "Test",
                KiloCalories = 100,
                Price = 12.50M,
                RestaurantId = restaurantId
            };

            var restaurant = new Restaurant()
            {
                Id = restaurantId,
            };

            var deleteCommand = new DeleteDishByIdCommand(restaurantId, dish.Id);


            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);


            //act 

            Func<Task> act = async () => await _handler.Handle(deleteCommand, CancellationToken.None);


            //assert

            await act.Should().ThrowAsync<Exception>();

        }

        [Fact()]
        public async Task Handle_WithNonExistingDishForRestaurant_ShouldReturnNotFoundException()
        {

            //arrange

            var restaurantId = 1;

            var dish = new Dish
            {
                Id = 1,
                Name = "Pizza",
                Description = "Test",
                KiloCalories = 100,
                Price = 12.50M,
                RestaurantId = restaurantId
            };

            var restaurant = new Restaurant()
            {
                Id = restaurantId,
                Dishes = new List<Dish>()
                {
                    dish
                }
            };

            var deleteCommand = new DeleteDishByIdCommand(restaurantId, 2);


            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);


            //act 

            Func<Task> act = async () => await _handler.Handle(deleteCommand, CancellationToken.None);


            //assert

            await act.Should().ThrowAsync<NotFoundException>();

        }

        [Fact()]
        public async Task Handle_WithUnauthorizedUser_ShouldReturnForbidException()
        {

            //arrange

            var restaurantId = 1;

            var dish = new Dish
            {
                Id = 1,
                Name = "Pizza",
                Description = "Test",
                KiloCalories = 100,
                Price = 12.50M,
                RestaurantId = restaurantId
            };

            var restaurant = new Restaurant()
            {
                Id = restaurantId,
                Dishes = new List<Dish>()
                {
                    dish
                }
            };

            var deleteCommand = new DeleteDishByIdCommand(restaurantId, 1);


            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);
            _restaurantAuthorizationServiceMock.Setup(a => a.Authorize(restaurant, ResourceOperation.Delete)).Returns(false);


            //act 

            Func<Task> act = async () => await _handler.Handle(deleteCommand, CancellationToken.None);


            //assert

            await act.Should().ThrowAsync<ForbidException>();

        }
    }
}