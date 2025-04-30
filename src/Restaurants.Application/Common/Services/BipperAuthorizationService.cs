using Restaurants.Application.Common.Interfaces;
using Restaurants.Application.Users;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Common.Services
{
    public class BipperAuthorizationService : IBipperAuthorizationService
    {
        private readonly IRestaurantAuthorizationService _restaurantAuthorizationService;

        public BipperAuthorizationService(IRestaurantAuthorizationService restaurantAuthorizationService)
        {
            _restaurantAuthorizationService = restaurantAuthorizationService;
        }

        public void EnsureBipperBelongsToClient(Client client, Bipper bipper)
        {
            if(bipper.ClientId != client.Id)
            {
                throw new InvalidOperationException($"The bipper with id {bipper.Id} does not belong to the specified client with id {client.Id}");
            }
        }

        public void EnsureBipperBelongsToRestaurant(Restaurant restaurant, Bipper bipper)
        {
            if (bipper.RestaurantId != restaurant.Id)
            {

                throw new ForbidException();
            
            }
        }

        public void EnsureIsAuthenticated(CurrentUser? user)
        {
            if (user == null)
            {
                throw new InvalidOperationException("User is not authenticated");
            }
        }

        public void EnsureValidResource(Restaurant restaurant, ResourceOperation operation)
        {
            if (!_restaurantAuthorizationService.Authorize(restaurant, operation))
            {
                throw new ForbidException();
            }
        }
    }
}
