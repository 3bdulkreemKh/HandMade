using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HandMade.Models
{
    public class Address
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int Id { get; set; }
        public int AccountId { get; set; }
        public string City { get; set; }
        public string Provice { get; set; }
        public string Country { get; set; }
        public string AddressDtails { get; set; }

        public virtual Account Account { get; set; }
    }
}