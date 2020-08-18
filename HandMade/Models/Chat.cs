using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HandMade.Models
{
    public class Chat
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public int? SendToId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime DateTime { get; set; }
        public string Time { get; set; }


        [ForeignKey("AccountId")]
        public virtual Account SendBy { get; set; }

        [ForeignKey("SendToId")]
        public virtual Account SendTo { get; set; }

    }
}