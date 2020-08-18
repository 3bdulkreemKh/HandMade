using HandMade.Helprer;
using HandMade.Manager;
using HandMade.Models;
using HandMade.Models.ViewModel;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HandMade.Controllers
{
    public class ProductController : Controller
    {
        private HandMadeContext context = new HandMadeContext();
        public MessageScreen messageScreen = new MessageScreen();
        private TokenManagement tokenManagement = new TokenManagement();
        private AuthorizationManagement authorizationManagement = new AuthorizationManagement();

        // GET: Product
        public ActionResult Index(int? page)
        {
            var products = context.Products.Include("Account").Include("SubCategory").Include("Reviews").Include("Pictures").ToList();
            ProductList productList = new ProductList()
            {
                Products = products,
                ProductFilter = new ProductFilter()
                {
                    ByCheaper = "",
                    IsService = "",
                    SearchString = "",
                    ByCategories = new List<Category>()
                }
            };

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            productList.Products.ToPagedList(pageNumber, pageSize);

            return View(productList);
        }

        [HttpPost]
        public ActionResult Index(ProductFilter ProductFilter, int? page)
        {
            var products = context.Products.Include("Account").Include("SubCategory").Include("Reviews").Include("Pictures").ToList();

            if (ProductFilter.SearchString != "" && ProductFilter.SearchString != null)
            {
                var searchVar = ProductFilter.SearchString;
                products = context.Products.Include("Account").Include("SubCategory").Include("Pictures").
                    Where(p => p.Name.Contains(searchVar) ||
                    p.SmallDisc.Contains(searchVar) ||
                    p.Discription.Contains(searchVar) ||
                    p.Account.FullName.Contains(searchVar)).ToList();
            }

            if (ProductFilter.IsService == "true")
            {
                products = products.Where(p => p.IsService == true).ToList();
            } 
            else if (ProductFilter.IsService == "false")
            {
                products = products.Where(p => p.IsService == false).ToList();
            }
            
            if (ProductFilter.ByCheaper == "true")
            {
                products = products.OrderBy(p => p.Price).ToList();
            }
            else if (ProductFilter.ByCheaper == "false")
            {
                products = products.OrderByDescending(p => p.Price).ToList();
            }
            
            if (ProductFilter.Category != null)
            {
                Category category = new Category()
                {
                    CategoryName = ProductFilter.Category
                };
                
                    if (ProductFilter.ByCategories.Count > 0)
                    {
                        Category findCategory = ProductFilter.ByCategories.FirstOrDefault(c => c.CategoryName == category.CategoryName);

                    if (findCategory != null)
                    {
                        ProductFilter.ByCategories.Remove(findCategory);
                    }
                    else
                    {
                        ProductFilter.ByCategories.Add(category);

                        List<Product> tempProducts = new List<Product>();
                        List<Product> tempProducts2 = new List<Product>();
                    }                       
                    }
                    else
                    {
                        ProductFilter.ByCategories.Add(category);                   
                }

               
            }

            if (ProductFilter.ByCategories.Count > 0 )
            {
                List<Product> tempProducts = new List<Product>();
                List<Product> tempProducts2 = new List<Product>();

                foreach (var pr in ProductFilter.ByCategories)
                {
                    tempProducts = products.Where(p => p.SubCategory.Category.CategoryName == pr.CategoryName).ToList();

                    if (tempProducts.Count > 0)
                    {
                        foreach (var prr in tempProducts)
                        {
                            tempProducts2.Add(prr);
                        }
                    }
                };

                products = tempProducts2;
            }

            var productList = new ProductList()
            {
                Products = products.Take(20).ToList(),
                ProductFilter = ProductFilter
            };        

            return View(productList);
        }

        public ActionResult Details(int productId)
        {
            Product product = context.Products.Include("SubCategory.Category").Include("Account").Include("Questions.Account")
                .Include("Questions.Answers.Account").Include("Reviews.Account").Include("Pictures").FirstOrDefault(p => p.Id == productId);
            
            return View(product);
        }

        public ActionResult Reviews(int productId,int? page)
        {
            ICollection<Review> reviews = context.Reviews.Include("Product").Include("Account").Where(r => r.ProductId == productId).ToList();

            int pageSize = 9;
            int pageNumber = (page ?? 1);

            return View(reviews.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult AddReview(int productId)
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = context.Accounts.FirstOrDefault(a => a.Email == userEmail);
            int currentUserId = currentUser.Id;

            var review = new Review()
            {
                Id = 999999,
                AccountId = currentUserId,
                ProductId = productId
            };

            return View(review);
        }

        [HttpPost]
        public ActionResult AddReview(Review review)
        {
           if (!ModelState.IsValid)
            {
                return View(review);
            }

            context.Reviews.Add(review);
            context.SaveChanges();

            messageScreen.Typr = "true";
            messageScreen.Content = "شكرا, تم إرسال تقييمك بنجاح";
            messageScreen.Action = "Index";
            messageScreen.Control = "Product";
            return RedirectToAction("Index", "Message", messageScreen);
        }

        public ActionResult Qustions(int productId, int? page)
        {
            ICollection<Question> qsts = context.Questions.Include("Answers").Include("Account").Where(r => r.ProductId == productId).ToList();

            int pageSize = 9;
            int pageNumber = (page ?? 1);

            return View(qsts.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult AddQustion(int productId)
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = context.Accounts.FirstOrDefault(a => a.Email == userEmail);
            int currentUserId = currentUser.Id;

            var qst = new Question()
            {
                Id = 999999,
                AccountId = currentUserId,
                ProductId = productId                
            };

            return View(qst);
        }

        [HttpPost]
        public ActionResult AddQustion(Question qst)
        {
            if (!ModelState.IsValid)
            {
                return View(qst);
            }

            context.Questions.Add(qst);
            context.SaveChanges();

            messageScreen.Typr = "true";
            messageScreen.Content = "شكرا, تم إرسال سؤالك بنجاح";
            messageScreen.Action = "Index";
            messageScreen.Control = "Product";
            return RedirectToAction("Index", "Message", messageScreen);
        }

        public ActionResult AddAnswer(int qustionId)
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = context.Accounts.FirstOrDefault(a => a.Email == userEmail);
            int currentUserId = currentUser.Id;
            var qstContent = context.Questions.FirstOrDefault(q => q.Id == qustionId).Content;

            var ans = new Answer()
            {
                Id = 999999,
                AccountId = currentUserId,
                QuestionId = qustionId,
                Content = qstContent
            };

            return View(ans);
        }

        [HttpPost]
        public ActionResult AddAnswer(Answer ans)
        {
            if (!ModelState.IsValid)
            {
                return View(ans);
            }

            context.Answers.Add(ans);
            context.SaveChanges();

            messageScreen.Typr = "true";
            messageScreen.Content = "شكرا, تم إرسال جوابك بنجاح";
            messageScreen.Action = "Index";
            messageScreen.Control = "Product";
            return RedirectToAction("Index", "Message", messageScreen);
        }


        public ActionResult AddToCart(int productId)
        {

            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = context.Accounts.FirstOrDefault(a => a.Email == userEmail);

            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Account",null);
            }
            ShoppingCart oldCart = context.ShoppingCarts.FirstOrDefault(s => s.ProductId == productId && s.AccountId == currentUser.Id);

            if(oldCart == null)
            {
                ShoppingCart cart = new ShoppingCart()
                {
                    ProductId = productId,
                    AccountId = currentUser.Id,
                    Qty = 1
                };

                context.ShoppingCarts.Add(cart);
                context.SaveChanges();
            } 
            else
            {
                oldCart.Qty = oldCart.Qty + 1;
                context.SaveChanges();
            }          

            messageScreen.Typr = "true";
            messageScreen.Content = "تمن إظافة المنتج الى سلة التسوق بنجاح";
            messageScreen.Action = "Index";
            messageScreen.Control = "Product";
            return RedirectToAction("Index", "Message", messageScreen);            
        }

        [HttpGet]
        public JsonResult getRegion(int regionId)
        {
            var cities = context.Cities.Where(s => s.RegionId == regionId).ToList();
            var result = JsonConvert.SerializeObject(cities, Formatting.Indented,
                          new JsonSerializerSettings
                          {
                              ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                          });

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}