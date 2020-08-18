using HandMade.Manager;
using HandMade.Models;
using HandMade.Models.ViewModel;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static HandMade.Manager.CustomeValidations;

namespace HandMade.Controllers
{
    public class StoreController : Controller
    {
        HandMadeContext context = new HandMadeContext();
        public MessageScreen messageScreen = new MessageScreen();
        private AuthorizationManagement authorizationManagement = new AuthorizationManagement();

        // GET: Store
        public ActionResult Index(int storeId,int? page)
        {
            var products = context.Products.Include("Account").Include("SubCategory").Include("Reviews").Include("Pictures").Where(s => s.Account.Id == storeId).ToList();
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

            var shippingCost = context.ShippingCosts.FirstOrDefault(s => s.AccountId == storeId);

            productList.ShippingCost = shippingCost;

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            productList.Products.ToPagedList(pageNumber, pageSize);

            return View(productList);
        }

        [HttpPost]
        public ActionResult Index(int storeId, ProductFilter ProductFilter, int? page)
        {
            var products = context.Products.Include("Account").Include("SubCategory").Include("Reviews").Include("Pictures").Where(s => s.Id == storeId).ToList();

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

            if (ProductFilter.ByCategories.Count > 0)
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

        [CustomAuthorize("Seller")]
        public ActionResult MyProducts(int? page)
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = context.Accounts.FirstOrDefault(a => a.Email == userEmail);
            int currentUserId = currentUser.Id;

            var product = context.Products.Include("SubCategory.Category").Include("Pictures").Include("Account").Where(p => p.AccountId == currentUserId).ToList();

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            return View(product.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Details(int productId)
        {
            Product product = context.Products.Include("SubCategory.Category").Include("Account").Include("Questions.Account")
                .Include("Questions.Answers.Account").Include("Reviews.Account").Include("Pictures").FirstOrDefault(p => p.Id == productId);

            return View(product);
        }

        [HttpGet]
        public JsonResult getSubCategory(int catId)
        {
           var subCategory = context.SubCategories.Where(s => s.CategoryId == catId).ToList();
            var result = JsonConvert.SerializeObject(subCategory, Formatting.Indented,
                          new JsonSerializerSettings
                          {
                              ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                          });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("Seller")]
        public ActionResult AddProduct()
        {        
            AddedProduct product = new AddedProduct();
            return View(product);
        }

        [CustomAuthorize("Seller")]
        [HttpPost]
        public ActionResult AddProduct(AddedProduct product, HttpPostedFileBase[] ImageFiles)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            if(ImageFiles.Length == 0)
            {
                product.Error = "الرجاء رفع صورة واحدة للمنتج على الأقل";
                return View(product);
            }

            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = context.Accounts.FirstOrDefault(a => a.Email == userEmail);
            int currentUserId = currentUser.Id;

            foreach (HttpPostedFileBase file in ImageFiles)
            {
                string FileName = Path.GetFileNameWithoutExtension(file.FileName);
                string FileExtension = Path.GetExtension(file.FileName);

                if (FileExtension.ToLower() != ".jpg" && FileExtension.ToLower() != ".png")
                {
                    product.Error = "الرجاء إختيار جميع الصور بصيغة png او jpg";
                    return View(product);
                }
            }

            Product realProduct = new Product()
            {
                Name = product.Name,
                AccountId =  currentUserId,
                Price = product.Price,
                Stock = product.Stock,
                SmallDisc = product.SmallDisc,
                DiscountPercentage = product.DiscountPercentage,
                SubCategoryId = product.SubCategoryId,
                Discription = product.Discription,
                IsService = product.IsService
            };

            context.Products.Add(realProduct);
            context.SaveChanges();

            Product lastProduct = context.Products.Where(p => p.AccountId == currentUserId).OrderByDescending(p => p.Id).FirstOrDefault();

            foreach (HttpPostedFileBase file in ImageFiles)
            {
                string FileName = Path.GetFileNameWithoutExtension(file.FileName);
                string FileExtension = Path.GetExtension(file.FileName);

                FileName = DateTime.Now.ToString("yyyyMMddhhmmss") + "-" + FileName.Trim() + FileExtension;
                string UploadPath = AppDomain.CurrentDomain.BaseDirectory + "ProductPicture\\";

                Picture picture = new Picture()
                {
                    ProductId = lastProduct.Id,
                    Path = FileName
                };
               
                file.SaveAs(UploadPath + FileName);

                context.Pictures.Add(picture);
                context.SaveChanges();
            }           
                      
            messageScreen.Typr = "true";
            messageScreen.Content = "تم تسجيل المنتج بنجاح";
            messageScreen.Action = "MyProducts";
            messageScreen.Control = "Store";
            return RedirectToAction("Index", "Message", messageScreen);
        }

        [CustomAuthorize("Seller")]
        public ActionResult EditProduct(int productId)
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = context.Accounts.FirstOrDefault(a => a.Email == userEmail);
            int currentUserId = currentUser.Id;          

            var product = context.Products.FirstOrDefault(p => p.Id == productId);

            if (product.AccountId != currentUserId)
            {
                messageScreen.Typr = "false";
                messageScreen.Content = "فضل تحديث المنتج ";
                return RedirectToAction("Index", "Message", messageScreen);
            }

            AddedProduct addProduct = new AddedProduct()
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                SmallDisc = product.SmallDisc,
                DiscountPercentage = product.DiscountPercentage,
                Discription = product.Discription,
                SubCategoryId = product.SubCategoryId,
                IsService = product.IsService,
                AccountId = product.AccountId
            };
            return View(addProduct);
        }

        [CustomAuthorize("Seller")]
        [HttpPost]
        public ActionResult EditProduct(AddedProduct product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }
           
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = context.Accounts.FirstOrDefault(a => a.Email == userEmail);
            int currentUserId = currentUser.Id;

            Product realProduct = context.Products.FirstOrDefault(p => p.Id == product.Id);

            if (realProduct.AccountId != currentUserId)
            {
                messageScreen.Typr = "false";
                messageScreen.Content = "فضل تحديث المنتج ";
                return RedirectToAction("Index", "Message", messageScreen);
            }

            realProduct.Name = product.Name;
            realProduct.Stock = product.Stock;
            realProduct.SmallDisc = product.SmallDisc;
            realProduct.Price = product.Price;
            realProduct.DiscountPercentage = product.DiscountPercentage;
            realProduct.Discription = product.Discription;
            realProduct.SubCategoryId = product.SubCategoryId;

            context.SaveChanges();

            messageScreen.Typr = "true";
            messageScreen.Content = "تم تحديث المنتج بنجاح";
            return RedirectToAction("Index", "Message", messageScreen);
        }

    }
}