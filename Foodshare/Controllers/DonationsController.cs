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
using Foodshare.Helper;

namespace Foodshare.Controllers
{
    [RequireHttps]

    [Authorize]
    public class DonationsController : Controller
    {
        private FoodshareDbContext db = new FoodshareDbContext();

        // GET: Forum
        public ActionResult Index()
        {
            var yesterday = DateTime.Now.AddDays(-1);

            var userId = User.Identity.GetUserId();

            var phoneNumber = db.Users.Where(x => x.Id == userId).Select(x => x.PhoneNumber).SingleOrDefault();

            if (User.IsInRole("Agency"))
            {
                if (String.IsNullOrWhiteSpace(phoneNumber))
                {
                    return RedirectToAction("EditPhoneNumber", "Manage");
                }
            }

            var verifiedEmail = db.Users.Where(x => x.Id == userId).Select(x => x.EmailConfirmed).SingleOrDefault();

            var items = new List<Donation>();

            if (User.IsInRole("Administrator"))
            {
                items = db.Donations.OrderByDescending(x => x.DonationId).Take(50).ToList();
            }
            else
            {
                if (User.IsInRole("Agency"))
                {
                    items = db.Donations.Where(x => !x.IsDeleted && x.AvailableTo > yesterday).OrderByDescending(x => x.AvailableTo).ToList();
                }
                else
                {
                    items = db.Donations.Where(x => !x.IsDeleted && x.AvailableTo > yesterday).Where(x => x.DonatedById == userId).OrderByDescending(x => x.AvailableTo).ToList();
                }
            }

            ViewBag.UserId = userId;
            ViewBag.EmailConfirmed = verifiedEmail;

            return View(items);
        }

        public ActionResult Edit(int? id)
        {
            var donation = new Donation();

            var userId = User.Identity.GetUserId();

            var prevDonation = db.Donations.Where(x => x.DonatedById == userId).OrderByDescending(x => x.DonationId).FirstOrDefault();

            if (prevDonation != null)
            {
                donation.Location = prevDonation.Location;
                donation.Phone = prevDonation.Phone;
            }

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

                //donation.DonatedBy.PhoneNumber = donation.Phone;
                //db.SaveChanges();

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


        [Authorize(Roles = "Agency")]
        public ActionResult Claim(int id)
        {
            var donation = db.Donations.Where(x => x.DonationId == id).SingleOrDefault();
            var userId = User.Identity.GetUserId();

            ViewBag.UserId = userId;

            return View(donation);
        }

        [Authorize(Roles = "Agency")]
        public ActionResult ConfirmClaim(int id)
        {
            var donation = db.Donations.Where(x => x.DonationId == id).SingleOrDefault();
            var userId = User.Identity.GetUserId();

            if (donation.ClaimedById == null)
            {
                // we can claim it!
                donation.ClaimedById = userId;
                db.SaveChanges();

                var body = "<h1>Great News!</h1>" +
                    "<p>Your donation has been claimed by:</p>" +
                    "<p><b>Email:</b> " + donation.ClaimedBy.Email + "</p>" +
                    "<p><b>Telephone:</b> " + donation.ClaimedBy.Telephone + "</p>" +
                    "";

                // now send notification
                EmailHelper.SendEmail(donation.DonatedBy.Email, "Your donation has been claimed.", body);


                var between = donation.AvailableFrom.ToString("dd MMM yyyy");

                if (donation.AvailableFrom.Date == donation.AvailableTo.Date)
                {
                    between += " " + donation.AvailableFrom.ToString("HH:mm") + " - " + donation.AvailableTo.ToString("HH:mm");
                }
                else
                {
                    between += " " + donation.AvailableFrom.ToString("HH:mm") + " - " + donation.AvailableTo.ToString("dd MMM yyyy") + " " + donation.AvailableTo.ToString("HH:mm");
                }

                body = "<h1>Donation Claimed</h1>" +
                    "<p><b>Description:</b> " + donation.Description + "</p>" +
                    "<p><b>Location:</b> " + donation.Location + "</p>" +
                    "<p><b>Contact Email:</b> " + donation.DonatedBy.Email + "</p>" +
                    "<p><b>Contact Phone:</b> " + donation.Phone + "</p>" +
                    "<p><b>Collection Date/Time:</b> " + between + "</p>" +
                    "";

                EmailHelper.SendEmail(donation.ClaimedBy.Email, "You've claimed this donation.", body);

            }


            return RedirectToAction("Claim", new { id = id });
        }

    }
}