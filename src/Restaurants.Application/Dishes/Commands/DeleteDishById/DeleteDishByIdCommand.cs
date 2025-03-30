using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Dishes.Commands.DeleteDishById
{
    public class DeleteDishByIdCommand : IRequest
    {

        public int DishId { get; set; }
        public int RestaurantId { get; set; }


        public DeleteDishByIdCommand(int restaurantId,int dishId)
        {
            RestaurantId = restaurantId;
            DishId = dishId;   
        }
    }
}
