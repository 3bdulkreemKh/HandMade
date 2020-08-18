using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HandMade.Models
{
    public class Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? AccountId { get; set; }
       
        public string Name { get; set; }
        public double Price { get; set; }
        public int? DiscountPercentage { get; set; }
        public int? SubCategoryId { get; set; }
        public string Discription { get; set; }
        public string SmallDisc { get; set; }
        public bool IsService { get; set; }
        public int Stock { get; set; }
        public virtual SubCategory SubCategory { get; set; }
        public virtual Account Account { get; set; }
       
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Picture> Pictures { get; set; }
        public virtual ICollection<BannedProducts> BannedProducts { get; set; }

    }

    public class Picture
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public string Path { get; set; }
    }

    public class Review
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int Id { get; set; }
        public int AccountId { get; set; }
        public int? ProductId { get; set; }
        public int Rating { get; set; }
        public string Content { get; set; }
        public virtual Account Account { get; set; }
        public virtual Product Product { get; set; }
    }

    public class Question
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int Id { get; set; }
        public int AccountId { get; set; }
        public int? ProductId { get; set; }
        public string Content { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual Account Account { get; set; }
        public virtual Product Product { get; set; }

    }

    public class Answer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public int? QuestionId { get; set; }
        public string Content { get; set; }
        public virtual Account Account { get; set; }
        public virtual Question Question { get; set; }
    }
}