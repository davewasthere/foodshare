using Foodshare.DAL;
using Foodshare.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Foodshare.Controllers
{
    [RequireHttps]

    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private FoodshareDbContext db = new FoodshareDbContext();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public UsersController()
        {

        }

        public UsersController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Users
        public ActionResult Index()
        {
            var users = db.Users.OrderByDescending(x => x.DateCreated).ToList();

            var roles = db.Roles.ToList();
            ViewBag.Roles = roles;

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

        public ActionResult BulkInvite()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> BulkInvite(string EmailAddresses, bool UserIsAgency)
        {
            List<string> emailLog = new List<string>();

            var emails = EmailAddresses.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var email in emails)
            {
                var user = db.Users.Where(x => x.Email == email).SingleOrDefault();

                if (user == null)
                {
                    // doesn't exist
                    user = new ApplicationUser { Email = email, UserName = email, DateCreated = DateTime.Now };

                    var result = await UserManager.CreateAsync(user, ConfigurationManager.AppSettings["temporaryPassword"]);

                    if (result.Succeeded)
                    {
                        emailLog.Add(user.Email + " Added");
                    }
                }
                else
                {
                    if (!user.EmailConfirmed)
                    {
                        emailLog.Add(user.Email + " already existed - will resend invitation email");
                    }
                }


                // we assume user has been set up okay
                if (user != null && !user.EmailConfirmed)
                {
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    var message = string.Empty;

                    if (UserIsAgency)
                    {
                        UserManager.AddToRole(user.Id, "Agency");

                        message = "<p>Dear Bendigo Foodshare client</p>" +
                            "<p>Do you need more prepared food for your clients who are experiencing food poverty?</p>" +
                            "<p>Is your agency or school able to pick the food up in Bendigo?</p>" +
                            "<p>If so, you should join our trial of the Bendigo Foodshare Food Donation App.</p>" +
                            "<p>Prepared meals, cooked vegetables, sandwiches, cakes, and so on are now available.</p>" +
                            "<p>Our App enables companies with excess food to donate directly to our registered agencies and schools that provide food for those in need. The Donor company simply takes a photo of the food, uploads it to our App and adds detail about when and where it needs to be picked up. Bendigo Foodshare Agencies that have registered online will receive an email notifying them of food that is being donated. If you want to take that food, you just click on the link and confirm that you want it. It will then no longer be available for other agencies, and you can pick it up at the agreed time and place.</p>" +
                            "<p>If you do want the food for your agency, you need to respond quickly as most of the food donated through this app needs to be picked up on the same day or within 24 hours.</p>" +
                            "<p>At present we have St John of God Hospital, Bendigo Goldfields catering services, The Bendigo Council Meals on Wheels program and Bendigo Foodshare registered as food donors. We expect more catering companies in the near future.</p>" +
                            "<p>To be able to accept the food, you need to click on the link below, complete your details and change your temporary password to something you can remember.</p>" +
                            "<p>Get online and give it a go!</p>" +
                            "<p>Please activate your account by clicking <a href=\"" + callbackUrl + "\">here</a></p>" +
                            "<p>You have been assigned a temporary password of <b>Donations123</b> - please change this after you activate your account.</p>" +
                            "<p>thanks,</p>" +
                            "<p>Cathie Steele<br />" +
                            "Chair of the Board<br />" +
                            "Bendigo Foodshare<br />" +
                            "0407307632</p>";
                    }
                    else
                    {
                        message = "<p>You have been invited to be a supplier at Bendigo Foodshare.</p>" +
                            "<p>Please activate your account by clicking <a href=\"" + callbackUrl + "\">here</a></p>" +
                            "<p>You have been assigned a temporary password of <b>Donations123</b> - please change this after you activate your account.</p>";
                    }

                    await UserManager.SendEmailAsync(user.Id, "Bendigo Foodshare App Invitation", message);

                    emailLog.Add(user.Email + " Sent Invite Email");
                }
                else
                {
                    emailLog.Add(user.Email + " NOT sent email. Already verified.");
                }
            }

            TempData["UsersEmailed"] = emailLog;

            return RedirectToAction("Index");
        }
    }
}