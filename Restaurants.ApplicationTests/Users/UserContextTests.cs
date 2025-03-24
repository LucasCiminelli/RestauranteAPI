﻿using Xunit;
using Restaurants.Application.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Restaurants.Domain.Constants;
using FluentAssertions;

namespace Restaurants.Application.Users.Tests
{
    public class UserContextTests
    {
        [Fact()]
        public void GetCurrentUser_WithAuthenticatedUser_ShouldReturnCurrentUser()
        {

            //arrage

            var dateOfBirth = new DateOnly(1995, 12, 18);

            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            var claims = new List<Claim>()
            {
                 new(ClaimTypes.NameIdentifier, "1"),
                 new(ClaimTypes.Email, "test@test.com"),
                 new(ClaimTypes.Role, UserRoles.Admin),
                 new(ClaimTypes.Role, UserRoles.User),
                 new("Nationality", "German"),
                 new("DateOfBirth",dateOfBirth.ToString("yyyy-MM-dd"))
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));

            var userContext = new UserContext(httpContextAccessorMock.Object);


            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext()
            {
                User = user
            });

            //act

            var currentUser = userContext.GetCurrentUser();


            //asset

            currentUser.Should().NotBeNull();
            currentUser.Id.Should().Be("1");
            currentUser.Email.Should().Be("test@test.com");
            currentUser.Roles.Should().ContainInOrder(UserRoles.Admin, UserRoles.User);
            currentUser.Nationality.Should().Be("German");
            currentUser.DateOfBirth.Should().Be(dateOfBirth);

        }

        [Fact()]
        public void GetCurrentUser_WithUserContextIsNull_ThrowsInvalidOperationException()
        {
            //Arrange

            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext)null!);

            var userContext = new UserContext(httpContextAccessorMock.Object);


            //Act

            Action action = () => userContext.GetCurrentUser();

            //Asset

            action.Should().Throw<InvalidOperationException>().WithMessage("User context is not present");
        }
    }
}