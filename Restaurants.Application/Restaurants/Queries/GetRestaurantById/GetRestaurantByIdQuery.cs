using MediatR;
using Restaurants.Application.Restaurants.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Restaurants.Queries.GetRestaurantById
{
    public class GetRestaurantByIdQuery : IRequest<RestaurantDTO>
    {

        public int Id { get; set; }

        public GetRestaurantByIdQuery(int id)
        {
            Id = id;
        }
    }
}
