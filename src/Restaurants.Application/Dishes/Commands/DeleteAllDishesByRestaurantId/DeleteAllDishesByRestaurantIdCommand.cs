using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Dishes.Commands.DeleteAllDishesByRestaurantId
{
    public class DeleteAllDishesByRestaurantIdCommand : IRequest
    {

        public int RestaurantId { get; set; }

        public DeleteAllDishesByRestaurantIdCommand(int restaurantId)
        {
            RestaurantId = restaurantId;
        }
    }
}
