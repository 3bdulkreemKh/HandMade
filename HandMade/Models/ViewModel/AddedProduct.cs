using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HandMade.Models.ViewModel
{
    public class AddedProduct
    {
        public int? Id { get; set; }
        public int? AccountId { get; set; }
        public string Name { get; set; }
        public string SmallDisc { get; set; }
        public int Stock { get; set; }
        public double Price { get; set; }
        public int? DiscountPercentage { get; set; }
        public int? SubCategoryId { get; set; }
        public string Discription { get; set; }
        public bool IsService { get; set; }
        public string Error { get; set; }
    }
}