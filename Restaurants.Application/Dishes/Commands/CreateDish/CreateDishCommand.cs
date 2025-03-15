using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Dishes.Commands.CreateDish
{
    public class CreateDishCommand : IRequest
    {

        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int? KiloCalories { get; set; }
        public int RestaurantId { get; set; }

    }
}
