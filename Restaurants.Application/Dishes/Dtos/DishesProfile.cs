using AutoMapper;
using Restaurants.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Dishes.Dtos
{
    public class DishesProfile : Profile
    {
        protected DishesProfile()
        {


            CreateMap<Dish, DishDTO>();

        }
    }
}
