using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Users;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Respositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Clients.Queries.GetClientById
{
    public class GetClientByEmailQueryHandler : IRequestHandler<GetClientByEmailQuery, Client>
    {

        private readonly IClientRepository _clientRepository;
        private readonly ILogger<GetClientByEmailQueryHandler> _logger;
        private readonly IUserContext _userContext;

        public GetClientByEmailQueryHandler(IClientRepository clientRepository, ILogger<GetClientByEmailQueryHandler> logger, IUserContext userContext)
        {
            _clientRepository = clientRepository;
            _logger = logger;
            _userContext = userContext;
        }

        public async Task<Client> Handle(GetClientByEmailQuery request, CancellationToken cancellationToken)
        {
            var currentUser = _userContext.GetCurrentUser();

            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("User must be authenticated");
            }

            var existingUser = await _clientRepository.FindByEmailAsync(request.Email);

            if(existingUser == null)
            {
                throw new NotFoundException(nameof(Client), request.Email);
            }


            return existingUser;

        }
    }
}
