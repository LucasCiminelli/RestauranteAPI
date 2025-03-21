using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Infrastructure.Authorization.Requirements
{
    public class AtLeastTwoRestaurantsRequirement : IAuthorizationRequirement
    {
        public int Quantity { get; set; }

        public AtLeastTwoRestaurantsRequirement(int quantity)
        {
            Quantity = quantity;
        }
    }
}
