using Restaurants.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Domain.Respositories
{
    public interface IDishRepository
    {
        Task<int> Create(Dish entity);
        Task RemoveAllDishes(IEnumerable<Dish> entities);
    }
}
