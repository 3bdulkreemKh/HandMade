using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HandMade.Models.ViewModel
{
    public class SignIn
    {
        [Required(ErrorMessage = "حقل مطلوب")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "حقل مطلوب")]
        public string Password { get; set; }
        public string Error { get; set; }

    }
}