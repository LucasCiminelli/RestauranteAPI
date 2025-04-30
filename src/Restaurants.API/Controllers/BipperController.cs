using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurants.Application.Bippers.Commands.CreateBipper;
using Restaurants.Application.Bippers.Commands.UpdateBipper;
using Restaurants.Application.Bippers.Dtos;
using Restaurants.Application.Bippers.Queries.GetBipperById;

namespace Restaurants.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/restaurants/{restaurantId}/clients/{clientId}/bippers")]
    public class BipperController : ControllerBase
    {

        private readonly IMediator _mediator;

        public BipperController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]

        public async Task<ActionResult<BipperDTO>> GetBipperById(
            [FromRoute] Guid id, 
            [FromRoute] int restaurantId, 
            [FromRoute] int clientId)
        {
            var query = new GetBipperByIdQuery
            {
                Id = id,
                RestaurantId = restaurantId,
                ClientId = clientId
            };

            var bipper = await _mediator.Send(query);

            return Ok(bipper);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]

        public async Task<ActionResult<BipperDTO>> CreateBipper(
            [FromRoute] int restaurantId, 
            [FromRoute] int clientId, 
            [FromBody] CreateBipperCommand command)
        {

            command.RestaurantId = restaurantId;
            command.ClientId = clientId;


            var bipper = await _mediator.Send(command);


            return CreatedAtAction(nameof(GetBipperById), new { id = bipper.Id, restaurantId = restaurantId, clientId = clientId }, bipper);
        }


        [HttpPut]
        [Route("{bipperId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]

        public async Task<ActionResult<BipperDTO>> UpdateBipper(
            [FromRoute] int restaurantId, 
            [FromRoute] Guid bipperId, 
            [FromRoute] int clientId, 
            [FromBody] UpdateBipperCommand command)
        {

            command.RestaurantId = restaurantId;
            command.Id = bipperId;
            command.ClientId = clientId;

            var bipper = await _mediator.Send(command);


            return Ok(bipper);
        }

    }
}
