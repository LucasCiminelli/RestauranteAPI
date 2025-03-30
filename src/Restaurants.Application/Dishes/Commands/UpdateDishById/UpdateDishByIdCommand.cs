using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Restaurants.Application.Dishes.Commands.UpdateDishById
{
    public class UpdateDishByIdCommand : IRequest
    {
        [JsonIgnore]
        public int RestaurantId { get; set; }
        [JsonIgnore]
        public int  DishId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? KiloCalories { get; set; }

       

    }
}
