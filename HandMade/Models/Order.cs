using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HandMade.Models
{
    public class Order
    {
        public Order()
        {
            this.OrderDate = DateTime.Now;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Id { get; set; }
        public int AccountId { get; set; }
        [ForeignKey("Account")]
        public int? SellerId { get; set; }
        public string Number { get; set; }
        public double ProductTotal { get; set; }
        public double ServiceTotal { get; set; }
        public double VatTotal { get; set; }
        public double Total { get; set; }
        public string Status { get; set; }
        public string City { get; set; }
        public string Provice { get; set; }
        public string Country { get; set; }
        public string AddressDtails { get; set; }
        public string ShippingCompany { get; set; }
        public string ShippingNumber { get; set; }
        public DateTime OrderDate { get; set; }

        public virtual Account Account { get; set; }
        public virtual Account Seller { get; set; }

        public virtual ICollection<OrderLine> OrderLines { get; set; }
    }
}