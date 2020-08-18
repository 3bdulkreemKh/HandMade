using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HandMade.Models
{
    public class Account
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserName { get; set; }
        [MinLength(8, ErrorMessage = "يجب أن  تكون كلمة المرور مكونة من 8 أرقام وحروف")]
        public string Password { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        [EmailAddress(ErrorMessage = "الرجاء إدخال بريد ألكتروني صحيح")]
        public string Email { get; set; }
        public string Role { get; set; }
        public string ImagePath { get; set; }
        public string Token { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<ShippingCost> ShippingCosts { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<BannedAccount> BannedAccounts { get; set; }

    }
}