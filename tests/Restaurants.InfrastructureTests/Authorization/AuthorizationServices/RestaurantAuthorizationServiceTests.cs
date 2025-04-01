using Xunit;
using Restaurants.Infrastructure.Authorization.AuthorizationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Users;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Constants;
using FluentAssertions;

namespace Restaurants.Infrastructure.Authorization.AuthorizationServices.Tests
{
    public class RestaurantAuthorizationServiceTests
    {

        private readonly Mock<ILogger<RestaurantAuthorizationService>> _loggerMock;
        private readonly Mock<IUserContext> _userContextMock;
        private readonly RestaurantAuthorizationService _service;


        public RestaurantAuthorizationServiceTests()
        {
            _loggerMock = new Mock<ILogger<RestaurantAuthorizationService>>();
            _userContextMock = new Mock<IUserContext>();
            _service = new RestaurantAuthorizationService(_loggerMock.Object, _userContextMock.Object);
        }


        [Fact()]
        public void Handle_AuthorizeServiceOwnerDelete_ShouldSucceed()
        {

            var currentUser = new CurrentUser("1", "test@email.com", new[] { "Owner" }, null, null);
            var restaurant = new Restaurant
            {
                OwnerId = currentUser.Id
            };

            _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);
            var result = _service.Authorize(restaurant, ResourceOperation.Delete);


            result.Should().BeTrue();
            currentUser.IsInRole(UserRoles.Owner).Should().BeTrue();

        }

        [Fact()]
        public void Handle_AuthorizeServiceOwnerUpdate_ShouldSucceed()
        {

            var currentUser = new CurrentUser("1", "test@email.com", new[] { "Owner" }, null, null);
            var restaurant = new Restaurant
            {
                OwnerId = currentUser.Id
            };

            _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);
            var result = _service.Authorize(restaurant, ResourceOperation.Update);


            result.Should().BeTrue();
            currentUser.IsInRole(UserRoles.Owner).Should().BeTrue();


        }

        [Fact()]
        public void Handle_AuthorizeServiceOwnerDelete_ShouldFailed()
        {

            var currentUser = new CurrentUser("1", "test@email.com", new[] { "Owner" }, null, null);
            var restaurant = new Restaurant
            {
                OwnerId = "12345"
            };

            _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);
            var result = _service.Authorize(restaurant, ResourceOperation.Delete);


            result.Should().BeFalse();
            currentUser.Id.Should().NotBe("12345");


        }

        [Fact()]
        public void Handle_AuthorizeServiceOwnerUpdate_ShouldFailed()
        {

            var currentUser = new CurrentUser("1", "test@email.com", new[] { "Owner" }, null, null);
            var restaurant = new Restaurant
            {
                OwnerId = "12345"
            };

            _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);
            var result = _service.Authorize(restaurant, ResourceOperation.Update);


            result.Should().BeFalse();
            currentUser.Id.Should().NotBe("12345");


        }

        [Fact()]
        public void Handle_AuthorizeServiceAdminDelete_ShouldSucceed()
        {

            var currentUser = new CurrentUser("1", "test@email.com", new[] { "Admin" }, null, null);
            var restaurant = new Restaurant
            {
                OwnerId = currentUser.Id
            };

            _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);
            var result = _service.Authorize(restaurant, ResourceOperation.Delete);


            result.Should().BeTrue();
            currentUser.IsInRole(UserRoles.Admin).Should().BeTrue();


        }

        [Fact()]
        public void Handle_AuthorizeServiceAdminDelete_ShouldFailed()
        {

            var currentUser = new CurrentUser("1", "test@email.com", new[] { "User" }, null, null);
            var restaurant = new Restaurant
            {
                OwnerId = "123456"
            };

            _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);
            var result = _service.Authorize(restaurant, ResourceOperation.Delete);


            currentUser.IsInRole(UserRoles.Admin).Should().BeFalse();
            result.Should().BeFalse();

        }

        [Fact()]
        public void Handle_AuthorizeService_ShouldFail_WhenUserIsNull()
        {
            _userContextMock.Setup(u => u.GetCurrentUser()).Returns((CurrentUser?)null);

            var restaurant = new Restaurant { OwnerId = "12345" };

            var resultDelete = _service.Authorize(restaurant, ResourceOperation.Delete);
            var resultUpdate = _service.Authorize(restaurant, ResourceOperation.Update);

            resultDelete.Should().BeFalse();
            resultUpdate.Should().BeFalse();
        }

        [Fact()]
        public void Handle_AuthorizeService_ShouldSucceed_ForReadAndCreate()
        {
            var currentUser = new CurrentUser("1", "test@email.com", new[] { "User" }, null, null);
            _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

            var restaurant = new Restaurant { OwnerId = "12345" };

            var resultRead = _service.Authorize(restaurant, ResourceOperation.Read);
            var resultCreate = _service.Authorize(restaurant, ResourceOperation.Create);

            resultRead.Should().BeTrue();
            resultCreate.Should().BeTrue();
        }

    }
}