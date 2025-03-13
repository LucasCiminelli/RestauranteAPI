using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Respositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Restaurants.Commands.UpdateRestaurant
{
    public class UpdateRestaurantCommandHandler : IRequestHandler<UpdateRestaurantCommand, bool>
    {

        private readonly ILogger<UpdateRestaurantCommandHandler> _logger;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IMapper _mapper;

        public UpdateRestaurantCommandHandler(ILogger<UpdateRestaurantCommandHandler> logger, IRestaurantRepository restaurantRepository, IMapper mapper)
        {
            _logger = logger;
            _restaurantRepository = restaurantRepository;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateRestaurantCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("updating restaurant");

            var restaurant = await _restaurantRepository.GetByIdAsync(request.Id);

            if (restaurant is null)
            {
                _logger.LogWarning($"Restaurant with Id {request.Id} not found.");
                return false;
            }

            _mapper.Map(request, restaurant);

            await _restaurantRepository.UpdateAsync(restaurant);

            return true;


        }
    }
}
