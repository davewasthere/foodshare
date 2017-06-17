using Foodshare.Models;
using Foodshare.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Foodshare.Controllers
{
    public class SuppliersController : ApiController
    {
        private FoodshareDbContext db = FoodshareDbContext.Create();


        public List<Supplier> Get()
        {
            return db.Suppliers.ToList();
        }
    }
}
