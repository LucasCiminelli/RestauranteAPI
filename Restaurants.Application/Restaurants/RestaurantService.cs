using Microsoft.Extensions.Logging;
using Restaurants.Application.Restaurants.Dtos;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Respositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Restaurants
{
    internal class RestaurantService : IRestaurantService
    {

        private readonly IRestaurantRepository _restaurantRepository;
        private readonly ILogger<RestaurantService> _logger;

        public RestaurantService(IRestaurantRepository restaurantRepository, ILogger<RestaurantService> logger)
        {
            _restaurantRepository = restaurantRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<RestaurantDTO>> GetAllRestaurants()
        {
            _logger.LogInformation("Getting all Restaurants");

            var restaurants = await _restaurantRepository.GetAllAsync();

            var restaurantsDto = restaurants.Select(RestaurantDTO.FromEntity);
                
            return restaurantsDto;
        }

        public async Task<RestaurantDTO> GetById(int id)
        {
            _logger.LogInformation($"Getting restaurant {id}");
            var restaurant = await _restaurantRepository.GetByIdAsync(id);

            var restaurantDto = RestaurantDTO.FromEntity(restaurant);

            return restaurantDto;
        }
    }
}
