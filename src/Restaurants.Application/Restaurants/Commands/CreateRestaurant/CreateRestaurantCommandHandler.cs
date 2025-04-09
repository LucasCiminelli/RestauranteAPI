using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Restaurants.Dtos;
using Restaurants.Application.Users;
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
        private readonly IUserContext _userContext;

        public CreateRestaurantCommandHandler(ILogger<CreateRestaurantCommandHandler> logger, IMapper mapper, IRestaurantRepository restaurantRepository, IUserContext userContext)
        {
            _logger = logger;
            _mapper = mapper;
            _restaurantRepository = restaurantRepository;
            _userContext = userContext;
        }

        public async Task<int> Handle(CreateRestaurantCommand request, CancellationToken cancellationToken)
        {

            var currentUser = _userContext.GetCurrentUser();

            if(currentUser == null)
            {
                throw new UnauthorizedAccessException("User must be authenticated");
            }

            _logger.LogInformation("{UserEmail} [{UserId}] is creating a new Restaurant {Restaurant}", currentUser!.Email, currentUser.Id, request);

            var restaurant = _mapper.Map<Restaurant>(request);
            restaurant.OwnerId = currentUser.Id;

            int id = await _restaurantRepository.CreateAsync(restaurant);

            return id;
        }
    }
}
