using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Restaurants.Dtos;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Respositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Restaurants.Commands.CreateRestaurant
{
    public class CreateRestaurantCommandHandler : IRequestHandler<CreateRestaurantCommand, int>
    {
        private readonly ILogger<CreateRestaurantCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IRestaurantRepository _restaurantRepository;

        public CreateRestaurantCommandHandler(ILogger<CreateRestaurantCommandHandler> logger, IMapper mapper, IRestaurantRepository restaurantRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _restaurantRepository = restaurantRepository;
        }

        public async Task<int> Handle(CreateRestaurantCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating a new Restaurant");

            var restaurant = _mapper.Map<Restaurant>(request);

            int id = await _restaurantRepository.CreateAsync(restaurant);

            return id;
        }
    }
}
