using AutoMapper;
using Restaurants.Application.Bippers.Commands.CreateBipper;
using Restaurants.Application.Bippers.Commands.UpdateBipper;
using Restaurants.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Bippers.Dtos
{
    public class BippersProfile : Profile
    {
        public BippersProfile()
        {

            CreateMap<UpdateBipperCommand, Bipper>();
            CreateMap<CreateBipperCommand, Bipper>();

            CreateMap<Bipper, BipperDTO>();



        }
    }
}
