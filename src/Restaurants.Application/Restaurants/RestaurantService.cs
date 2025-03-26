using AutoMapper;
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
        private readonly IMapper _mapper;

        public RestaurantService(IRestaurantRepository restaurantRepository, ILogger<RestaurantService> logger, IMapper mapper)
        {
            _restaurantRepository = restaurantRepository;
            _logger = logger;
            _mapper = mapper;
        }

        //public async Task<int> Create(CreateRestaurantDTO restaurantDto)
        //{
        //    _logger.LogInformation("Creating a new Restaurant");

        //    var restaurant = _mapper.Map<Restaurant>(restaurantDto);

        //    int id = await _restaurantRepository.CreateAsync(restaurant);

        //    return id;
        //}

        //public async Task<IEnumerable<RestaurantDTO>> GetAllRestaurants()
        //{
        //    _logger.LogInformation("Getting all Restaurants");

        //    var restaurants = await _restaurantRepository.GetAllAsync();

        //    var restaurantsDto = _mapper.Map<IEnumerable<RestaurantDTO>>(restaurants);

        //    return restaurantsDto;
        //}

        //public async Task<RestaurantDTO> GetById(int id)
        //{
        //    _logger.LogInformation($"Getting restaurant {id}");
        //    var restaurant = await _restaurantRepository.GetByIdAsync(id);

        //    var restaurantDto = _mapper.Map<RestaurantDTO>(restaurant);

        //    return restaurantDto;
        //}
    }
}
