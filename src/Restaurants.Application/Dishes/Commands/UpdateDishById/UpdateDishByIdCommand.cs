using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Dishes.Commands.UpdateDishById
{
    public class UpdateDishByIdCommand : IRequest
    {

        public int RestaurantId { get; set; }
        public int  DishId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Price { get; set; }
        public int? KiloCalories { get; set; }

       

    }
}
