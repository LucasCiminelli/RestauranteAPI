using Xunit;
using Restaurants.Application.Restaurants.Commands.DeleteRestaurant;
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
using Restaurants.Application.Users;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Constants;
using FluentAssertions;
using Restaurants.Domain.Exceptions;

namespace Restaurants.Application.Restaurants.Commands.DeleteRestaurant.Tests
{
    public class DeleteRestaurantCommandHandlerTests
    {

        private readonly Mock<ILogger<DeleteRestaurantCommandHandler>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRestaurantRepository> _restaurantRepositoryMock;
        private readonly Mock<IRestaurantAuthorizationService> _authorizationServiceMock;
        private readonly DeleteRestaurantCommandHandler _handler;

        public DeleteRestaurantCommandHandlerTests()
        {

            _loggerMock = new Mock<ILogger<DeleteRestaurantCommandHandler>>();
            _mapperMock = new Mock<IMapper>();
            _restaurantRepositoryMock = new Mock<IRestaurantRepository>();
            _authorizationServiceMock = new Mock<IRestaurantAuthorizationService>();
            _handler = new DeleteRestaurantCommandHandler(_loggerMock.Object, _restaurantRepositoryMock.Object, _authorizationServiceMock.Object);


        }

        [Fact()]
        public async Task Handle_WithValidRequest_ShouldDeleteRequest()
        {
            //arrange

            var userContextMock = new Mock<IUserContext>();
            var restaurantId = 1;

            var command = new DeleteRestaurantCommand(restaurantId);
            

            var restaurant = new Restaurant
            {
                Id = restaurantId
            };

            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);

            _authorizationServiceMock.Setup(a => a.Authorize(restaurant, ResourceOperation.Delete)).Returns(true);


            // act

            await _handler.Handle(command, CancellationToken.None);

            //assert

            _restaurantRepositoryMock.Verify(r => r.DeleteAsync(restaurant), Times.Once);

        }

        [Fact()]
        public async Task Handle_WithNonExistingRestaurant_ShouldThrowNotFoundException()
        {
            //arrange

            var restaurantId = 1;

            var command = new DeleteRestaurantCommand(restaurantId);

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

            var command = new DeleteRestaurantCommand(restaurantId);

            var existingRestaurant = new Restaurant
            {
                Id = restaurantId
            };

            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(existingRestaurant);

            _authorizationServiceMock.Setup(a => a.Authorize(existingRestaurant, ResourceOperation.Delete)).Returns(false);


            //act

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);


            //assert

            await act.Should().ThrowAsync<ForbidException>();


        }

    }
}