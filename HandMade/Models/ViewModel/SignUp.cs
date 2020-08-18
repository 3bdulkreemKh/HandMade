using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static HandMade.Manager.CustomeValidations;

namespace HandMade.Models.ViewModel
{
    public class SignUp
    {
        [Required(ErrorMessage = "حقل مطلوب")]
        [MinLength(8, ErrorMessage = "يجب أن  تكون كلمة المرور مكونة من 8 أرقام وحروف")]
        public string Password { get; set; }
        [Required(ErrorMessage = "حقل مطلوب")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "حقل مطلوب")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "حقل مطلوب")]
        [EmailAddress(ErrorMessage = "الرجاء إدخال بريد ألكتروني صحيح")]
        [IsEmailExisits]
        public string Email { get; set; }
        [Required(ErrorMessage = "حقل مطلوب")]
        public string Role { get; set; }
        public string UserName { get; set; }
        public string ImagePath { get; set; }
        public string Error { get; set; }

    }
}