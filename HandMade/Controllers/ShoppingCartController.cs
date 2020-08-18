using HandMade.Helprer;
using HandMade.Manager;
using HandMade.Models;
using HandMade.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using static HandMade.Manager.CustomeValidations;

namespace HandMade.Controllers
{
    [CustomAuthorize("Buyer")]
    public class ShoppingCartController : Controller
    {
        private HandMadeContext context = new HandMadeContext();
        public MessageScreen messageScreen = new MessageScreen();
        private TokenManagement tokenManagement = new TokenManagement();
        private AuthorizationManagement authorizationManagement = new AuthorizationManagement();

        // GET: ShoppingCart
        public ActionResult Index()
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = context.Accounts.FirstOrDefault(a => a.Email == userEmail);
            int currentUserId = currentUser.Id;

            var shoppingCart = context.ShoppingCarts.Include("Product").Where(s => s.AccountId == currentUserId).ToList();

            Address address = context.Addresses.FirstOrDefault(a => a.AccountId == currentUserId);

            if (address == null)
            {
                return RedirectToAction("AddAddress", "Account", null);
            }

            Cart cart = new Cart();

            int? storeId = 0;

            double? ItemsTotal = 0;
            double? ShippingTotal = 0;

            foreach(var shoppingCa in shoppingCart.ToList())
            {
                storeId = shoppingCa.Product.AccountId;

                ShippingCost sC = context.ShippingCosts.FirstOrDefault(s => s.AccountId == storeId);
                Account accountt = context.Accounts.FirstOrDefault(a => a.Id == storeId);

                bool canBeShippedTo = false;
                double shippingCost = 0;

                if (address.City == sC.City && sC.IsInCityShipping == true)
                {
                    canBeShippedTo = true;
                    shippingCost = sC.InCityCost;
                }

                if (address.City != sC.City && sC.IsOutCityShipping == true)
                {
                    canBeShippedTo = true;
                    shippingCost = sC.OutCityCost;
                }

                ProductBySeller bySeller = new ProductBySeller()
                {
                    AccountId = storeId,
                    CanBeShipped = canBeShippedTo,
                    ShippingCost = shippingCost,
                    Account = accountt
                };

                foreach (var sCa in shoppingCart.Where(s => s.Product.AccountId == storeId).ToList())
                {                   
                    double? itemsTotal = 0;

                    var prod = sCa.Product;

                    double? productRealPrice = prod.Price;

                    if (prod.DiscountPercentage != null && prod.DiscountPercentage != 0)
                    {
                        productRealPrice = productRealPrice - (productRealPrice * prod.DiscountPercentage / 100);
                    }

                    productRealPrice = productRealPrice * sCa.Qty;

                    itemsTotal = itemsTotal + productRealPrice;

                    if (canBeShippedTo == true)
                    {
                        ItemsTotal = ItemsTotal + productRealPrice;
                    }


                    bySeller.Products.Add(sCa);

                    shoppingCart.Remove(sCa);

                }

                if (bySeller.Products.Count() > 0)
                {
                    ShippingTotal = ShippingTotal + shippingCost;
                    cart.ProductBySellers.Add(bySeller);
                }

            }

            cart.ItemsTotal = ItemsTotal;
            cart.ShippingTotal = ShippingTotal;
            cart.VATTotal = ItemsTotal * 0.05;
            cart.Total = cart.VATTotal + ItemsTotal + cart.ShippingTotal;
            cart.Address = address;

            return View(cart);
        }

        public ActionResult AddOne(int shoppingCartId)
        {
            var shoppingCart = context.ShoppingCarts.Include("Product").Where(s => s.Id == shoppingCartId).FirstOrDefault();
            var product = context.Products.FirstOrDefault(p => p.Id == shoppingCart.ProductId);

            if (product.Stock - shoppingCart.Qty < 1)
            {
                messageScreen.Typr = "false";
                messageScreen.Content = "الحد المتوفر من المنتج هو " + product.Stock;
                messageScreen.Action = "Index";
                messageScreen.Control = "ShoppingCart";
                return RedirectToAction("Index", "Message", messageScreen);
            }

            if (shoppingCart == null)
            {
                string userEmail2 = authorizationManagement.IsUserLogedIn();
                Account currentUser2 = context.Accounts.FirstOrDefault(a => a.Email == userEmail2);
                int currentUserId2 = currentUser2.Id;

                var shoppingCart22 = context.ShoppingCarts.Include("Product").Where(s => s.AccountId == currentUserId2).ToList();
                return RedirectToAction("Index", shoppingCart22);
            }

            shoppingCart.Qty = shoppingCart.Qty + 1;

            context.SaveChanges();

            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = context.Accounts.FirstOrDefault(a => a.Email == userEmail);
            int currentUserId = currentUser.Id;

            var shoppingCart2 = context.ShoppingCarts.Include("Product").Where(s => s.AccountId == currentUserId).ToList();
            return RedirectToAction("Index",shoppingCart2);

        }

