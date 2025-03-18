using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurants.Application.Restaurants;
using Restaurants.Application.Restaurants.Commands.CreateRestaurant;
using Restaurants.Application.Restaurants.Commands.DeleteRestaurant;
using Restaurants.Application.Restaurants.Commands.UpdateRestaurant;
using Restaurants.Application.Restaurants.Dtos;
using Restaurants.Application.Restaurants.Queries.GetAllRestaurants;
using Restaurants.Application.Restaurants.Queries.GetRestaurantById;
using System.Threading.Tasks;

namespace Restaurants.API.Controllers
{
    [ApiController]
    [Route("api/restaurants")]
    public class RestaurantsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RestaurantsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RestaurantDTO>))]
        public async Task<ActionResult<IEnumerable<RestaurantDTO>>> GetAll()
        {
            var request = new GetAllRestaurantsQuery();
            var restaurants = await _mediator.Send(request);

            return Ok(restaurants);
        }



        [HttpGet("{id}", Name = "GetById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RestaurantDTO))]
        public async Task<ActionResult<RestaurantDTO>> GetById([FromRoute] int id)
        {
            var request = new GetRestaurantByIdQuery(id);

            var restaurant = await _mediator.Send(request);


            return Ok(restaurant);

        }


        [HttpPost]
        public async Task<IActionResult> CreateRestaurant(CreateRestaurantCommand request)
        {
            int id = await _mediator.Send(request);

            return CreatedAtAction(nameof(GetById), new { id }, null);
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRestaurant([FromRoute] int id)
        {
            var command = new DeleteRestaurantCommand(id);
            await _mediator.Send(command);


            return NoContent();


        }


        //Mi implementación

        //[HttpPatch]
        //public async Task<IActionResult> UpdateRestaurant(UpdateRestaurantCommand request)
        //{
        //    var isUpdated = await _mediator.Send(request);

        //    if (isUpdated)
        //    {
        //        return NoContent();
        //    }

        //    return NotFound();
        //}

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRestaurant([FromRoute] int id, UpdateRestaurantCommand request)
        {

            request.Id = id;

            await _mediator.Send(request);

            return NoContent();

        }

    }
}
