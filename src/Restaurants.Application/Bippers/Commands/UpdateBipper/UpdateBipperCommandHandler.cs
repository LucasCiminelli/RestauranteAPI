using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Bippers.Dtos;
using Restaurants.Application.Users;
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

namespace Restaurants.Application.Bippers.Commands.UpdateBipper
{
    public class UpdateBipperCommandHandler : IRequestHandler<UpdateBipperCommand, BipperDTO>
    {

        private readonly IBipperRepository _bipperRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IRestaurantAuthorizationService _authorizationService;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateBipperCommandHandler> _logger;

        public UpdateBipperCommandHandler(IBipperRepository bipperRepository, IRestaurantRepository restaurantRepository, IRestaurantAuthorizationService authorizationService, IUserContext userContext, IMapper mapper, ILogger<UpdateBipperCommandHandler> logger)
        {
            _bipperRepository = bipperRepository;
            _restaurantRepository = restaurantRepository;
            _authorizationService = authorizationService;
            _userContext = userContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BipperDTO> Handle(UpdateBipperCommand request, CancellationToken cancellationToken)
        {

            var currentUser = _userContext.GetCurrentUser();
            EnsureIsAuthenticated(currentUser);

            var restaurant = await _restaurantRepository.GetByIdAsync(request.RestaurantId);

            if (restaurant == null)
            {
                throw new NotFoundException(nameof(Restaurant), request.RestaurantId.ToString());

            }

            var bipperToUpdate = await _bipperRepository.FindByIdAsync(request.Id);

            if (bipperToUpdate == null)
            {
                throw new NotFoundException(nameof(Bipper), request.Id.ToString());
            }


            EnsureValidResource(restaurant);
            EnsureBipperBelongsToRestaurant(bipperToUpdate, restaurant);


            _mapper.Map(request, bipperToUpdate);
            await _bipperRepository.UpdateAsync(bipperToUpdate);


            var bipperDTO = _mapper.Map<BipperDTO>(bipperToUpdate);

            return bipperDTO;

        }


        private void EnsureIsAuthenticated(CurrentUser? user)
        {
            if (user == null)
            {
                throw new InvalidOperationException("User Unauthenticated");
            }
        }

        private void EnsureValidResource(Restaurant restaurant)
        {
            if (!_authorizationService.Authorize(restaurant, ResourceOperation.Update))
            {
                throw new ForbidException();
            }
        }

        private void EnsureBipperBelongsToRestaurant(Bipper bipper, Restaurant restaurant)
        {
            if (bipper.RestaurantId != restaurant.Id)
            {
                throw new ForbidException();
            }
        }
    }
}
