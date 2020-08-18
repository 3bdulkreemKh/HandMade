using HandMade.Helprer;
using HandMade.Manager;
using HandMade.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static HandMade.Manager.CustomeValidations;

namespace HandMade.Controllers
{


    [CustomAuthorize("Admin")]
    public class AdminController : Controller
    {
        private HandMadeContext _context = new HandMadeContext();
        public MessageScreen messageScreen = new MessageScreen();
        private TokenManagement tokenManagement = new TokenManagement();
        private AuthorizationManagement authorizationManagement = new AuthorizationManagement();

        // GET: Admin
        public ActionResult Index()
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = _context.Accounts.FirstOrDefault(a => a.Email == userEmail);

            return View(currentUser);
        }

        public ActionResult Orders(int? page)
        {
            var model = _context.Orders.Include("OrderLines").Include("Account").ToList();

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            return View(model.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult OrderDetails(int id)
        {
            var model = _context.Orders.Include("OrderLines").Include("Account").Include("Seller").FirstOrDefault(o => o.Id == id);
            var buyer = _context.Accounts.FirstOrDefault(a => a.Id == model.AccountId);

            model.Account = buyer;

            return View(model);
        }

        [HttpPost]
        public ActionResult Orders(string search, int? page)
        {
            var model = _context.Orders.Include("OrderLines").Include("Account").
                Where(p => p.Account.FullName.Contains(search) ||
                    p.Account.Email.Contains(search) ||
                    p.Account.PhoneNumber.Contains(search) ||
                    p.Number.Contains(search)).ToList();

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            return View(model.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Questions(int? page)
        {
            var model = _context.Questions.Include("Product").Include("Account").Include("Answers").ToList();

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            return View(model.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult Questions(string search, int? page)
        {
            var model = _context.Questions.Include("Product").Include("Account").Include("Answers").
                Where(p => p.Account.FullName.Contains(search) ||
                    p.Account.Email.Contains(search) ||
                    p.Account.PhoneNumber.Contains(search) ||
                    p.Answers.Any(a => a.Content.Contains(search)) ||
                    p.Content.Contains(search)).ToList();

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            return View(model.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult DeleteQuestion(int id)
        {
            var model = _context.Questions.FirstOrDefault(r => r.Id == id);
            _context.Questions.Remove(model);
            _context.SaveChanges();

            messageScreen.Typr = "true";
            messageScreen.Content = "تم حذف السؤال بنجاح";
            messageScreen.Action = "Reviews";
            messageScreen.Control = "Admin";

            return RedirectToAction("Index", "Message", messageScreen);
        }

        public ActionResult DeleteAnswer(int id)
        {
            var model = _context.Answers.FirstOrDefault(r => r.Id == id);
            _context.Answers.Remove(model);
            _context.SaveChanges();

            messageScreen.Typr = "true";
            messageScreen.Content = "تم حذف الجواب بنجاح";
            messageScreen.Action = "Reviews";
            messageScreen.Control = "Admin";

            return RedirectToAction("Index", "Message", messageScreen);
        }

        public ActionResult Reviews(int? page)
        {
            var model = _context.Reviews.Include("Product").Include("Account").ToList();

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            return View(model.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult Reviews(string search, int? page)
        {
            var model = _context.Reviews.Include("Product").Include("Account").
                Where(p => p.Account.FullName.Contains(search) ||
                    p.Account.Email.Contains(search) ||
                    p.Account.PhoneNumber.Contains(search) ||
                    p.Content.Contains(search)).ToList();

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            return View(model.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult DeleteReview(int id)
        {
            var model = _context.Reviews.FirstOrDefault(r => r.Id == id);
            _context.Reviews.Remove(model);
            _context.SaveChanges();

            messageScreen.Typr = "true";
            messageScreen.Content = "تم حذف التقييم بنجاح";
            messageScreen.Action = "Reviews";
            messageScreen.Control = "Admin";

            return RedirectToAction("Index", "Message", messageScreen);
        }

        public ActionResult Products(int? page)
        {
            var product = _context.Products.Include("SubCategory.Category").Include("Pictures").Include("BannedProducts").Include("Account").ToList();

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            return View(product.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult Products(string search, int? page)
        {
            var product = _context.Products.Include("SubCategory.Category").Include("Pictures").Include("BannedProducts").Include("Account")
                .Where(p => p.Name.Contains(search) ||
                    p.SmallDisc.Contains(search) ||
                    p.Discription.Contains(search) ||
                    p.Account.FullName.Contains(search)).ToList();
            
            int pageSize = 20;
            int pageNumber = (page ?? 1);

            return View(product.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult UnBannedAccount(int accountId)
        {
            var bannedAccount = _context.BannedAccounts.FirstOrDefault(a => a.AccountId == accountId);

            if (bannedAccount != null)
            {
                _context.BannedAccounts.Remove(bannedAccount);
                _context.SaveChanges();
            }

            messageScreen.Typr = "true";
            messageScreen.Content = "تم رفع الحضر عن المستخدم بنجاح";
            messageScreen.Action = "Accounts";
            messageScreen.Control = "Admin";

            return RedirectToAction("Index", "Message", messageScreen);

        }

        public ActionResult Accounts(int? page)
        {
            var accounts = _context.Accounts.Where(a => a. Role != "Admin").ToList();

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            return View(accounts.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult BannedAccount(int accountId)
        {
            var bannedAccount = new BannedAccount();

            var account = _context.Accounts.FirstOrDefault(a => a.Id == accountId);

            bannedAccount.AccountId = accountId;
            bannedAccount.Account = account;

            return View(bannedAccount);
        }

        [HttpPost]
        public ActionResult BannedAccount(int accountId, string reason)
        {

            var bannedAccount = new BannedAccount()
            {
                AccountId = accountId,
                Reason = reason
            };

            _context.BannedAccounts.Add(bannedAccount);
            _context.SaveChanges();

            messageScreen.Typr = "true";
            messageScreen.Content = "تم حضر الحساب بنجاح";
            messageScreen.Action = "Products";
            messageScreen.Control = "Admin";

            return RedirectToAction("Index", "Message", messageScreen);

        }

        public ActionResult UnBannedProduct(int productId)
        {
            var bannedAccount = _context.BannedProducts.FirstOrDefault(a => a.ProductId == productId);

            if (bannedAccount != null)
            {
                _context.BannedProducts.Remove(bannedAccount);
                _context.SaveChanges();
            }

            messageScreen.Typr = "true";
            messageScreen.Content = "تم رفع الحضر عن المنتج بنجاح";
            messageScreen.Action = "Accounts";
            messageScreen.Control = "Admin";

            return RedirectToAction("Index", "Message", messageScreen);

        }

        public ActionResult BannedProduct(int productId)
        {
            var bannedAccount = new BannedProducts();

            var product = _context.Products.FirstOrDefault(a => a.Id == productId);

            bannedAccount.ProductId = productId;
            bannedAccount.Product = product;


            return View(bannedAccount);
        }

        [HttpPost]
        public ActionResult BannedProduct(int productId, string reason)
        {

            var bannedProducts = new BannedProducts()
            {
                ProductId = productId,
                Reason = reason
            };

            _context.BannedProducts.Add(bannedProducts);
            _context.SaveChanges();

            messageScreen.Typr = "true";
            messageScreen.Content = "تم حضر المنتج بنجاح";
            messageScreen.Action = "Products";
            messageScreen.Control = "Admin";

            return RedirectToAction("Index", "Message", messageScreen);

        }
        
    }
}