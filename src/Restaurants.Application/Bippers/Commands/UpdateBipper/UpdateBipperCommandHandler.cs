using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Bippers.Dtos;
using Restaurants.Application.Common.Interfaces;
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
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateBipperCommandHandler> _logger;
        private readonly IBipperAuthorizationService _bipperAuthorizationService;
        private readonly IClientRepository _clientRepository;

        public UpdateBipperCommandHandler(IBipperRepository bipperRepository, IRestaurantRepository restaurantRepository, IUserContext userContext, IMapper mapper, ILogger<UpdateBipperCommandHandler> logger, IBipperAuthorizationService bipperAuthorizationService, IClientRepository clientRepository)
        {
            _bipperRepository = bipperRepository;
            _restaurantRepository = restaurantRepository;
            _userContext = userContext;
            _mapper = mapper;
            _logger = logger;
            _bipperAuthorizationService = bipperAuthorizationService;
            _clientRepository = clientRepository;
        }

        public async Task<BipperDTO> Handle(UpdateBipperCommand request, CancellationToken cancellationToken)
        {

            var currentUser = _userContext.GetCurrentUser();
            _bipperAuthorizationService.EnsureIsAuthenticated(currentUser);

            var restaurant = await _restaurantRepository.GetByIdAsync(request.RestaurantId);

            if (restaurant == null)
            {
                throw new NotFoundException(nameof(Restaurant), request.RestaurantId.ToString());

            }

            var client = await _clientRepository.FindByIdAsync(request.ClientId);

            if (client == null)
            {
                throw new NotFoundException(nameof(Client), request.RestaurantId.ToString());
            }


            var bipperToUpdate = await _bipperRepository.FindByIdAsync(request.Id);

            if (bipperToUpdate == null)
            {
                throw new NotFoundException(nameof(Bipper), request.Id.ToString());
            }


            _bipperAuthorizationService.EnsureValidResource(restaurant,ResourceOperation.Update);
            _bipperAuthorizationService.EnsureBipperBelongsToRestaurant(restaurant,bipperToUpdate);
            _bipperAuthorizationService.EnsureBipperBelongsToClient(client, bipperToUpdate);


            _mapper.Map(request, bipperToUpdate);
            await _bipperRepository.UpdateAsync(bipperToUpdate);


            var bipperDTO = _mapper.Map<BipperDTO>(bipperToUpdate);

            return bipperDTO;

        }

    }
}
