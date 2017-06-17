using Foodshare.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Foodshare.DAL
{
    public class DbContext : IdentityDbContext<ApplicationUser>
    {
        public DbContext() : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static DbContext Create()
        {
            return new DbContext();
        }

        public DbSet<Supplier> Suppliers { get; set; }

    }
}