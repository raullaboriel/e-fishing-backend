using System;
using efishingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace efishingAPI.Context
{
    public class eFishing : DbContext
    {
        public eFishing(DbContextOptions<eFishing> options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CartProduct> CartProducts { get; set; }

    }
}
