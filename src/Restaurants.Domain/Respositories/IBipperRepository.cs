using Restaurants.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Domain.Respositories
{
    public interface IBipperRepository
    {
        Task<Bipper> CreateAsync(Bipper bipper);
        Task UpdateAsync(Bipper bipper);
        Task<Bipper?> FindByIdAsync(Guid id);
    }
}
