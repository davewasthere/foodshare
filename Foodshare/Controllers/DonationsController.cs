using Foodshare.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Foodshare.Controllers
{
    [Authorize]
    public class DonationsController : Controller
    {
        private FoodshareDbContext db = new FoodshareDbContext();

        // GET: Forum
        public ActionResult Index()
        {
            var items = db.Donations.ToList();

            return View(items);
        }
    }
}