        public ActionResult RemoveOne(int shoppingCartId)
        {
            var shoppingCart = context.ShoppingCarts.Include("Product").Where(s => s.Id == shoppingCartId).FirstOrDefault();

            if (shoppingCart == null)
            {
                string userEmail2 = authorizationManagement.IsUserLogedIn();
                Account currentUser2 = context.Accounts.FirstOrDefault(a => a.Email == userEmail2);
                int currentUserId2 = currentUser2.Id;

                var shoppingCart22 = context.ShoppingCarts.Include("Product").Where(s => s.AccountId == currentUserId2).ToList();
                return RedirectToAction("Index", shoppingCart22);
            }

            shoppingCart.Qty = shoppingCart.Qty - 1;

            if (shoppingCart.Qty <= 0)
            {
                context.ShoppingCarts.Remove(shoppingCart);
            }

            context.SaveChanges();

            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = context.Accounts.FirstOrDefault(a => a.Email == userEmail);
            int currentUserId = currentUser.Id;

            var shoppingCart2 = context.ShoppingCarts.Include("Product").Where(s => s.AccountId == currentUserId).ToList();
            return RedirectToAction("Index", shoppingCart2);
        }

        public ActionResult RemoveItem(int shoppingCartId)
        {
            var shoppingCart = context.ShoppingCarts.Include("Product").Where(s => s.Id == shoppingCartId).FirstOrDefault();

            if (shoppingCart == null)
            {
                string userEmail2 = authorizationManagement.IsUserLogedIn();
                Account currentUser2 = context.Accounts.FirstOrDefault(a => a.Email == userEmail2);
                int currentUserId2 = currentUser2.Id;

                var shoppingCart22 = context.ShoppingCarts.Include("Product").Where(s => s.AccountId == currentUserId2).ToList();
                return RedirectToAction("Index", shoppingCart22);
            }
                context.ShoppingCarts.Remove(shoppingCart);
            
            context.SaveChanges();

            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = context.Accounts.FirstOrDefault(a => a.Email == userEmail);
            int currentUserId = currentUser.Id;

            var shoppingCart2 = context.ShoppingCarts.Include("Product").Where(s => s.AccountId == currentUserId).ToList();
            return RedirectToAction("Index", shoppingCart2);

        }

