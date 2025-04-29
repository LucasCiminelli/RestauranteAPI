using AutoMapper;
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

namespace Restaurants.Application.Clients.Commands.CreateClient
{
    public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, Client>
    {

        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateClientCommandHandler> _logger;
        private readonly IUserContext _userContext;

        public CreateClientCommandHandler(IClientRepository clientRepository, IMapper mapper, ILogger<CreateClientCommandHandler> logger, IUserContext context)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
            _logger = logger;
            _userContext = context;
        }

        public async Task<Client> Handle(CreateClientCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _userContext.GetCurrentUser();

            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("User must be authenticated");
            }

            _logger.LogInformation("Buscando un cliente existente con el email {Email}", request.Email);

            var existingClient = await _clientRepository.FindByEmailAsync(request.Email!);

            if (existingClient != null)
            {
                _logger.LogInformation("El cliente con el email {Email} ya existe en la base de datos", existingClient.Email);
                throw new InvalidOperationException("El cliente ya existe en la base de datos");
            }

            _logger.LogInformation("Creando un nuevo usuario con la siguiente información: {Request}", request);

            var newClient = new Client
            {
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Phone = request.Phone,
                Email = request.Email,
            };

            await _clientRepository.CreateAsync(newClient);


            return newClient;
        }
    }
}
