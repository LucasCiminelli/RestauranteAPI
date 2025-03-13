using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurants.Application.Restaurants;
using Restaurants.Application.Restaurants.Commands.CreateRestaurant;
using Restaurants.Application.Restaurants.Dtos;
using Restaurants.Application.Restaurants.Queries.GetAllRestaurants;
using Restaurants.Application.Restaurants.Queries.GetRestaurantById;
using System.Threading.Tasks;

namespace Restaurants.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RestaurantsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var request = new GetAllRestaurantsQuery();
            var restaurants = await _mediator.Send(request);

            return Ok(restaurants);
        }



        [HttpGet("{id}", Name = "GetById")]

        public async Task<IActionResult> GetById([FromRoute] int id)
        {

            try
            {
                var request = new GetRestaurantByIdQuery(id);

                var restaurant = await _mediator.Send(request);


                return Ok(restaurant);
            }
            catch (KeyNotFoundException)
            {

                return NotFound();
            }

        }


        [HttpPost]
        public async Task<IActionResult> CreateRestaurant(CreateRestaurantCommand request)
        {
            int id = await _mediator.Send(request);

            return CreatedAtAction(nameof(GetById), new { id }, null);
        }

    }
}
