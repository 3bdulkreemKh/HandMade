using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HandMade.Models
{
    public class ChatModel
    {
        public int? Id { get; set; }
        public int? AccountId { get; set; }
        public int? SendToId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime DateTime { get; set; }
        public string Time { get; set; }
        public string SendBy { get; set; }
        public string SendTo { get; set; }

    }
}