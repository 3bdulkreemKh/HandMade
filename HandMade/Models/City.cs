using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HandMade.Models
{
    public class City
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "الرجاء إختيار المدينة")]
        public string CityName { get; set; }
        public int RegionId { get; set; }
        public virtual Region Region { get; set; }
    }
}