using Restaurants.Application.Users;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Common.Interfaces
{
    public interface IBipperAuthorizationService
    {

        void EnsureIsAuthenticated(CurrentUser? user);
        void EnsureValidResource(Restaurant restaurant, ResourceOperation operation);
        void EnsureBipperBelongsToRestaurant(Restaurant restaurant, Bipper bipper);

        void EnsureBipperBelongsToClient(Client client, Bipper bipper);

    }
}
