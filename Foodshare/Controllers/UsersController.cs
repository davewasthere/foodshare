using Foodshare.DAL;
using Foodshare.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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

        public ActionResult ToggleAgency(string id)
        {
            var user = db.Users.Find(id);

            if (user != null)
            {
                user.IsAgency = !user.IsAgency;

                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

                if (!roleManager.RoleExists("Agency"))
                {
                    roleManager.Create(new IdentityRole("Agency"));
                }

                if (user.IsAgency)
                {
                    userManager.AddToRole(id, "Agency");
                }
                else
                {
                    userManager.RemoveFromRole(id, "Agency");
                }

                db.SaveChanges();
            }



            return Json(new { id = id, isAgency = user.IsAgency });
        }
    }
}