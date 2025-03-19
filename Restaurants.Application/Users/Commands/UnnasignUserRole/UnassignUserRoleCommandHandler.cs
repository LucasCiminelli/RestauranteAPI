using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Users.Commands.UnnasignUserRole
{
    public class UnassignUserRoleCommandHandler : IRequestHandler<UnassignUserRoleCommand>
    {

        private readonly ILogger<UnassignUserRoleCommandHandler> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UnassignUserRoleCommandHandler(ILogger<UnassignUserRoleCommandHandler> logger, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Handle(UnassignUserRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Unassigning role from User {@Request}", request);

            var user = await _userManager.FindByEmailAsync(request.UserEmail!) ?? throw new NotFoundException(nameof(User), request.UserEmail!);

            var role = await _roleManager.FindByNameAsync(request.RoleName!) ?? throw new NotFoundException(nameof(IdentityRole), request.RoleName!);


            await _userManager.RemoveFromRoleAsync(user, role.Name!);


        }
    }
}
