using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HandMade.Models.ViewModel
{
    public class ProductList
    {
        public ICollection<Product> Products { get; set; }
        public ProductFilter ProductFilter { get; set; }
        public ShippingCost ShippingCost { get; set; }
    }

    public class ProductFilter
    {
        public ProductFilter()
        {
            ByCategories = new List<Category>();
        }

        public string SearchString { get; set; }
        public string ByCheaper { get; set; }
        public string IsService { get; set; }
        public string Category { get; set; }

        public List<Category> ByCategories { get; set; }       
}
}