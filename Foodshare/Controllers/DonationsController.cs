using Foodshare.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Foodshare.Models;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Net.Mail;
using System.Configuration;

namespace Foodshare.Controllers
{
    [Authorize]
    public class DonationsController : Controller
    {
        private FoodshareDbContext db = new FoodshareDbContext();

        // GET: Forum
        public ActionResult Index()
        {
            var yesterday = DateTime.Now.AddDays(-1);

            var userId = User.Identity.GetUserId();

            var verifiedEmail = db.Users.Where(x => x.Id == userId).Select(x => x.EmailConfirmed).Single();

            var items = new List<Donation>();

            if (User.IsInRole("Agency"))
            {
                items = db.Donations.Where(x => !x.IsDeleted && x.AvailableTo > yesterday).OrderByDescending(x => x.AvailableTo).ToList();
            }
            else
            {
                items = db.Donations.Where(x => !x.IsDeleted && x.AvailableTo > yesterday).Where(x => x.DonatedById == userId).OrderByDescending(x => x.AvailableTo).ToList();
            }

            ViewBag.UserId = userId;
            ViewBag.EmailConfirmed = verifiedEmail;

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
                var sendEmail = false;


                var uploads = Server.MapPath("~/content/uploads");
                var image = Request.Files["DonationImage"];

                if (image != null && image.ContentLength > 0)
                {
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }

                    image.SaveAs(uploads + "\\" + image.FileName);
                    donation.ImageUrl = "/content/uploads/" + image.FileName;
                }



                if (donation.DonationId == 0)
                {
                    db.Donations.Add(donation);
                    donation.DonatedById = User.Identity.GetUserId();
                    sendEmail = true;
                }
                else
                {
                    db.Entry(donation).State = System.Data.Entity.EntityState.Modified;
                }

                
                db.SaveChanges();

                if (sendEmail)
                {
                    var message = new MailMessage();

                    message.To.Add(ConfigurationManager.AppSettings["donationsEmail"]);

                    var agencyRole = db.Roles.Where(x => x.Name == "Agency").SingleOrDefault();

                    if (agencyRole != null)
                    {
                        var userIds = agencyRole.Users.Select(x => x.UserId).ToList();

                        var users = db.Users.Where(x => userIds.Contains(x.Id) && x.EmailConfirmed).Select(x => x.Email).ToList();

                        foreach (var user in users)
                        { 
                            message.Bcc.Add(new MailAddress(user));
                        }

                        
                    }

                    var domainName = Request.Url.GetLeftPart(UriPartial.Authority);


                    message.Subject = "New Donation Available: " + donation.Title;
                    message.Body = "A new donation has been added at " + domainName  + "\r\n\r\nTitle: " + donation.Title + "\r\nDescription: " + donation.Description + "\r\nLocation: " + donation.Location;

                    using (var smtpClient = new SmtpClient())
                    {
                        try
                        {
                            // I've learnt my lesson with public repos. ;)
                            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["emailUsername"], ConfigurationManager.AppSettings["emailPassword"]);
                            smtpClient.Credentials = credentials;

                            smtpClient.Send(message);
                        }
                        catch { }
                    }
                }


                return RedirectToAction("Index");
            }


            return View(donation);
        }



        public ActionResult Claim(int id)
        {
            var donation = db.Donations.Where(x => x.DonationId == id).SingleOrDefault();
            var userId = User.Identity.GetUserId();

            ViewBag.UserId = userId;

            return View(donation);
        }

        public ActionResult ConfirmClaim(int id)
        {
            var donation = db.Donations.Where(x => x.DonationId == id).SingleOrDefault();
            var userId = User.Identity.GetUserId();

            if (donation.ClaimedById == null)
            {
                // we can claim it!
                donation.ClaimedById = userId;
                db.SaveChanges();
            }


            return RedirectToAction("Claim", new { id = id });
        }

    }
}