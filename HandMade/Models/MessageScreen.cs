using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HandMade.Models
{
    public class MessageScreen
    {
        public string Typr { get; set; }
        public string Content { get; set; }
        public string Extra { get; set; }

        public string Action { get; set; }
        public string Control { get; set; }
        public string Route { get; set; }
    }
}