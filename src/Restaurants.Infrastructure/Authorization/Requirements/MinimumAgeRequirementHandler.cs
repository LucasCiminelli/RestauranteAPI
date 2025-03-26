using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Infrastructure.Authorization.Requirements
{
    public class MinimumAgeRequirementHandler : AuthorizationHandler<MinimumAgeRequirement>
    {

        private readonly ILogger<MinimumAgeRequirementHandler> _logger;
        private readonly IUserContext _userContext;

        public MinimumAgeRequirementHandler(ILogger<MinimumAgeRequirementHandler> logger, IUserContext userContext)
        {
            _logger = logger;
            _userContext = userContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {

            var user = _userContext.GetCurrentUser();

            _logger.LogInformation("User: {Email}, date of birth {DateOfBirh} - Handling MinimumAgeRequirement", user!.Email, user!.DateOfBirth);

            


            if (user.DateOfBirth!.Value.AddYears(requirement.MinimumAge) <= DateOnly.FromDateTime(DateTime.Today))
            {
                _logger.LogInformation("Authorization Succeeded");
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

                
            return Task.CompletedTask;

        }
    }
}
