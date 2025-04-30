using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Bippers.Dtos;
using Restaurants.Application.Common.Interfaces;
using Restaurants.Application.Users;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Respositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Bippers.Queries.GetBipperById
{
    public class GetBipperByIdQueryHandler : IRequestHandler<GetBipperByIdQuery, BipperDTO>
    {

        private readonly IBipperRepository _bipperRepository;
        private readonly ILogger<GetBipperByIdQueryHandler> _logger;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;
        private readonly IBipperAuthorizationService _bipperAuthorizationService;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IClientRepository _clientRepository;

        public GetBipperByIdQueryHandler(IBipperRepository bipperRepository, ILogger<GetBipperByIdQueryHandler> logger, IUserContext userContext, IMapper mapper, IBipperAuthorizationService bipperAuthorizationService, IRestaurantRepository restaurantRepository, IClientRepository clientRepository)
        {
            _bipperRepository = bipperRepository;
            _logger = logger;
            _userContext = userContext;
            _mapper = mapper;
            _bipperAuthorizationService = bipperAuthorizationService;
            _restaurantRepository = restaurantRepository;
            _clientRepository = clientRepository;
        }

        public async Task<BipperDTO> Handle(GetBipperByIdQuery request, CancellationToken cancellationToken)
        {
            var currentUser = _userContext.GetCurrentUser();

            _bipperAuthorizationService.EnsureIsAuthenticated(currentUser);

            
            var restaurant = await _restaurantRepository.GetByIdAsync(request.RestaurantId);

            if(restaurant == null)
            {
                throw new NotFoundException(nameof(Restaurant), request.RestaurantId.ToString());
            }

            var client = await _clientRepository.FindByIdAsync(request.ClientId);

            if(client == null)
            {
                throw new NotFoundException(nameof(Client), request.RestaurantId.ToString());
            }

            var bipper = await _bipperRepository.FindByIdAsync(request.Id);

            if (bipper == null) 
            {
                throw new NotFoundException(nameof(Bipper), request.Id.ToString());

            }

            _bipperAuthorizationService.EnsureBipperBelongsToRestaurant(restaurant, bipper);
            _bipperAuthorizationService.EnsureBipperBelongsToClient(client, bipper);


            var bipperDTO = _mapper.Map<BipperDTO>(bipper);


            return bipperDTO;
        }
    }
}
