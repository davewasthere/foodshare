using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Foodshare.Models
{
    public class FoodItem
    {
        public int FoodItemId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableTo { get; set; }

        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

        public string Contact { get; set; }
        public string Phone { get; set; }

        public string ImageUrl { get; set; }
    }
}