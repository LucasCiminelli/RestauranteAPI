using MediatR;
using Restaurants.Application.Dishes.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Dishes.Queries.GetAllDishesForRestaurant
{
    public class GetAllDishesForRestaurantQuery : IRequest<IEnumerable<DishDTO>>
    {

        public int RestaurantId { get; set; }

        public GetAllDishesForRestaurantQuery(int restaurantId)
        {
            RestaurantId = restaurantId;
        }
    }
}
