using HandMade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HandMade.Controllers
{
    public class MessageController : Controller
    {
        // GET: Message
        public ActionResult Index(MessageScreen model)
        {
            return View(model);
        }

        public ActionResult OrderConfirmation(MessageScreen model)
        {
            return View(model);
        }
    }
}