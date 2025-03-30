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

namespace Restaurants.Application.Dishes.Commands.UpdateDishById
{
    public class UpdateDishByIdCommandHandler : IRequestHandler<UpdateDishByIdCommand>
    {

        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IDishRepository _dishRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateDishByIdCommandHandler> _logger;
        private readonly IRestaurantAuthorizationService _restaurantAuthorizationService;

        public UpdateDishByIdCommandHandler(IRestaurantRepository restaurantRepository, IDishRepository dishRepository, IMapper mapper, ILogger<UpdateDishByIdCommandHandler> logger, IRestaurantAuthorizationService restaurantAuthorizationService)
        {
            _restaurantRepository = restaurantRepository;
            _dishRepository = dishRepository;
            _mapper = mapper;
            _logger = logger;
            _restaurantAuthorizationService = restaurantAuthorizationService;
        }

        public async Task Handle(UpdateDishByIdCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("updating dish with id {DishId} for restaurant with id {RestaurantId} with {@UpdatedRestaurant}", request.DishId, request.RestaurantId, request);


            var restaurant = await _restaurantRepository.GetByIdAsync(request.RestaurantId);

            if (restaurant == null)
            {
                throw new NotFoundException(nameof(Restaurant), request.RestaurantId.ToString());
            }

            var dishes = restaurant.Dishes;

            if (dishes == null)
            {
                throw new Exception("This restaurant don't have any dishes");
            }

            var dishToUpdate = dishes.FirstOrDefault(x => x.Id == request.DishId);

            if (dishToUpdate == null || dishToUpdate.RestaurantId != request.RestaurantId)
            {

                throw new NotFoundException(nameof(Dish), request.DishId.ToString());

            }

            if(!_restaurantAuthorizationService.Authorize(restaurant, ResourceOperation.Update))
            {
                throw new ForbidException();
            }


            _mapper.Map(request, dishToUpdate);

            await _dishRepository.Update(dishToUpdate);


        }
    }
}
