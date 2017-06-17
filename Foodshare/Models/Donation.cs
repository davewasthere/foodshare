using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Foodshare.Models
{
    public class Donation
    {
        public int DonationId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableTo { get; set; }

        public string DonatedById { get; set; }
        public virtual ApplicationUser DonatedBy { get; set; }

        public string Contact { get; set; }
        public string Phone { get; set; }

        public string ImageUrl { get; set; }

        public string ClaimedById { get; set; }
        public virtual ApplicationUser ClaimedBy { get; set; }
    }
}