using AutoMapper;
using Restaurants.Application.Restaurants.Commands.CreateRestaurant;
using Restaurants.Application.Restaurants.Commands.UpdateRestaurant;
using Restaurants.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Restaurants.Dtos
{
    public class RestaurantsProfile : Profile
    {
        protected RestaurantsProfile()
        {

            CreateMap<UpdateRestaurantCommand, Restaurant>()
                .ForAllMembers(
                opts => opts.Condition
                (
                    (src, dest, srcMember) => srcMember != null)
                );

            CreateMap<CreateRestaurantDTO, Restaurant>()
                .ForMember(d => d.Address,
                options => options.MapFrom
                (
                    src => new Address
                    {
                        City = src.City,
                        Street = src.Street,
                        PostalCode = src.PostalCode,
                    })
                );

            CreateMap<CreateRestaurantCommand,  Restaurant>()
                .ForMember(d => d.Address,
                options => options.MapFrom
                (
                    src => new Address
                    {
                        City = src.City,
                        Street = src.Street,
                        PostalCode = src.PostalCode,
                    })
                );

            CreateMap<Restaurant, RestaurantDTO>()
                .ForMember(d => d.City,
                    options => options.MapFrom
                    (
                        src => src.Address == null
                        ? null
                        : src.Address.City
                    )
                )
                .ForMember(d => d.Street,
                    options => options.MapFrom
                    (
                        src => src.Address == null
                        ? null
                        : src.Address.Street
                    )
                )
                .ForMember(d => d.PostalCode,
                    options => options.MapFrom
                    (
                        src => src.Address == null
                        ? null
                        : src.Address.PostalCode
                    )
                )
                .ForMember(d => d.Dishes,
                    options => options.MapFrom
                    (
                        src => src.Dishes
                    )
                );
        }
    }
}
