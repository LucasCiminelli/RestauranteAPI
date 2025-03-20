using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Interfaces;
using Restaurants.Domain.Respositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Dishes.Commands.CreateDish
{
    public class CreateDishCommandHandler : IRequestHandler<CreateDishCommand, int>
    {

        private readonly ILogger<CreateDishCommandHandler> _logger;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IDishRepository _dishRepository;
        private readonly IMapper _mapper;
        private readonly IRestaurantAuthorizationService _restaurantAuthorizationService;

        public CreateDishCommandHandler(ILogger<CreateDishCommandHandler> logger, IRestaurantRepository restaurantRepository, IDishRepository dishRepository, IMapper mapper, IRestaurantAuthorizationService restaurantAuthorizationService)
        {
            _logger = logger;
            _restaurantRepository = restaurantRepository;
            _dishRepository = dishRepository;
            _mapper = mapper;
            _restaurantAuthorizationService = restaurantAuthorizationService;
        }

        public async Task<int> Handle(CreateDishCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("Creating a new dish: {@DishRequest}", request);


            var restaurant = await _restaurantRepository.GetByIdAsync(request.RestaurantId);

            if (restaurant is null)
            {

                throw new NotFoundException(nameof(Restaurant), request.RestaurantId.ToString());
            }

            if(!_restaurantAuthorizationService.Authorize(restaurant, ResourceOperation.Create))
            {
                throw new ForbidException();
            }


            var dish = _mapper.Map<Dish>(request);


            return await _dishRepository.Create(dish);
        }
    }
}
