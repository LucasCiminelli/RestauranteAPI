using Xunit;
using Restaurants.Application.Restaurants.Commands.UpdateRestaurant;
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
using Restaurants.Application.Users;
using Restaurants.Domain.Constants;
using FluentAssertions;
using Restaurants.Domain.Exceptions;

namespace Restaurants.Application.Restaurants.Commands.UpdateRestaurant.Tests
{
    public class UpdateRestaurantCommandHandlerTests
    {

        private readonly Mock<ILogger<UpdateRestaurantCommandHandler>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRestaurantRepository> _restaurantRepositoryMock;
        private readonly Mock<IRestaurantAuthorizationService> _restaurantAuthorizationMock;
        private readonly UpdateRestaurantCommandHandler _handler;


        public UpdateRestaurantCommandHandlerTests()
        {
            _loggerMock = new Mock<ILogger<UpdateRestaurantCommandHandler>>();
            _mapperMock = new Mock<IMapper>();
            _restaurantRepositoryMock = new Mock<IRestaurantRepository>();
            _restaurantAuthorizationMock = new Mock<IRestaurantAuthorizationService>();

            _handler = new UpdateRestaurantCommandHandler
                (
                    _loggerMock.Object,
                    _restaurantRepositoryMock.Object,
                    _mapperMock.Object,
                    _restaurantAuthorizationMock.Object
                );
        }

        [Fact()]
        public async Task Handle_WithValidRequest_ShouldUpdateRequest()
        {

            //arrange 

            var userContextMock = new Mock<IUserContext>();

            var restaurantId = 1;
            var command = new UpdateRestaurantCommand()
            {
                Id = restaurantId,
                Name = "New Test",
                Description = "New Description",
                HasDelivery = true
            };

            var restaurant = new Restaurant()
            {
                Id = restaurantId,
                Name = "Test",
                Description = "Description"
            };

            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);

            _restaurantAuthorizationMock.Setup(a => a.Authorize(restaurant, ResourceOperation.Update)).Returns(true);

            //Act

            await _handler.Handle(command,CancellationToken.None);

            //assert

            _restaurantRepositoryMock.Verify(r => r.UpdateAsync(restaurant), Times.Once);
            _mapperMock.Verify(m => m.Map(command, restaurant), Times.Once);

        }

        [Fact()]
        public async Task Handle_WithNonExistingRestaurant_ShouldThrowNotFoundException()
        {
            var restaurantId = 1;

            var command = new UpdateRestaurantCommand
            {
                Id = restaurantId,
            };


            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId))
                .ReturnsAsync((Restaurant?)null);


            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();

        }

        [Fact()]
        public async Task Handle_WithUnauthorizedUser_ShouldThrowForbidException()
        {
            var restaurantId = 1;

            var command = new UpdateRestaurantCommand
            {
                Id = restaurantId,
            };

            var existingRestaurant = new Restaurant()
            {
                Id = restaurantId,
            };

            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId))
               .ReturnsAsync(existingRestaurant);


            _restaurantAuthorizationMock.Setup(a => a.Authorize(existingRestaurant, ResourceOperation.Update)).Returns(false);


            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<ForbidException>();
        }
    }
}