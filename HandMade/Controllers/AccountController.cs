using HandMade.Helprer;
using HandMade.Manager;
using HandMade.Models;
using HandMade.Models.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static HandMade.Manager.CustomeValidations;

namespace HandMade.Controllers
{
    public class AccountController : Controller
    {
        private HandMadeContext _context = new HandMadeContext();
        public MessageScreen messageScreen = new MessageScreen();
        private TokenManagement tokenManagement = new TokenManagement();
        private AuthorizationManagement authorizationManagement = new AuthorizationManagement();

        [CustomAuthorize("Any")]
        // GET: Account
        public ActionResult Index()
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = _context.Accounts.Include("Orders").Include("Reviews").FirstOrDefault(a => a.Email == userEmail);

            if (currentUser.Role == "Seller")
            {
                return RedirectToAction("Seller");
            }

            if (currentUser.Role == "Admin")
            {
                return RedirectToAction("Index", "Admin", null);
            }

            var address = _context.Addresses.Where(a => a.AccountId == currentUser.Id).ToList();
         
            currentUser.Addresses = address;
            return View(currentUser);
        }

        [CustomAuthorize("Any")]
        // GET: Account
        public ActionResult Seller()
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = _context.Accounts.Include("Orders").Include("Reviews").FirstOrDefault(a => a.Email == userEmail);

            if (currentUser.Role == ("Buyer"))
            {
                return RedirectToAction("Index");
            }

            if (currentUser.Role == "Admin")
            {
                return RedirectToAction("Index", "Admin", null);
            }

