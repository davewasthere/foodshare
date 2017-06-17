using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime AvailableFrom { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
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