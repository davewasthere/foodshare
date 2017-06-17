using Foodshare.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Foodshare.Models;
using Microsoft.AspNet.Identity;

namespace Foodshare.Controllers
{
    [Authorize]
    public class DonationsController : Controller
    {
        private FoodshareDbContext db = new FoodshareDbContext();

        // GET: Forum
        public ActionResult Index()
        {

            var userId = User.Identity.GetUserId();

            var items = new List<Donation>();

            if (User.IsInRole("Agency"))
            {
                items = db.Donations.OrderByDescending(x => x.DonationId).ToList();
            }
            else
            {
                items = db.Donations.Where(x => x.DonatedById == userId).OrderByDescending(x => x.DonationId).ToList();
            }

            return View(items);
        }

        public ActionResult Edit(int? id)
        {
            var donation = new Donation();

            donation.AvailableFrom = DateTime.Now;
            donation.AvailableTo = DateTime.Now.AddHours(3);

            if (id.HasValue)
            {
                donation = db.Donations.Where(x => x.DonationId == id.Value).SingleOrDefault();


            }
            

            return View(donation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Donation donation)
        {
            if (ModelState.IsValid)
            {
                var uploads = Server.MapPath("~/content/uploads");
                var image = Request.Files["DonationImage"];

                if (image != null && image.ContentLength > 0)
                {
                    image.SaveAs(uploads + "\\" + image.FileName);
                    donation.ImageUrl = "/content/uploads/" + image.FileName;
                }



                if (donation.DonationId == 0)
                {
                    db.Donations.Add(donation);
                    donation.DonatedById = User.Identity.GetUserId();
                }
                else
                {
                    db.Entry(donation).State = System.Data.Entity.EntityState.Modified;
                }

                
                db.SaveChanges();

                return RedirectToAction("Index");
            }


            return View(donation);
        }

    }
}