using Xunit;
using Restaurants.Application.Dishes.Commands.UpdateDishById;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Restaurants.Domain.Respositories;
using Restaurants.Domain.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Constants;
using FluentAssertions;
using Restaurants.Domain.Exceptions;

namespace Restaurants.Application.Dishes.Commands.UpdateDishById.Tests
{
    public class UpdateDishByIdCommandHandlerTests
    {

        private readonly Mock<IRestaurantRepository> _restaurantRepositoryMock;
        private readonly Mock<IDishRepository> _dishRepositoryMock;
        private readonly Mock<IRestaurantAuthorizationService> _restaurantAuthorizationServiceMock;
        private readonly Mock<ILogger<UpdateDishByIdCommandHandler>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UpdateDishByIdCommandHandler _handler;


        public UpdateDishByIdCommandHandlerTests()
        {

            _loggerMock = new Mock<ILogger<UpdateDishByIdCommandHandler>>();
            _mapperMock = new Mock<IMapper>();
            _dishRepositoryMock = new Mock<IDishRepository>();
            _restaurantRepositoryMock = new Mock<IRestaurantRepository>();
            _restaurantAuthorizationServiceMock = new Mock<IRestaurantAuthorizationService>();
            _handler = new UpdateDishByIdCommandHandler
                (
                    _restaurantRepositoryMock.Object,
                    _dishRepositoryMock.Object,
                    _mapperMock.Object,
                    _loggerMock.Object,
                    _restaurantAuthorizationServiceMock.Object
                );

        }


        [Fact()]
        public async Task Handle_WithValidRequest_ShouldUpdateDish()
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

            var updateCommand = new UpdateDishByIdCommand
            {
                RestaurantId = restaurantId,
                DishId = 1,
                Name = "Changed",
                Description = "Changed",
                KiloCalories = 150,
                Price = 15.50M,
            };


            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);
            _restaurantAuthorizationServiceMock.Setup(a => a.Authorize(restaurant, ResourceOperation.Update)).Returns(true);


            //act 

            await _handler.Handle(updateCommand, CancellationToken.None);


            //assert

            _dishRepositoryMock.Verify(d => d.Update(dish));
            _mapperMock.Verify(m => m.Map(updateCommand, dish), Times.Once);
        }

        [Fact()]
        public async Task Handle_WithNonExistingRestaurant_ShouldThrowNotFoundException()
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

            var updateCommand = new UpdateDishByIdCommand
            {
                RestaurantId = restaurantId,
                DishId = 1,
                Name = "Changed",
                Description = "Changed",
                KiloCalories = 150,
                Price = 15.50M,
            };


            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync((Restaurant?)null);


            //act 

            Func<Task> act = async () => await _handler.Handle(updateCommand, CancellationToken.None);


            //assert

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact()]
        public async Task Handle_WithUnauthorizedUser_ShouldThrowForbidException()
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

            var updateCommand = new UpdateDishByIdCommand
            {
                RestaurantId = restaurantId,
                DishId = 1,
                Name = "Changed",
                Description = "Changed",
                KiloCalories = 150,
                Price = 15.50M,
            };


            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);
            _restaurantAuthorizationServiceMock.Setup(a => a.Authorize(restaurant, ResourceOperation.Update)).Returns(false);


            //act 

            Func<Task> act = async () => await _handler.Handle(updateCommand, CancellationToken.None);

            
            //assert

            await act.Should().ThrowAsync<ForbidException>();
        }

        [Fact()]
        public async Task Handle_WithNonExistingDishForRestaurant_ShouldThrowNotFoundException()
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

            var updateCommand = new UpdateDishByIdCommand
            {
                RestaurantId = restaurantId,
                DishId = 2,
                Name = "Changed",
                Description = "Changed",
                KiloCalories = 150,
                Price = 15.50M,
            };


            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);


            //act 

            Func<Task> act = async () => await _handler.Handle(updateCommand, CancellationToken.None);


            //assert

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact()]
        public async Task Handle_WithNonExistingDishForRestaurantById_ShouldThrowNotFoundException()
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

            var updateCommand = new UpdateDishByIdCommand
            {
                RestaurantId = 2,
                DishId = 1,
                Name = "Changed",
                Description = "Changed",
                KiloCalories = 150,
                Price = 15.50M,
            };


            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);


            //act 

            Func<Task> act = async () => await _handler.Handle(updateCommand, CancellationToken.None);


            //assert

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact()]
        public async Task Handle_WithNonExistingDishesForRestaurantById_ShouldThrowException()
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

            var updateCommand = new UpdateDishByIdCommand
            {
                RestaurantId = 2,
                DishId = 1,
                Name = "Changed",
                Description = "Changed",
                KiloCalories = 150,
                Price = 15.50M,
            };


            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);


            //act 

            Func<Task> act = async () => await _handler.Handle(updateCommand, CancellationToken.None);


            //assert

            await act.Should().ThrowAsync<Exception>();
        }
    }
}