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


        public HttpResponseMessage GetByEmail(string email)
        {
            var supplier = db.Suppliers.Where(x => x.Email == email).SingleOrDefault();

            if (supplier == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, new Exception("that supplier doesn't exist"));
            }

            return Request.CreateResponse(HttpStatusCode.OK, supplier);
        }
    }
}
