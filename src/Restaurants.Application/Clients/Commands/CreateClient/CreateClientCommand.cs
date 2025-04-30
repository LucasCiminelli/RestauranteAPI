using MediatR;
using Restaurants.Application.Clients.Dtos;
using Restaurants.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Clients.Commands.CreateClient
{
    public class CreateClientCommand : IRequest<ClientDTO>
    {

        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Please provide a valid phone number")]
        public string Phone { get; set; } = string.Empty;

    }
}
