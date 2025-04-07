using Xunit;
using Restaurants.Application.Restaurants.Commands.UploadRestaurantLogo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Restaurants.Domain.Respositories;
using Restaurants.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Constants;
using FluentAssertions;
using Restaurants.Domain.Exceptions;

namespace Restaurants.Application.Restaurants.Commands.UploadRestaurantLogo.Tests
{

    public class UploadRestaurantLogoCommandHandlerTests
    {

        private readonly Mock<ILogger<UploadRestaurantLogoCommandHandler>> _loggerMock;
        private readonly Mock<IRestaurantRepository> _restaurantRepositoryMock;
        private readonly Mock<IRestaurantAuthorizationService> _restaurantAuthorizationService;
        private readonly Mock<IBlobStorageService> _blobStorageServiceMock;
        private readonly UploadRestaurantLogoCommandHandler _handler;

        public UploadRestaurantLogoCommandHandlerTests()
        {
            _loggerMock = new Mock<ILogger<UploadRestaurantLogoCommandHandler>>();
            _restaurantRepositoryMock = new Mock<IRestaurantRepository>();
            _restaurantAuthorizationService = new Mock<IRestaurantAuthorizationService>();
            _blobStorageServiceMock = new Mock<IBlobStorageService>();
            _handler = new UploadRestaurantLogoCommandHandler(
                _loggerMock.Object,
                _restaurantRepositoryMock.Object,
                _restaurantAuthorizationService.Object,
                _blobStorageServiceMock.Object);
        }

        [Fact()]
        public async Task Handle_WithValidRequest_ShouldUploadLogo()
        {
            var restaurantId = 1;

            var restaurant = new Restaurant
            {
                Id = restaurantId
            };

            var uploadCommand = new UploadRestaurantLogoCommand()
            {
                RestaurantId = restaurantId,

            };

            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);

            _restaurantAuthorizationService.Setup(a => a.Authorize(restaurant, ResourceOperation.Update)).Returns(true);

            _blobStorageServiceMock.Setup(b => b.UploadToBlobAsync(It.IsAny<Stream>(), It.IsAny<string>())).ReturnsAsync(It.IsAny<string>());



            await _handler.Handle(uploadCommand, CancellationToken.None);


            _restaurantRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _restaurantAuthorizationService.Verify(r => r.Authorize(It.IsAny<Restaurant>(), ResourceOperation.Update), Times.Once);
            _blobStorageServiceMock.Verify(b => b.UploadToBlobAsync(It.IsAny<Stream>(), It.IsAny<string>()), Times.Once);
            _restaurantRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Restaurant>()), Times.Once);


        }

        [Fact()]
        public async Task Handle_WithNonExistingRestaurant_ShouldReturnNotFoundException()
        {
            var restaurantId = 1;

            var restaurant = new Restaurant
            {
                Id = restaurantId
            };

            var uploadCommand = new UploadRestaurantLogoCommand()
            {
                RestaurantId = restaurantId,

            };

            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync((Restaurant?)null);


            Func<Task> act = async () => await _handler.Handle(uploadCommand, CancellationToken.None);


            await act.Should().ThrowAsync<NotFoundException>();


        }

        [Fact()]
        public async Task Handle_WithUnauthorizedUser_ShouldReturnForbidException()
        {
            var restaurantId = 1;

            var restaurant = new Restaurant
            {
                Id = restaurantId
            };

            var uploadCommand = new UploadRestaurantLogoCommand()
            {
                RestaurantId = restaurantId,

            };

            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);

            _restaurantAuthorizationService.Setup(a => a.Authorize(restaurant, ResourceOperation.Update)).Returns(false);


            Func<Task> act = async () => await _handler.Handle(uploadCommand, CancellationToken.None);


            await act.Should().ThrowAsync<ForbidException>();


        }
    }
}