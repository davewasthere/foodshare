using Foodshare.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Foodshare.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private FoodshareDbContext db = new FoodshareDbContext();

        // GET: Users
        public ActionResult Index()
        {
            var users = db.Users.ToList();

            return View(users);
        }
    }
}