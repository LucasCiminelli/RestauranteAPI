using Xunit;
using Restaurants.Application.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurants.Domain.Constants;
using FluentAssertions;

namespace Restaurants.Application.Users.Tests
{
    public class CurrentUserTests
    {
        [Theory()]
        [InlineData(UserRoles.Admin)]
        [InlineData(UserRoles.User)]
        public void IsInRole_WithMatchingRole_ShouldReturnTrue(string roleName)
        {

            var currentUser = new CurrentUser("1", "test@test.com", [UserRoles.Admin, UserRoles.User], null, null);

            var isInRole = currentUser.IsInRole(roleName);

            isInRole.Should().BeTrue();


        }

        [Fact()]
        public void IsInRole_WithNoMatchingRole_ShouldReturnFalse()
        {

            var currentUser = new CurrentUser("1", "test@test.com", [UserRoles.Admin, UserRoles.User], null, null);

            var isInRole = currentUser.IsInRole(UserRoles.Owner);

            isInRole.Should().BeFalse();


        }

        [Fact()]
        public void IsInRole_WithNoMatchingRoleCase_ShouldReturnFalse()
        {

            var currentUser = new CurrentUser("1", "test@test.com", [UserRoles.Admin, UserRoles.User], null, null);

            var isInRole = currentUser.IsInRole(UserRoles.Admin.ToLower());

            isInRole.Should().BeFalse();


        }
    }
}