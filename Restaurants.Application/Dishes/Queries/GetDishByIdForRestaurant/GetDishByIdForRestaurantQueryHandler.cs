using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Dishes.Dtos;
using Restaurants.Application.Dishes.Queries.GetAllDishesForRestaurant;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Respositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Dishes.Queries.GetDishByIdForRestaurant
{
    public class GetDishByIdForRestaurantQueryHandler : IRequestHandler<GetDishByIdForRestaurantQuery, DishDTO>
    {

        private readonly ILogger<GetDishByIdForRestaurantQueryHandler> _logger;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IMapper _mapper;

        public GetDishByIdForRestaurantQueryHandler(ILogger<GetDishByIdForRestaurantQueryHandler> logger, IRestaurantRepository restaurantRepository, IMapper mapper)
        {
            _logger = logger;
            _restaurantRepository = restaurantRepository;
            _mapper = mapper;
        }

        public async Task<DishDTO> Handle(GetDishByIdForRestaurantQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retreiven dish with id {DishId} for restaurant with id {RestaurantId}", request.DishId, request.RestaurantId);

            var restaurant = await _restaurantRepository.GetByIdAsync(request.RestaurantId);

            if (restaurant == null)
            {

                throw new NotFoundException(nameof(Restaurant), request.RestaurantId.ToString());

            }

            var dishes = restaurant.Dishes;

            if(dishes is null)
            {
                throw new Exception("This restaurant don't have any dishes");
            }

            var dish = dishes.FirstOrDefault(x => x.Id == request.DishId);

            if(dish is null)
            {
                throw new NotFoundException(nameof(Dish), request.DishId.ToString());
            }


            var mappedDish = _mapper.Map<DishDTO>(dish);

            return mappedDish;
        }
    }
}
