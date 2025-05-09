using Xunit;
using Restaurants.Application.Bippers.Commands.UpdateBipper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Restaurants.Domain.Respositories;
using Restaurants.Application.Common.Interfaces;
using Restaurants.Application.Users;
using Restaurants.Application.Bippers.Commands.CreateBipper;
using Restaurants.Application.Bippers.Dtos;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using FluentAssertions;
using Restaurants.Domain.Exceptions;

namespace Restaurants.Application.Bippers.Commands.UpdateBipper.Tests
{
    public class UpdateBipperCommandHandlerTests
    {
        private readonly Mock<ILogger<UpdateBipperCommandHandler>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IBipperRepository> _bipperRepositoryMock;
        private readonly Mock<IBipperAuthorizationService> _bipperAuthorizationServiceMock;
        private readonly Mock<IRestaurantRepository> _restaurantRepositoryMock;
        private readonly Mock<IClientRepository> _clientRepositoryMock;
        private readonly Mock<IUserContext> _userContextMock;
        private readonly UpdateBipperCommandHandler _handler;

        public UpdateBipperCommandHandlerTests()
        {

            _loggerMock = new Mock<ILogger<UpdateBipperCommandHandler>>();
            _mapperMock = new Mock<IMapper>();
            _bipperRepositoryMock = new Mock<IBipperRepository>();
            _bipperAuthorizationServiceMock = new Mock<IBipperAuthorizationService>();
            _restaurantRepositoryMock = new Mock<IRestaurantRepository>();
            _clientRepositoryMock = new Mock<IClientRepository>();
            _userContextMock = new Mock<IUserContext>();

            _handler = new UpdateBipperCommandHandler(
                _bipperRepositoryMock.Object,
                _restaurantRepositoryMock.Object,
                _userContextMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _bipperAuthorizationServiceMock.Object,
                _clientRepositoryMock.Object);
        }



        [Fact()]
        public async Task HandleUpdate_WithValidRequest_ShouldUpdateBipper()
        {
            //arrange

            var restaurantId = 1;
            var restaurant = new Restaurant { Id = restaurantId };

            var currentUser = new CurrentUser("owner-id", "test@test.com", [], null, null);
            var client = new Client { Id = 1 };

            var bipperId = new Guid();
            var bipper = new Bipper
            {
                Id = bipperId,
                RestaurantId = restaurantId,
                ClientId = client.Id,
                Status = BipperStatus.Pending,
                IsReady = false,
                IsActive = true,
                Type = BipperType.TableAssignment
            };

            var command = new UpdateBipperCommand
            {
                Id = bipperId,
                RestaurantId = restaurantId,
                ClientId = client.Id,
                Status = BipperStatus.Notified,
                IsReady = true,
            };


            _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);
            _clientRepositoryMock.Setup(c => c.FindByIdAsync(1)).ReturnsAsync(client);
            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);

            _bipperRepositoryMock.Setup(b => b.FindByIdAsync(bipperId)).ReturnsAsync(bipper);

            _bipperAuthorizationServiceMock.Setup(bas => bas.EnsureIsAuthenticated(currentUser));
            _bipperAuthorizationServiceMock.Setup(bas => bas.EnsureValidResource(restaurant, ResourceOperation.Update));
            _bipperAuthorizationServiceMock.Setup(bas => bas.EnsureBipperBelongsToRestaurant(restaurant, bipper));


            _mapperMock.Setup(m => m.Map(command, bipper)).Callback<UpdateBipperCommand, Bipper>
            (
                (cmd, b)
                =>
                {
                    if (cmd.Status.HasValue)
                        b.Status = cmd.Status.Value;
                    if (cmd.IsReady.HasValue)
                        b.IsReady = cmd.IsReady.Value;
                }
            )
            .Returns(bipper);

            _mapperMock.Setup(m => m.Map<BipperDTO>(bipper)).Returns(new BipperDTO());


            //act

            var result = await _handler.Handle(command, CancellationToken.None);


            //assert