        public ActionResult ConfirmOrder()
        {
            string userEmail = authorizationManagement.IsUserLogedIn();
            Account currentUser = context.Accounts.FirstOrDefault(a => a.Email == userEmail);
            int currentUserId = currentUser.Id;

            var orderLines = new List<OrderLine>();
            
            var shoppingCart = context.ShoppingCarts.Include("Product").Where(s => s.AccountId == currentUserId).ToList();

            Address address = context.Addresses.FirstOrDefault(a => a.AccountId == currentUserId);

            if (address == null)
            {
                return RedirectToAction("AddAddress", "Account", null);
            }

            Cart cart = new Cart();

            int? storeId = 0;

            double? ItemsTotal = 0;
            double? ShippingTotal = 0;

            foreach (var shoppingCa in shoppingCart.ToList())
            {
                storeId = shoppingCa.Product.AccountId;

                ShippingCost sC = context.ShippingCosts.FirstOrDefault(s => s.AccountId == storeId);
                Account accountt = context.Accounts.FirstOrDefault(a => a.Id == storeId);

                bool canBeShippedTo = false;
                double shippingCost = 0;
                double? totalForStore = 0;

                if (address.City == sC.City && sC.IsInCityShipping == true)
                {
                    canBeShippedTo = true;
                    shippingCost = sC.InCityCost;
                }

                if (address.City != sC.City && sC.IsOutCityShipping == true)
                {
                    canBeShippedTo = true;
                    shippingCost = sC.OutCityCost;
                }

                ProductBySeller bySeller = new ProductBySeller()
                {
                    AccountId = storeId,
                    CanBeShipped = canBeShippedTo,
                    ShippingCost = shippingCost,
                    Account = accountt
                };

                foreach (var sCa in shoppingCart.Where(s => s.Product.AccountId == storeId).ToList())
                {
                    double? itemsTotal = 0;

                    var prod = sCa.Product;

                    double? productRealPrice = prod.Price;

                    if (prod.DiscountPercentage != null && prod.DiscountPercentage != 0)
                    {
                        productRealPrice = productRealPrice - (productRealPrice * prod.DiscountPercentage / 100);
                    }

                    var productSinglePrice = productRealPrice;
                    productRealPrice = productRealPrice * sCa.Qty;

                    itemsTotal = itemsTotal + productRealPrice;

                    if (canBeShippedTo == true)
                    {
                        OrderLine orderline = new OrderLine()
                        {
                            ProductId = sCa.Product.Id,
                            Qyt = sCa.Qty
                        };

                        var reducedProduct = context.Products.FirstOrDefault(p => p.Id == prod.Id);

                        reducedProduct.Stock = reducedProduct.Stock - sCa.Qty;

                        context.SaveChanges();

                        orderline.ProductPrice = (productSinglePrice.HasValue) ? productSinglePrice.Value : 0;
                        orderline.TotalPrice = (productRealPrice.HasValue) ? productRealPrice.Value : 0;
                        orderline.VatTotal = ((productRealPrice * 0.05).HasValue) ? (productRealPrice * 0.05).Value : 0;
                        orderline.Total = (((productRealPrice * 0.05) + productRealPrice).HasValue) ? ((productRealPrice * 0.05) + productRealPrice).Value : 0;

                        orderLines.Add(orderline);

                        totalForStore = totalForStore + productRealPrice;

                        ItemsTotal = ItemsTotal + productRealPrice;
                    }


                    bySeller.Products.Add(sCa);

                    shoppingCart.Remove(sCa);

                }

                if (bySeller.Products.Count() > 0)
                {
                    ShippingTotal = ShippingTotal + shippingCost;
                    cart.ProductBySellers.Add(bySeller);
                    
                    if (bySeller.CanBeShipped == true)
                    {
                        var order = new Order();

                        order.SellerId = storeId;
                        order.AccountId = currentUserId;
                        order.ProductTotal = (totalForStore.HasValue) ? totalForStore.Value : 0;
                        order.ServiceTotal = (ShippingTotal.HasValue) ? ShippingTotal.Value : 0;
                        order.VatTotal = ((totalForStore * 0.05).HasValue) ? (totalForStore * 0.05).Value : 0;
                        order.Total = ((order.VatTotal + totalForStore + order.ServiceTotal).HasValue) ? (order.VatTotal + totalForStore + order.ServiceTotal).Value : 0;
                        order.City = address.City;
                        order.Provice = address.Provice;
                        order.Country = address.Country;
                        order.AddressDtails = address.AddressDtails;
                        order.Status = "جديد";
                        order.Number = "OR" + DateTime.Now.Second.ToString() + Get8Digits();

                        context.Orders.Add(order);
                        context.SaveChanges();

                        var savedOrder = context.Orders.FirstOrDefault(o => o.Number == order.Number);

                        foreach (var ord in orderLines)
                        {
                            ord.OrderId = savedOrder.Id;
                            context.OrderLines.Add(ord);
                        }

                        context.SaveChanges();
                    }
                  
                }

                

            }



            var inDbCart = context.ShoppingCarts.Where(s => s.AccountId == currentUserId).ToList();

            foreach (var sCart in inDbCart)
            {
                var willBeRemovedCart = context.ShoppingCarts.FirstOrDefault(s => s.Id == sCart.Id);
                context.ShoppingCarts.Remove(willBeRemovedCart);
            }

            context.SaveChanges();

            messageScreen.Typr = "true";
            messageScreen.Content = "تم إنشاء طلبك بنجاح";
            messageScreen.Extra = "الرجاء الذهاب لصفحة الطلبات الخاصة بك لمتابعة طلباتك";
            return RedirectToAction("OrderConfirmation", "Message", messageScreen);
        }

        public string Get8Digits()
        {
            var bytes = new byte[4];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            uint random = BitConverter.ToUInt32(bytes, 0) % 100000000;
            return String.Format("{0:D8}", random);
        }
    }


}