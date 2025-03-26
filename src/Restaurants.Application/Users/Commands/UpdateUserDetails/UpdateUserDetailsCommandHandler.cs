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

namespace Restaurants.Application.Users.Commands.UpdateUserDetails
{
    public class UpdateUserDetailsCommandHandler : IRequestHandler<UpdateUserDetailsCommand>
    {

        private readonly ILogger<UpdateUserDetailsCommandHandler> _logger;
        private readonly IUserContext _userContext;
        private readonly IUserStore<User> _userStore;

        public UpdateUserDetailsCommandHandler(ILogger<UpdateUserDetailsCommandHandler> logger, IUserContext userContext, IUserStore<User> userStore)
        {
            _logger = logger;
            _userContext = userContext;
            _userStore = userStore;
        }

        public async Task Handle(UpdateUserDetailsCommand request, CancellationToken cancellationToken)
        {
            var user = _userContext.GetCurrentUser();
            _logger.LogInformation("Updating user {UserId}, with {@Request}", user!.Id, request);


            var dbUser = await _userStore.FindByIdAsync(user!.Id, cancellationToken);


            if (dbUser == null)
            {

                throw new NotFoundException(nameof(User), user!.Id);
            }

            dbUser.Nationality = request.Nationality;
            dbUser.DateOfBirth = request.DateOfBirth;

            await _userStore.UpdateAsync(dbUser, cancellationToken);

        }
    }
}
