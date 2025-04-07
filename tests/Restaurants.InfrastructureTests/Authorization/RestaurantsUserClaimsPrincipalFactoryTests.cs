using Xunit;
using Restaurants.Infrastructure.Authorization;
using System;
using System.Linq;
using Restaurants.Domain.Entities;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using FluentAssertions;
using System.Security.Claims;

namespace Restaurants.Infrastructure.Authorization.Tests
{
    public class RestaurantsUserClaimsPrincipalFactoryTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RestaurantsUserClaimsPrincipalFactoryTests()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(
                userStoreMock.Object,
                null!, null!, null!, null!, null!, null!, null!, null!
            );

            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            _roleManager = new RoleManager<IdentityRole>(
                roleStoreMock.Object,
                new IRoleValidator<IdentityRole>[0],
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                null!
            );
        }

        [Fact]
        public async Task Handle_WhenUserHasDateOfBirthAndNationality_ShouldAddClaimsToPrincipal()
        {

            // Arrange

            var options = Options.Create(new IdentityOptions());

            var user = new User
            {
                Id = "123",
                UserName = "testuser",
                DateOfBirth = new DateOnly(1995, 12, 12),
                Nationality = "Argentine"
            };

             _userManagerMock
             .Setup(m => m.GetUserIdAsync(It.IsAny<User>()))
             .ReturnsAsync("123");

              _userManagerMock
              .Setup(m => m.GetUserNameAsync(It.IsAny<User>()))
              .ReturnsAsync("testuser");

            var factory = new RestaurantsUserClaimsPrincipalFactory(_userManagerMock.Object, _roleManager, options);


            // Act
            var principal = await factory.CreateAsync(user);
            var claims = principal.Claims.ToList();


            // Assert
            claims.Should().ContainSingle(c => c.Type == AppClaimTypes.Nationality && c.Value == "Argentine");
            claims.Should().ContainSingle(c => c.Type == AppClaimTypes.DateOfBirth && c.Value == "1995-12-12");
        }

        

    }
}
