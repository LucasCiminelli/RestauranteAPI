using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurants.Application.Dishes.Commands.CreateDish;
using Restaurants.Application.Dishes.Commands.DeleteAllDishesByRestaurantId;
using Restaurants.Application.Dishes.Dtos;
using Restaurants.Application.Dishes.Queries.GetAllDishesForRestaurant;
using Restaurants.Application.Dishes.Queries.GetDishByIdForRestaurant;

namespace Restaurants.API.Controllers
{
    [ApiController]
    [Route("api/restaurants/{restaurantId}/dishes")]
    public class DishesController : ControllerBase
    {

        private readonly IMediator _mediator;

        public DishesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateDish([FromRoute] int restaurantId, [FromBody] CreateDishCommand command)
        {

            command.RestaurantId = restaurantId;

            var dishId = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetDishByIdForRestaurant), new { restaurantId, dishId }, null);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DishDTO>))]
        public async Task<ActionResult<IEnumerable<DishDTO>>> GetAllDishesForRestaurant([FromRoute] int restaurantId)
        {
            var request = new GetAllDishesForRestaurantQuery(restaurantId);
            var dishes = await _mediator.Send(request);

            return Ok(dishes);
        }

        [HttpGet("{dishId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DishDTO))]
        public async Task<ActionResult<DishDTO>> GetDishByIdForRestaurant([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            var request = new GetDishByIdForRestaurantQuery(restaurantId, dishId);
            var dish = await _mediator.Send(request);

            return Ok(dish);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteAllDishesByRestaurantId([FromRoute] int restaurantId)
        {
            var command = new DeleteAllDishesByRestaurantIdCommand(restaurantId);

            await _mediator.Send(command);

            return NoContent();
        }

    }
}
