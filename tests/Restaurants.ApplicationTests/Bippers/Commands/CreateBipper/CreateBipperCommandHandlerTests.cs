using Xunit;
using Restaurants.Application.Bippers.Commands.CreateBipper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Restaurants.Domain.Respositories;
using Restaurants.Application.Common.Interfaces;
using Restaurants.Application.Users;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Constants;
using FluentAssertions;
using Restaurants.Application.Bippers.Dtos;
using System.Reflection.Metadata;

namespace Restaurants.Application.Bippers.Commands.CreateBipper.Tests
{
    public class CreateBipperCommandHandlerTests
    {

        private readonly Mock<ILogger<CreateBipperCommandHandler>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IBipperRepository> _bipperRepositoryMock;
        private readonly Mock<IBipperAuthorizationService> _bipperAuthorizationServiceMock;
        private readonly Mock<IRestaurantRepository> _restaurantRepositoryMock;
        private readonly Mock<IClientRepository> _clientRepositoryMock;
        private readonly Mock<IUserContext> _userContextMock;
        private readonly CreateBipperCommandHandler _handler;


        public CreateBipperCommandHandlerTests()
        {
            _loggerMock = new Mock<ILogger<CreateBipperCommandHandler>>();
            _mapperMock = new Mock<IMapper>();
            _bipperRepositoryMock = new Mock<IBipperRepository>();
            _bipperAuthorizationServiceMock = new Mock<IBipperAuthorizationService>();
            _restaurantRepositoryMock = new Mock<IRestaurantRepository>();
            _clientRepositoryMock = new Mock<IClientRepository>();
            _userContextMock = new Mock<IUserContext>();

            _handler = new CreateBipperCommandHandler(
                _bipperRepositoryMock.Object,
                _loggerMock.Object,
                _userContextMock.Object,
                _mapperMock.Object,
                _restaurantRepositoryMock.Object,
                _bipperAuthorizationServiceMock.Object,
                _clientRepositoryMock.Object);

        }


        [Fact()]
        public async Task CreateBipper_WithValidCommand_ReturnsBipperDTO()
        {

            //arrange

            var restaurantId = 1;
            var restaurant = new Restaurant { Id = restaurantId };
            var currentUser = new CurrentUser("owner-id", "test@test.com", [], null, null);
            var client = new Client { Id = 1 };
            var bipper = new Bipper { RestaurantId = restaurantId };

            var command = new CreateBipperCommand { RestaurantId = restaurantId, ClientId = 1 };

            _clientRepositoryMock.Setup(c => c.FindByIdAsync(1)).ReturnsAsync(client);
            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);
            _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);
            _bipperRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Bipper>())).ReturnsAsync(bipper);
            _mapperMock.Setup(m => m.Map<Bipper>(command)).Returns(bipper);
            _mapperMock.Setup(m => m.Map<BipperDTO>(bipper)).Returns(new BipperDTO());

            _bipperAuthorizationServiceMock.Setup(bas => bas.EnsureIsAuthenticated(currentUser));
            _bipperAuthorizationServiceMock.Setup(bas => bas.EnsureValidResource(restaurant, ResourceOperation.Create));
            _bipperAuthorizationServiceMock.Setup(bas => bas.EnsureBipperBelongsToRestaurant(restaurant, bipper));


            //act

            var result = await _handler.Handle(command, CancellationToken.None);


            //assert

            result.Should().BeOfType<BipperDTO>();
            _bipperRepositoryMock.Verify(b => b.CreateAsync(bipper), Times.Once());
            bipper.RestaurantId.Should().Be(1);

        }


        [Fact()]
        public async Task CreateBipper_WithNullCurrentUser_ReturnsInvalidOperationException()
        {


            //arrange

            var restaurantId = 1;
            var restaurant = new Restaurant { Id = restaurantId };
            var currentUser = new CurrentUser("owner-id", "test@test.com", [], null, null);
            var client = new Client { Id = 1 };
            var bipper = new Bipper { RestaurantId = restaurantId };

            var command = new CreateBipperCommand { RestaurantId = restaurantId, ClientId = 1 };

            _clientRepositoryMock.Setup(c => c.FindByIdAsync(1)).ReturnsAsync(client);
            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);
            _userContextMock.Setup(u => u.GetCurrentUser()).Returns((CurrentUser)null!);

            _bipperAuthorizationServiceMock
            .Setup(bas => bas.EnsureIsAuthenticated(It.Is<CurrentUser>(u => u == null)))
            .Throws(new InvalidOperationException("User not authenticated"));


            //act

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);


            //assert

            await act.Should().ThrowAsync<InvalidOperationException>();
        }


    }
}