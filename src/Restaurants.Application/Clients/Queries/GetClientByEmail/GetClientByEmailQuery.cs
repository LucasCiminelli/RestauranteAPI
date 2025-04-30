using MediatR;
using Restaurants.Application.Clients.Dtos;
using Restaurants.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Clients.Queries.GetClientById
{
    public class GetClientByEmailQuery : IRequest<ClientDTO>
    {

        public string Email { get; set; } = string.Empty;
    }
}