            result.Should().BeOfType<BipperDTO>();
            _bipperRepositoryMock.Verify(b => b.UpdateAsync(bipper), Times.Once());
            bipper.RestaurantId.Should().Be(1);
            bipper.Id.Should().Be(bipperId);
            bipper.Status.Should().Be(BipperStatus.Notified);
            bipper.IsReady.Should().BeTrue();
        }

        [Fact()]
        public async Task UpdateBipper_WithNullCurrentUser_ReturnsInvalidOperationException()
        {


            //arrange

            var restaurantId = 1;
            var restaurant = new Restaurant { Id = restaurantId };
            var currentUser = new CurrentUser("owner-id", "test@test.com", [], null, null);
            var client = new Client { Id = 1 };
            var bipper = new Bipper { RestaurantId = restaurantId };

            var command = new UpdateBipperCommand { RestaurantId = restaurantId, ClientId = 1 };

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

        [Fact()]
        public async Task UpdateBipper_WithNonExistingRestaurant_ReturnsNotFoundException()
        {
            var restaurantId = 1;
            var restaurant = new Restaurant { Id = restaurantId };
            var currentUser = new CurrentUser("owner-id", "test@test.com", [], null, null);
            var client = new Client { Id = 1 };
            var bipper = new Bipper { RestaurantId = restaurantId };

            var command = new UpdateBipperCommand { RestaurantId = restaurantId, ClientId = 1 };

            _clientRepositoryMock.Setup(c => c.FindByIdAsync(1)).ReturnsAsync(client);
            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync((Restaurant)null!);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();

        }

        [Fact()]
        public async Task UpdateBipper_WithNonExistingClient_ReturnsNotFoundException()
        {
            var restaurantId = 1;
            var restaurant = new Restaurant { Id = restaurantId };
            var currentUser = new CurrentUser("owner-id", "test@test.com", [], null, null);
            var client = new Client { Id = 1 };
            var bipper = new Bipper { RestaurantId = restaurantId };

            var command = new UpdateBipperCommand { RestaurantId = restaurantId, ClientId = 1 };

            _clientRepositoryMock.Setup(c => c.FindByIdAsync(1)).ReturnsAsync((Client)null!);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();

        }

        [Fact()]
        public async Task UpdateBipper_WithNonExistingBipper_ReturnsNotFoundException()
        {
            //arrange

            var restaurantId = 1;
            var restaurant = new Restaurant { Id = restaurantId };

            var currentUser = new CurrentUser("owner-id", "test@test.com", [], null, null);
            var client = new Client { Id = 1 };

            var bipperId = new Guid();
            var bipper = new Bipper
            {
                Id = bipperId,
                RestaurantId = restaurantId,
                ClientId = client.Id,
                Status = BipperStatus.Pending,
                IsReady = false,
                IsActive = true,
                Type = BipperType.TableAssignment
            };

            var command = new UpdateBipperCommand
            {
                Id = bipperId,
                RestaurantId = restaurantId,
                ClientId = client.Id,
                Status = BipperStatus.Notified,
                IsReady = true,
            };


            _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);
            _clientRepositoryMock.Setup(c => c.FindByIdAsync(1)).ReturnsAsync(client);
            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);

            _bipperRepositoryMock.Setup(b => b.FindByIdAsync(bipperId)).ReturnsAsync((Bipper)null!);


            //act

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact()]
        public async Task UpdateBipper_WithNotValidResource_ReturnsInvalidOperationException()
        {

            //arrange

            var restaurantId = 1;
            var restaurant = new Restaurant { Id = restaurantId };

            var currentUser = new CurrentUser("owner-id", "test@test.com", [], null, null);
            var client = new Client { Id = 1 };

            var bipperId = new Guid();
            var bipper = new Bipper
            {
                Id = bipperId,
                RestaurantId = restaurantId,
                ClientId = client.Id,
                Status = BipperStatus.Pending,
                IsReady = false,
                IsActive = true,
                Type = BipperType.TableAssignment
            };

            var command = new UpdateBipperCommand
            {
                Id = bipperId,
                RestaurantId = restaurantId,
                ClientId = client.Id,
                Status = BipperStatus.Notified,
                IsReady = true,
            };


            _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);
            _clientRepositoryMock.Setup(c => c.FindByIdAsync(1)).ReturnsAsync(client);
            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);

            _bipperRepositoryMock.Setup(b => b.FindByIdAsync(bipperId)).ReturnsAsync(bipper);

            _bipperAuthorizationServiceMock
            .Setup(bas => bas.EnsureValidResource(restaurant, ResourceOperation.Update))
            .Throws(new ForbidException());



            //act

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);


            //assert

            await act.Should().ThrowAsync<ForbidException>();
        }

        [Fact()]
        public async Task UpdateBipper_BipperDontBelongsToRestaurant_ReturnsInvalidOperationException()
        {

            //arrange

            var restaurantId = 1;
            var restaurant = new Restaurant { Id = restaurantId };

            var currentUser = new CurrentUser("owner-id", "test@test.com", [], null, null);
            var client = new Client { Id = 1 };

            var bipperId = new Guid();
            var bipper = new Bipper
            {
                Id = bipperId,
                RestaurantId = restaurantId,
                ClientId = client.Id,
                Status = BipperStatus.Pending,
                IsReady = false,
                IsActive = true,
                Type = BipperType.TableAssignment
            };

            var command = new UpdateBipperCommand
            {
                Id = bipperId,
                RestaurantId = restaurantId,
                ClientId = client.Id,
                Status = BipperStatus.Notified,
                IsReady = true,
            };


            _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);
            _clientRepositoryMock.Setup(c => c.FindByIdAsync(1)).ReturnsAsync(client);
            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);

            _bipperRepositoryMock.Setup(b => b.FindByIdAsync(bipperId)).ReturnsAsync(bipper);

            _bipperAuthorizationServiceMock
            .Setup(bas => bas.EnsureBipperBelongsToRestaurant(restaurant, bipper))
            .Throws(new ForbidException());



            //act

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);


            //assert

            await act.Should().ThrowAsync<ForbidException>();
        }

        [Fact()]
        public async Task UpdateBipper_BipperDontBelongsToClient_ReturnsInvalidOperationException()
        {

            //arrange

            var restaurantId = 1;
            var restaurant = new Restaurant { Id = restaurantId };

            var currentUser = new CurrentUser("owner-id", "test@test.com", [], null, null);
            var client = new Client { Id = 1 };

            var bipperId = new Guid();
            var bipper = new Bipper
            {
                Id = bipperId,
                RestaurantId = restaurantId,
                ClientId = client.Id,
                Status = BipperStatus.Pending,
                IsReady = false,
                IsActive = true,
                Type = BipperType.TableAssignment
            };

            var command = new UpdateBipperCommand
            {
                Id = bipperId,
                RestaurantId = restaurantId,
                ClientId = client.Id,
                Status = BipperStatus.Notified,
                IsReady = true,
            };


            _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);
            _clientRepositoryMock.Setup(c => c.FindByIdAsync(1)).ReturnsAsync(client);
            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);

            _bipperRepositoryMock.Setup(b => b.FindByIdAsync(bipperId)).ReturnsAsync(bipper);

            _bipperAuthorizationServiceMock
            .Setup(bas => bas.EnsureBipperBelongsToClient(client, bipper))
            .Throws(new InvalidOperationException());



            //act

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);


            //assert

            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}