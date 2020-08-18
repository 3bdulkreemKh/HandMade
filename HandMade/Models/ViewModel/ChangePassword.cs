using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HandMade.Models.ViewModel
{
    public class ChangePassword
    {
        public string OldPAssword { get; set; }
        [MinLength(8, ErrorMessage = "يجب أن  تكون كلمة المرور مكونة من 8 أرقام وحروف")]
        public string NewPAssword { get; set; }
        [MinLength(8, ErrorMessage = "يجب أن  تكون كلمة المرور مكونة من 8 أرقام وحروف")]
        public string ConfirmNew { get; set; }
        public string Error { get; set; }

    }
}