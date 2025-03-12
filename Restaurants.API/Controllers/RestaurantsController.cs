using Microsoft.AspNetCore.Mvc;
using Restaurants.Application.Restaurants;
using System.Threading.Tasks;

namespace Restaurants.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantsController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantsController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var restaurants = await _restaurantService.GetAllRestaurants();

            return Ok(restaurants);
        }



        [HttpGet("{id}", Name = "GetById")]

        public async Task<IActionResult> GetById([FromRoute] int id)
        {

            try
            {
                var restaurant = await _restaurantService.GetById(id);
                return Ok(restaurant);
            }
            catch (KeyNotFoundException)
            {

                return NotFound();
            }

        }

    }
}
