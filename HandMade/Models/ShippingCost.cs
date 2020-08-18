using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HandMade.Models
{
    public class ShippingCost
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string City { get; set; }
        public bool IsInPerson { get; set; }
        public bool IsInCityShipping { get; set; }
        public double InCityCost { get; set; }
        public bool IsOutCityShipping { get; set; }
        public double OutCityCost { get; set; }

        public virtual Account Account { get; set; }
    }
}