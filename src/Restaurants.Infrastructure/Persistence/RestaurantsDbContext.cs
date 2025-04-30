using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restaurants.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Infrastructure.Persistence
{
    public class RestaurantsDbContext : IdentityDbContext<User>
    {
        public RestaurantsDbContext(DbContextOptions<RestaurantsDbContext> options) : base(options)
        {
        }

        internal DbSet<Restaurant> Restaurants { get; set; }
        internal DbSet<Dish> Dishes { get; set; }
        internal DbSet<Bipper> Bippers { get; set; }
        internal DbSet<Client> Clients { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Restaurant>().OwnsOne(r => r.Address);

            modelBuilder.Entity<Restaurant>()
                .HasMany(r => r.Dishes)
                .WithOne()
                .HasForeignKey(d => d.RestaurantId);

            modelBuilder.Entity<Restaurant>()
               .HasMany(r => r.Bippers)
               .WithOne(b => b.Restaurant)
               .HasForeignKey(b => b.RestaurantId)
               .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<User>()
                .HasMany(o => o.OwnedRestaraunts)
                .WithOne(r => r.Owner)
                .HasForeignKey(r => r.OwnerId);

            modelBuilder.Entity<Client>()
              .HasMany(c => c.Bippers)
              .WithOne(b => b.Client)
              .HasForeignKey(b => b.ClientId)
              .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
