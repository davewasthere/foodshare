using Foodshare.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Foodshare.DAL
{
    public class FoodshareDbContext : IdentityDbContext<ApplicationUser>
    {
        public FoodshareDbContext() : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static FoodshareDbContext Create()
        {
            return new FoodshareDbContext();
        }

        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<FoodItem> FoodItems { get; set; }

    }
}