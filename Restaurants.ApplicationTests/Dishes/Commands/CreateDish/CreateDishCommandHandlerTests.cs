using Xunit;
using Restaurants.Application.Dishes.Commands.CreateDish;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Restaurants.Domain.Respositories;
using Restaurants.Domain.Interfaces;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Constants;
using FluentAssertions;
using Restaurants.Domain.Exceptions;

namespace Restaurants.Application.Dishes.Commands.CreateDish.Tests
{
    public class CreateDishCommandHandlerTests
    {

        private readonly Mock<ILogger<CreateDishCommandHandler>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRestaurantRepository> _restaurantRepositoryMock;
        private readonly Mock<IRestaurantAuthorizationService> _authorizationServiceMock;
        private readonly Mock<IDishRepository> _dishRepository;
        private readonly CreateDishCommandHandler _handler;

        public CreateDishCommandHandlerTests()
        {

            _loggerMock = new Mock<ILogger<CreateDishCommandHandler>>();
            _mapperMock = new Mock<IMapper>();
            _restaurantRepositoryMock = new Mock<IRestaurantRepository>();
            _dishRepository = new Mock<IDishRepository>();
            _authorizationServiceMock = new Mock<IRestaurantAuthorizationService>();
            _handler = new CreateDishCommandHandler
                (
                     _loggerMock.Object,
                     _restaurantRepositoryMock.Object,
                     _dishRepository.Object,
                     _mapperMock.Object,
                     _authorizationServiceMock.Object
                );

        }

        [Fact()]
        public async Task Handle_WithValidRequest_ReturnsCreatedDishId()
        {

            //arrange 

            var restaurantId = 1;

            var restaurant = new Restaurant
            {
                Id = restaurantId,
            };

            var command = new CreateDishCommand()
            {
                RestaurantId = restaurantId
            };

            var dish = new Dish();

            _mapperMock.Setup(d => d.Map<Dish>(command)).Returns(dish);
            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);
            _dishRepository.Setup(d => d.Create(It.IsAny<Dish>())).ReturnsAsync(1);
            _authorizationServiceMock.Setup(a => a.Authorize(restaurant, ResourceOperation.Create)).Returns(true);

            
            //act


            var result = await _handler.Handle(command, CancellationToken.None);


            //assert

            result.Should().Be(1);
            _dishRepository.Verify(d => d.Create(dish), Times.Once());

        }

        [Fact()]
        public async Task Handle_WithNonExistingRestaurant_ReturnsNotFoundException()
        {

            //arrange

            var restaurantId = 1;

            var command = new CreateDishCommand()
            {
                RestaurantId = restaurantId
            };

            var restaurant = new Restaurant
            {
                Id = restaurantId,
            };


            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync((Restaurant?)null);


            //act

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            
            //assert

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact()]
        public async Task Handle_WithUnauthorizedUser_ReturnsForbidException()
        {
            //arrange

            var restaurantId = 1;

            var command = new CreateDishCommand()
            {
                RestaurantId = restaurantId
            };

            var restaurant = new Restaurant
            {
                Id = restaurantId,
            };


            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);

            _authorizationServiceMock.Setup(a => a.Authorize(restaurant, ResourceOperation.Create)).Returns(false);


            //act

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);


            //assert

            await act.Should().ThrowAsync<ForbidException>();

        }
    }
}