            var address = _context.ShippingCosts.Where(a => a.AccountId == currentUser.Id).ToList();
            currentUser.ShippingCosts = address;
            return View(currentUser);
        }

        public ActionResult SignIn()
        {
            Models.ViewModel.SignIn signIn = new SignIn {};
            return View(signIn);
        }

        [HttpPost]
        public ActionResult SignIn(SignIn signin)
        {

            Account account = _context.Accounts.FirstOrDefault(a => a.UserName == signin.UserName);

            if (account == null)
            {
                Models.ViewModel.SignIn signIn = new SignIn { Error = "أسم المسخدم المدخل غير صحيح" };
                return View(signIn);

            } else
            {
                bool isUserAuthorised = tokenManagement.IsUserAUthorized(signin.UserName, signin.Password);
                if (isUserAuthorised)
                {

                    var blocked = _context.BannedAccounts.FirstOrDefault(b => b.AccountId == account.Id);

                    if(blocked != null)
                    {
                        Models.ViewModel.SignIn signIn = new SignIn { Error = blocked.Reason };
                        tokenManagement.RemoveCookieValue();

                        return View(signIn);
                    }

                    messageScreen.Typr = "true";
                    messageScreen.Content = "تم تسجيل دخولك بنجاح";
                    messageScreen.Action = "Index";
                    messageScreen.Control = "Home";
                    return RedirectToAction("Index", "Message", messageScreen);
                } else
                {
                    Models.ViewModel.SignIn signIn = new SignIn { Error = "كلمة المرور خاطئة" };
                    return View(signIn);
                }
                
            }
            
        }


        public ActionResult ChangePassword()
        {
            Models.ViewModel.ChangePassword changePassword = new ChangePassword { };
            return View(changePassword);
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePassword changePassword)
        {

            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = _context.Accounts.FirstOrDefault(a => a.Email == userEmail);
            int currentUserId = currentUser.Id;

            if (!ModelState.IsValid)
            {
                return View(changePassword);
            }

            if (changePassword.OldPAssword != currentUser.Password)
            {
                changePassword.Error = "كلمة المرور المدخلة خاطئة";
                return View(changePassword);
            }

            if (changePassword.NewPAssword != changePassword.ConfirmNew)
            {
                changePassword.Error = "كلمة المرور الجديدة لا تتطابق مع نأكيد كلمة المرور";
                return View(changePassword);
            }

            currentUser.Password = changePassword.NewPAssword;
            _context.SaveChanges();

            messageScreen.Typr = "true";
            messageScreen.Content = "تم تغيير كلمة المرور بنجاح";
            messageScreen.Action = "Index";
            messageScreen.Control = "Account";
            return RedirectToAction("Index", "Message", messageScreen);
        }


        public ActionResult SignOut()
        {
            tokenManagement.RemoveCookieValue();

            messageScreen.Typr = "true";
            messageScreen.Content = "تم تسجيل خروجك بنجاح";
            messageScreen.Action = "Index";
            messageScreen.Control = "Home";
            return RedirectToAction("Index", "Message", messageScreen);
        }

        public ActionResult SignUp()
        {
            SignUp account = new SignUp { };
            return View(account);
        }

        [HttpPost]
        public ActionResult SignUp(SignUp account, HttpPostedFileBase ImageFile)
        {
            if(!ModelState.IsValid)
            {
                return View(account);
            }

            string FileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
            string FileExtension = Path.GetExtension(ImageFile.FileName);
            if (FileExtension.ToLower() != ".jpg" && FileExtension.ToLower() != ".png")
            {
                account.Error = "الرجاء إختيار ملف بصيغة png او jpg" ;
                return View(account);
            }

            if (account.Role != "Buyer" && account.Role != "Seller")
            {
                messageScreen.Typr = "false";
                messageScreen.Content = "حدث خطأ أثناء تسجيل الحساب";
                messageScreen.Action = "Index";
                messageScreen.Control = "Home";
                return RedirectToAction("Index", "Message", messageScreen);
            }

            FileName = DateTime.Now.ToString("yyyyMMddhhmmss") + "-" + FileName.Trim() + FileExtension;
            string UploadPath = AppDomain.CurrentDomain.BaseDirectory + "ProfilePicture\\";
            account.ImagePath = UploadPath + FileName;
            ImageFile.SaveAs(account.ImagePath);

            Account realAccount = new Account()
            {
                UserName = account.Email,
                Email = account.Email,
                Password = account.Password,
                FullName = account.FullName,
                ImagePath = FileName,
                PhoneNumber = account.PhoneNumber,
                Role = account.Role
            };

            _context.Accounts.Add(realAccount);
            _context.SaveChanges();


            Account account2 = _context.Accounts.FirstOrDefault(a => a.Email == account.Email);
            bool isUserAuthorised = tokenManagement.IsUserAUthorized(account2.Email, account2.Password);

            if (!isUserAuthorised)
            {
                Models.ViewModel.SignIn signIn = new SignIn { Error = "حدث خطأ أثناء التسجيل" };
                return View(signIn);
            }

            if (account.Role == "Seller")
            {
                return RedirectToAction("AddShippingCost");
            } 
            else
            {
                return RedirectToAction("AddAddress");
            }

           
        }


        public ActionResult EditAccount()
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = _context.Accounts.Include("Orders").Include("Reviews").FirstOrDefault(a => a.Email == userEmail);

            SignUp account = new SignUp { 
            Email = currentUser.Email,
            Password = currentUser.Password,
            PhoneNumber =currentUser.PhoneNumber,
            FullName = currentUser.FullName,
            ImagePath = currentUser.ImagePath,
            Role = currentUser.Role,
            UserName = currentUser.Email
            
            };

            return View(account);
        }

        [HttpPost]
        public ActionResult EditAccount(SignUp account, HttpPostedFileBase ImageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(account);
            }
           
            if (account.Role != "Buyer" && account.Role != "Seller")
            {
                messageScreen.Typr = "false";
                messageScreen.Content = "حدث خطأ أثناء تسجيل الحساب";
                messageScreen.Action = "Index";
                messageScreen.Control = "Home";
                return RedirectToAction("Index", "Message", messageScreen);
            }

          

            Account realAccount = _context.Accounts.FirstOrDefault(a => a.Email == account.UserName);

            realAccount.Email = account.Email;
            realAccount.UserName = account.Email;
            realAccount.Password = account.Password;
            realAccount.FullName = account.FullName;
            realAccount.PhoneNumber = account.PhoneNumber;


            if (ImageFile != null)
            {
                string FileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                string FileExtension = Path.GetExtension(ImageFile.FileName);
                if (FileExtension.ToLower() != ".jpg" && FileExtension.ToLower() != ".png")
                {
                    account.Error = "الرجاء إختيار ملف بصيغة png او jpg";
                    return View(account);
                }

                if (account.Role != "Buyer" && account.Role != "Seller")
                {
                    messageScreen.Typr = "false";
                    messageScreen.Content = "حدث خطأ أثناء تسجيل الحساب";
                    messageScreen.Action = "Index";
                    messageScreen.Control = "Home";
                    return RedirectToAction("Index", "Message", messageScreen);
                }

                FileName = DateTime.Now.ToString("yyyyMMddhhmmss") + "-" + FileName.Trim() + FileExtension;
                string UploadPath = AppDomain.CurrentDomain.BaseDirectory + "ProfilePicture\\";
                account.ImagePath = UploadPath + FileName;
                ImageFile.SaveAs(account.ImagePath);

                realAccount.ImagePath = FileName;

            }

            _context.SaveChanges();

            
                    messageScreen.Typr = "true";
                    messageScreen.Content = "تم تحديث بياناتك بنجاح";
                    messageScreen.Action = "Index";
                    messageScreen.Control = "Account";
                    return RedirectToAction("Index", "Message", messageScreen);
      
           

        }


        

        [CustomAuthorize("Seller")]
        // GET: Account/Create
        public ActionResult EditShippingCost()
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = _context.Accounts.FirstOrDefault(a => a.Email == userEmail);
            int currentUserId = currentUser.Id;

            var model = _context.ShippingCosts.FirstOrDefault(s => s.AccountId == currentUserId);

            if (model == null)
            {
                return RedirectToAction("AddShippingCost");
            }

            return View(model);
        }

        [CustomAuthorize("Seller")]
        // POST: Account/Create
        [HttpPost]
        public ActionResult EditShippingCost(ShippingCost model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var shippingCost = _context.ShippingCosts.FirstOrDefault(s => s.Id == model.Id);

            shippingCost.City = model.City;
            shippingCost.InCityCost = model.InCityCost;
            shippingCost.IsInCityShipping = model.IsInCityShipping;
            shippingCost.OutCityCost = model.OutCityCost;
            shippingCost.IsOutCityShipping = model.IsOutCityShipping;
            shippingCost.IsInPerson = model.IsInPerson;


            _context.SaveChanges();

            messageScreen.Typr = "true";
            messageScreen.Content = "شكرا, تم تعديل التسعيره بنجاح";
            messageScreen.Action = "Index";
            messageScreen.Control = "Product";
            return RedirectToAction("Index", "Account", messageScreen);

        }

        [CustomAuthorize("Seller")]
        // GET: Account/Create
        public ActionResult AddShippingCost()
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = _context.Accounts.FirstOrDefault(a => a.Email == userEmail);
            int currentUserId = currentUser.Id;

            ShippingCost model = new ShippingCost()
            {
                AccountId = currentUserId,
                InCityCost = 0,
                OutCityCost = 0,
                IsInPerson = true,
                IsOutCityShipping = false,
                IsInCityShipping = true,
            };

            return View(model);
        }

        [CustomAuthorize("Seller")]
        // POST: Account/Create
        [HttpPost]
        public ActionResult AddShippingCost(ShippingCost model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _context.ShippingCosts.Add(model);
            _context.SaveChanges();

            messageScreen.Typr = "true";
            messageScreen.Content = "شكرا, تم إكمال تسجيلك بنجاح";
            messageScreen.Action = "Index";
            messageScreen.Control = "Product";
            return RedirectToAction("Index", "Message", messageScreen);

        }

        [CustomAuthorize("Buyer")]
        public ActionResult AddAddress()
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = _context.Accounts.FirstOrDefault(a => a.Email == userEmail);
            int currentUserId = currentUser.Id;

            Address model = new Address()
            {
                AccountId = currentUserId,
                Country = "Saudi Arabia"                
            };

            return View(model);
        }

        [CustomAuthorize("Buyer")]
        // POST: Account/Create
        [HttpPost]
        public ActionResult EditAddress(Address model)
        {
            var oldAddress = _context.Addresses.FirstOrDefault(a => a.AccountId == model.AccountId);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int provinceInInt = int.Parse(model.Provice);
            var provinceName = _context.Regions.FirstOrDefault(m => m.Id == provinceInInt).RegionName;

            model.Provice = provinceName;

            oldAddress.AddressDtails = model.AddressDtails;
            oldAddress.City = model.City;
            oldAddress.Provice = model.Provice;

            _context.SaveChanges();

            messageScreen.Typr = "true";
            messageScreen.Content = "تم تعديل العنوان بنجاح";
            messageScreen.Action = "Index";
            messageScreen.Control = "Product";
            return RedirectToAction("Index", "Message", messageScreen);

        }

        [CustomAuthorize("Buyer")]
        public ActionResult EditAddress()
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = _context.Accounts.FirstOrDefault(a => a.Email == userEmail);
            int currentUserId = currentUser.Id;

            Address model = _context.Addresses.FirstOrDefault(m => m.AccountId == currentUserId);

            return View(model);
        }

        [CustomAuthorize("Buyer")]
        // POST: Account/Create
        [HttpPost]
        public ActionResult AddAddress(Address model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var provinceName = _context.Regions.FirstOrDefault(m => m.Id == Int32.Parse(model.Provice)).RegionName;
            model.Provice = provinceName;

            _context.Addresses.Add(model);
            _context.SaveChanges();

            messageScreen.Typr = "true";
            messageScreen.Content = "شكرا, تم إكمال تسجيلك بنجاح";
            messageScreen.Action = "Index";
            messageScreen.Control = "Product";
            return RedirectToAction("Index", "Message", messageScreen);

        }


        public ActionResult Reviews(int? page)
        {

            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = _context.Accounts.FirstOrDefault(a => a.Email == userEmail);

            ICollection<Review> reviews = _context.Reviews.Include("Product.Account").Where(r => r.AccountId == currentUser.Id).ToList();

            int pageSize = 9;
            int pageNumber = (page ?? 1);

            return View(reviews.ToPagedList(pageNumber, pageSize));
        }

        [CustomAuthorize("Buyer")]
        public ActionResult Qustions(int? page)
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = _context.Accounts.FirstOrDefault(a => a.Email == userEmail);

            ICollection<Question> reviews = _context.Questions.Include("Product.Account").Where(r => r.AccountId == currentUser.Id).ToList();

            int pageSize = 9;
            int pageNumber = (page ?? 1);

            return View(reviews.ToPagedList(pageNumber, pageSize));
        }

        [CustomAuthorize("Buyer")]
        public ActionResult Answers(int? page)
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = _context.Accounts.FirstOrDefault(a => a.Email == userEmail);

            ICollection<Answer> model = _context.Answers.Include("Question.Account").Include("Question.Product").Where(r => r.AccountId == currentUser.Id).ToList();

            int pageSize = 9;
            int pageNumber = (page ?? 1);

            return View(model.ToPagedList(pageNumber, pageSize));
        }

       

        [CustomAuthorize("Buyer")]
        public ActionResult MyOrders()
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = _context.Accounts.FirstOrDefault(a => a.Email == userEmail);

            var myOrders = _context.Orders.Include("Seller").Where(o => o.AccountId == currentUser.Id).ToList();
           
            return View(myOrders);
        }

        [CustomAuthorize("Buyer")]
        public ActionResult OrderDetails(string orderNumber)
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = _context.Accounts.FirstOrDefault(a => a.Email == userEmail);

            var myOrders = _context.Orders.Include("Seller").Include("OrderLines.Product").Include("Account").Where(o => o.Number == orderNumber).FirstOrDefault();

            if (myOrders.AccountId != currentUser.Id)
            {
                return RedirectToAction("Index", "Home", null);
            }
            return View(myOrders);
        }

        [CustomAuthorize("Seller")]
        public ActionResult MyOrdersSeller()
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = _context.Accounts.FirstOrDefault(a => a.Email == userEmail);

            var myOrders = _context.Orders.Include("Seller").Where(o => o.SellerId == currentUser.Id).ToList();

            return View(myOrders);
        }

        [CustomAuthorize("Seller")]
        public ActionResult OrderDetailsSeller(string orderNumber)
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = _context.Accounts.FirstOrDefault(a => a.Email == userEmail);

            var myOrders = _context.Orders.Include("Seller").Include("OrderLines.Product").Include("Account").Where(o => o.Number == orderNumber).FirstOrDefault();

            myOrders.Account = _context.Accounts.FirstOrDefault(a => a.Id == myOrders.AccountId);

            if (myOrders.SellerId != currentUser.Id)
            {
                return RedirectToAction("Index", "Home", null);
            }

            return View(myOrders);
        }

        [CustomAuthorize("Seller")]
        [HttpPost]
        public ActionResult OrderDetailsSeller(string orderNumber, string shippingNumber, string shippingCompany)
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = _context.Accounts.FirstOrDefault(a => a.Email == userEmail);

            var myOrders = _context.Orders.Where(o => o.Number == orderNumber).FirstOrDefault();

            if (myOrders.SellerId != currentUser.Id)
            {
                return RedirectToAction("Index", "Home", null);
            }

            myOrders.Status = "تم الشحن";
            myOrders.ShippingNumber = shippingNumber;
            myOrders.ShippingCompany = shippingCompany;

            _context.SaveChanges();

            return RedirectToAction("OrderDetailsSeller", "Account", new { orderNumber= orderNumber });
        }

        [CustomAuthorize("Buyer")]
        public ActionResult ConfirmReceived(string orderNumber)
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = _context.Accounts.FirstOrDefault(a => a.Email == userEmail);

            var myOrders = _context.Orders.Where(o => o.Number == orderNumber).FirstOrDefault();

            if (myOrders.AccountId != currentUser.Id)
            {
                return RedirectToAction("Index", "Home", null);
            }

            myOrders.Status = "تم الإستلام";
            _context.SaveChanges();

            return RedirectToAction("OrderDetails", "Account", new { orderNumber = orderNumber });
        }
    }
}
