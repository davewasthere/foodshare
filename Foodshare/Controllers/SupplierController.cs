using Foodshare.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Foodshare.Controllers
{
    public class SupplierController : Controller
    {
        private FoodshareDbContext db = new FoodshareDbContext();

        // GET: Supplier
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string email)
        {
            var supplier = db.Suppliers.Where(x => x.Email == email).FirstOrDefault();

            if (supplier == null)
            {
                throw new Exception("not found");
            }



            return Redirect("/");
        }

    }
}