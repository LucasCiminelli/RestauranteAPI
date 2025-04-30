using AutoMapper;
using Restaurants.Application.Clients.Commands.CreateClient;
using Restaurants.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Clients.Dtos
{
    public class ClientsProfile : Profile
    {
        public ClientsProfile()
        {


            CreateMap<Client, ClientDTO>();
            CreateMap<CreateClientCommand, ClientDTO>();


        }
    }
}
