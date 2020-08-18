using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HandMade.Models.ViewModel
{
    public class Cart
    {
        public Cart()
        {
            ProductBySellers = new List<ProductBySeller>();
        }

        public double? ItemsTotal { get; set; }
        public double? ShippingTotal { get; set; }
        public double? VATTotal { get; set; }
        public double? Total { get; set; }

        public Address Address { get; set; }

        public ICollection<ProductBySeller> ProductBySellers { get; set; }
    }

    public class ProductBySeller
    {
        public ProductBySeller()
        {
            Products = new List<ShoppingCart>();
        }


        public int? AccountId { get; set; }
        public bool CanBeShipped { get; set; }
        public double ShippingCost { get; set; }
        public double? TotalItemsCost { get; set; }
        public ICollection<ShoppingCart> Products { get; set; }
        public Account Account { get; set; }
    }
}