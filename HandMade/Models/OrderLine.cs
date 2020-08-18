using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HandMade.Models
{
    public class OrderLine
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Qyt { get; set; }
        public double ProductPrice { get; set; }
        public double TotalPrice { get; set; }
        public double VatTotal { get; set; }
        public double Total { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }

    }
}