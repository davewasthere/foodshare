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
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private FoodshareDbContext db = new FoodshareDbContext();

        // GET: Users
        public ActionResult Index()
        {
            var users = db.Users.ToList();

            return View(users);
        }

        public ActionResult ToggleRole(string id, string role)
        {
            var user = db.Users.Find(id);
            var isInRole = false;

            if (user != null)
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

                if (!roleManager.RoleExists(role))
                {
                    roleManager.Create(new IdentityRole(role));
                }

                if (userManager.IsInRole(id, role))
                {
                    userManager.RemoveFromRole(id, role);
                }
                else
                {
                    userManager.AddToRole(id, role);
                    isInRole = true;
                }

                db.SaveChanges();
            }



            return Json(new { id = id, isInRole = isInRole });
        }
    }
}