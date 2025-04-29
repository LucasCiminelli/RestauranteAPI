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

namespace Restaurants.Application.Bippers.Commands.UpdateBipper
{
    public class UpdateBipperCommandHandler : IRequestHandler<UpdateBipperCommand, Bipper>
    {

        private readonly IBipperRepository _bipperRepository;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateBipperCommandHandler> _logger;

        public UpdateBipperCommandHandler(IBipperRepository bipperRepository, IUserContext userContext, IMapper mapper, ILogger<UpdateBipperCommandHandler> logger)
        {
            _bipperRepository = bipperRepository;
            _userContext = userContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Bipper> Handle(UpdateBipperCommand request, CancellationToken cancellationToken)
        {

            var currentUser = _userContext.GetCurrentUser();

            if(currentUser == null)
            {
                throw new InvalidOperationException("User Unauthenticated");
            }


            var bipperToUpdate = await _bipperRepository.FindByIdAsync(request.Id);

            if(bipperToUpdate == null)
            {
                throw new NotFoundException(nameof(Bipper), request.Id.ToString());
            }

            _mapper.Map(request, bipperToUpdate);

            await _bipperRepository.UpdateAsync(bipperToUpdate);


            return bipperToUpdate;

        }
    }
}
