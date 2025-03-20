using Microsoft.Extensions.Logging;
using Restaurants.Application.Users;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Infrastructure.Authorization.AuthorizationServices
{
    public class RestaurantAuthorizationService : IRestaurantAuthorizationService
    {

        private readonly ILogger<RestaurantAuthorizationService> _logger;
        private readonly IUserContext _userContext;

        public RestaurantAuthorizationService(ILogger<RestaurantAuthorizationService> logger, IUserContext userContext)
        {
            _logger = logger;
            _userContext = userContext;
        }

        public bool Authorize(Restaurant restaurant, ResourceOperation resourceOperation)
        {

            var user = _userContext.GetCurrentUser();


            _logger.LogInformation("Authorizng user {UserEmail}, to {Operation} for restaurant {RestaurantName}", user!.Email, resourceOperation, restaurant.Name);



              if (resourceOperation == ResourceOperation.Read || resourceOperation == ResourceOperation.Create)
            {
                _logger.LogInformation("Create/Read opereacion - successfull authorization");
                return true;
            }

            if (resourceOperation == ResourceOperation.Delete && user.IsInRole(UserRoles.Admin))
            {
                _logger.LogInformation("Admin user, delete operation - successfull authorization");
                return true;

            }
            if ((resourceOperation == ResourceOperation.Delete || resourceOperation == ResourceOperation.Update) && user.Id == restaurant.OwnerId)
            {
                _logger.LogInformation("Restaurant owner - successfull authorization");
                return true;

            }

            return false;

        }
    }
}
