﻿using Xunit;
using Restaurants.Application.Users.Commands.UnnasignUserRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using Restaurants.Application.Users.Commands.AssignUserRole;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Entities;
using FluentAssertions;
using Restaurants.Domain.Exceptions;

namespace Restaurants.Application.Users.Commands.UnnasignUserRole.Tests
{
    public class UnassignUserRoleCommandHandlerTests
    {
        private readonly Mock<ILogger<UnassignUserRoleCommandHandler>> _loggerMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly UnassignUserRoleCommandHandler _handler;

        public UnassignUserRoleCommandHandlerTests()
        {
            _loggerMock = new Mock<ILogger<UnassignUserRoleCommandHandler>>();

            var store = new Mock<IUserStore<User>>();

            _userManagerMock = new Mock<UserManager<User>>
                (
                    store.Object,
                    null!, null!, null!, null!, null!, null!, null!, null!
                );

            var roleStore = new Mock<IRoleStore<IdentityRole>>();

            _roleManagerMock = new Mock<RoleManager<IdentityRole>>
                (
                    roleStore.Object,
                    null!, null!, null!, null!
                );


            _handler = new UnassignUserRoleCommandHandler(_loggerMock.Object, _userManagerMock.Object, _roleManagerMock.Object);
        }


        [Fact()]
        public async Task Handle_ForValidCommand_ReturnsUnassignRoleToUser()
        {
            //arrange

            var user = new User
            {
                Email = "Testemail@test.com"
            };

            var role = new IdentityRole
            {
                Name = "Admin"
            };

            var command = new UnassignUserRoleCommand();

            command.UserEmail = "Testemail@test.com";
            command.RoleName = "Admin";


            _userManagerMock.Setup(u => u.FindByEmailAsync(command.UserEmail)).ReturnsAsync(user);

            _roleManagerMock.Setup(r => r.FindByNameAsync(command.RoleName)).ReturnsAsync(role);

            _userManagerMock.Setup(u => u.RemoveFromRoleAsync(user, role.Name));


            //act

            await _handler.Handle(command, CancellationToken.None);


            //assert

            _userManagerMock.Verify(u => u.RemoveFromRoleAsync(user, role.Name), Times.Once);
        }

        [Fact()]
        public async Task Handle_ForNonExistingUser_ReturnsNotFoundException()
        {
            //arrange

            var user = new User
            {
                Email = "Testemail@test.com"
            };

            var role = new IdentityRole
            {
                Name = "Admin"
            };

            var command = new UnassignUserRoleCommand();

            command.UserEmail = "Testemail@test.com";
            command.RoleName = "Admin";


            _userManagerMock.Setup(u => u.FindByEmailAsync(command.UserEmail)).ReturnsAsync((User?)null);


            //act

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);


            //assert

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact()]
        public async Task Handle_ForNonExistingRole_ReturnsNotFoundException()
        {
            //arrange

            var user = new User
            {
                Email = "Testemail@test.com"
            };

            var role = new IdentityRole
            {
                Name = "Admin"
            };

            var command = new UnassignUserRoleCommand();

            command.UserEmail = "Testemail@test.com";
            command.RoleName = "Admin";


            _userManagerMock.Setup(u => u.FindByEmailAsync(command.UserEmail)).ReturnsAsync(user);

            _roleManagerMock.Setup(r => r.FindByNameAsync(command.RoleName)).ReturnsAsync((IdentityRole?)null);


            //act

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);


            //assert

            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}