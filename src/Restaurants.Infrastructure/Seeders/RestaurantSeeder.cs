using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Infrastructure.Seeders
{
    public class RestaurantSeeder : IRestaurantSeeder
    {

        private readonly RestaurantsDbContext _dbContext;

        public RestaurantSeeder(RestaurantsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Seed()
        {
            if (_dbContext.Database.GetPendingMigrations().Any()) 
            {
            await _dbContext.Database.MigrateAsync();
            }


            if (await _dbContext.Database.CanConnectAsync())
            {
                if (!_dbContext.Restaurants.Any())
                {
                    var restaurants = GetRestaurants();
                    _dbContext.Restaurants.AddRange(restaurants);
                    await _dbContext.SaveChangesAsync();
                }

                if (!_dbContext.Roles.Any())
                {

                    var roles = GetRoles();
                    _dbContext.Roles.AddRange(roles);
                    await _dbContext.SaveChangesAsync();


                }
            }
        }

        public IEnumerable<IdentityRole> GetRoles()
        {
            List<IdentityRole> roles =
                [
                    new IdentityRole(UserRoles.Admin)
                    {
                        NormalizedName = UserRoles.Admin.ToUpper()
                    },
                    new IdentityRole(UserRoles.Owner)
                    {
                         NormalizedName = UserRoles.Owner.ToUpper()
                    },
                    new IdentityRole(UserRoles.User)
                    {
                         NormalizedName = UserRoles.User.ToUpper()
                    }
                ];

            return roles;
        }

        public IEnumerable<Restaurant> GetRestaurants()
        {

            User owner = new User
            {
                Email = "seed-user@test.com"
            };

            List<Restaurant> restaurants = [

                new()
                {
                    Owner = owner,
                    Name = "KFC",
                    Category = "Fast Food",
                    Description = "Description Value",
                    ContactEmail = "contact@kfc.com",
                    HasDelivery = true,
                    Dishes =
                    [

                        new ()
                        {
                            Name = "NashVille Hot Chicken",
                            Description = "NashVille Hot Chiken (10 pcs.)",
                            Price = 10.30M
                        },

                        new ()
                        {
                            Name = "Chicken Nuggets",
                            Description = "Chicken Nuggets (5 pcs.)",
                            Price = 5.30M
                        }

                    ],
                    Address = new ()
                    {
                        City = "London",
                        Street = "Cork St 5",
                        PostalCode = "WC2N 5DU",

                    }
                },
                new Restaurant()
                {
                    Owner = owner,
                    Name = "McDonalds",
                    Category = "Fast Food",
                    Description = "Description 2",
                    ContactEmail = "contact@mcdonalds.com",
                    HasDelivery = true,
                    Address = new Address()
                    {
                        City = "London",
                        Street = "Boots 193",
                        PostalCode = "W1F 8SR"
                    }
                }
            ];

            return restaurants;
        }

    }
}
