using Microsoft.EntityFrameworkCore;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Respositories;
using Restaurants.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Infrastructure.Repositories
{
    public class ClientsRepository : IClientRepository
    {

        private readonly RestaurantsDbContext _restaurantsDbContext;

        public ClientsRepository(RestaurantsDbContext restaurantsDbContext)
        {
            _restaurantsDbContext = restaurantsDbContext;
        }

        public async Task<Client> CreateAsync(Client client)
        {
            await _restaurantsDbContext.Clients.AddAsync(client);
            await _restaurantsDbContext.SaveChangesAsync();

            return client;
        }

        public async Task<Client?> FindByEmailAsync(string email)
        {

            var client = await _restaurantsDbContext.Clients.FirstOrDefaultAsync(c => c.Email == email);
            return client;

        }
    }
}
