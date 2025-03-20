using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Users;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Respositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Infrastructure.Authorization.Requirements
{
    public class AtLeastTwoRestaurantsRequirementHandler : AuthorizationHandler<AtLeastTwoRestaurantsRequirement>
    {
        private readonly ILogger<AtLeastTwoRestaurantsRequirementHandler> _logger;
        private readonly IUserContext _userContext;
        private readonly IRestaurantRepository _restaurantRepository;

        public AtLeastTwoRestaurantsRequirementHandler(ILogger<AtLeastTwoRestaurantsRequirementHandler> logger, IUserContext userContext, IRestaurantRepository restaurantRepository)
        {
            _logger = logger;
            _userContext = userContext;
            _restaurantRepository = restaurantRepository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AtLeastTwoRestaurantsRequirement requirement)
        {
            var user = _userContext.GetCurrentUser();

            var restaurants = await _restaurantRepository.GetAllAsync();

            var totalRestaurants = restaurants.Count(x => x.OwnerId == user!.Id);

            if (totalRestaurants >= requirement.Quantity)
            {
                _logger.LogInformation("user {OwnerId} has {TotalRestaurants} restaurants registered", user!.Id, totalRestaurants);
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

        }
    }
}
