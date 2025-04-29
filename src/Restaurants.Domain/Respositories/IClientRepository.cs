using Restaurants.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Domain.Respositories
{
    public interface IClientRepository
    {

        Task<Client> CreateAsync(Client client);
        Task<Client?> FindByEmailAsync(string email);


    }
}
