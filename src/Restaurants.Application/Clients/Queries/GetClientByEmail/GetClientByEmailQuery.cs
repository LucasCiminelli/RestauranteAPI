using MediatR;
using Restaurants.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Clients.Queries.GetClientById
{
    public class GetClientByIdQuery : IRequest<Client>
    {

        public string Email { get; set; } = string.Empty;
    }
}
