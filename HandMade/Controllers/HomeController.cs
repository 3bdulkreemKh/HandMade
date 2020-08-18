using HandMade.Helprer;
using HandMade.Manager;
using HandMade.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HandMade.Controllers
{
    public class HomeController : Controller
    {
        private HandMadeContext _context = new HandMadeContext();
        public MessageScreen messageScreen = new MessageScreen();
        private TokenManagement tokenManagement = new TokenManagement();
        private AuthorizationManagement authorizationManagement = new AuthorizationManagement();

        [HttpGet]
        public JsonResult getChat()
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = _context.Accounts.FirstOrDefault(a => a.Email == userEmail);

            if (currentUser != null)
            {
                var chats = _context.Chats.Where(s => s.SendToId == currentUser.Id).ToList();

                if (chats != null)
                {
                    var chatModels = new List<ChatModel>();

                    foreach (var chat in chats)
                    {
                        

                        var sendBy = _context.Accounts.FirstOrDefault(a => a.Id == chat.AccountId);
                        var sendTo = _context.Accounts.FirstOrDefault(a => a.Id == chat.SendToId);

                        var chatModel = new ChatModel()
                        {
                            Id = chat.Id,
                            AccountId = chat.AccountId,
                            SendToId = chat.SendToId,
                            Message = chat.Message,
                            IsRead = chat.IsRead,
                            DateTime = chat.DateTime,
                            Time = chat.Time,
                            SendBy = sendBy.FullName,
                            SendTo = sendTo.FullName
                        };

                        chatModels.Add(chatModel);
                    }

                    var result = JsonConvert.SerializeObject(chatModels, Formatting.Indented,
                                  new JsonSerializerSettings
                                  {
                                      ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                  });

                    return Json(result, JsonRequestBehavior.AllowGet);
                }              
            }
            
            return null;
        }

        [HttpPost]
        public JsonResult postChat(Chat chat)
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = _context.Accounts.FirstOrDefault(a => a.Email == userEmail);

            if (string.IsNullOrEmpty(chat.Message))
            {
                return null;
            }

            if (currentUser != null)
            {
                chat.DateTime = DateTime.Now;
                chat.AccountId = currentUser.Id;
                chat.IsRead = true;
                chat.Time = DateTime.Now.ToString("tt hh:mm", CultureInfo.InvariantCulture);
                _context.Chats.Add(chat);
                _context.SaveChanges();                
            }

            return null;
        }

        [HttpPost]
        public JsonResult readChat(int id)
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = _context.Accounts.FirstOrDefault(a => a.Email == userEmail);

            var chat = _context.Chats.FirstOrDefault(c => c.Id == id);
            chat.IsRead = true;
            _context.Chats.Remove(chat);
            _context.SaveChanges();

            return null;
        }

        public ActionResult Index()
        {            
            var r = new Random();
            var rList = new List<Product>();
            var list = _context.Products.ToList();
            int count = 3;

            if (_context.Products.Count() > 3)
            {
                while (count > 0 && _context.Products.Count() > 0)
                {
                    try
                    {
                        var n = r.Next(0, _context.Products.Count());
                        var e = list[n];
                        rList.Add(e);
                        list.RemoveAt(n);
                        count--;
                    }
                    catch
                    {
                    }

                }
            }
           

            return View(rList);
        }

       
    }
}