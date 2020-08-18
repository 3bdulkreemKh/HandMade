using HandMade.Helprer;
using HandMade.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HandMade.Manager
{
    public class CustomeValidations
    {
        public class CustomAuthorizeAttribute : AuthorizeAttribute
        {
            private readonly string[] allowedroles;
            private HandMadeContext context = new HandMadeContext();
          

            public CustomAuthorizeAttribute(params string[] roles)
            {
                this.allowedroles = roles;
            }
            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                bool authorize = false;
                string userName = "";
              

                TokenManagement tokenManagement = new TokenManagement();

                string token = tokenManagement.GetCookieValue();

                if (token == null || token == "") return false;

                if (tokenManagement.GetClaimValueFromToken("User", token) != null)
                {
                    userName = tokenManagement.GetClaimValueFromToken("User", token);
                }

                if (!string.IsNullOrEmpty(userName))
                {
                    Account account = context.Accounts.FirstOrDefault(a => a.Email == userName);

                    if (account == null) return false;
                    if (account.Token != token) return false;                   

                    
                    foreach (var role in allowedroles)
                    {
                        if (role == "Any") return true;
                        if (role == account.Role) return true;
                    }

                }

                return authorize;
            }

            protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
            {
                filterContext.Result = new RedirectToRouteResult(
                   new RouteValueDictionary
                   {
                    { "controller", "Home" },
                    { "action", "UnAuthorized" }
                   });
            }
        }

        public class IsEmailExisits : ValidationAttribute
        {
            private HandMadeContext context = new HandMadeContext();
            public MessageScreen messageScreen = new MessageScreen();
            private TokenManagement tokenManagement = new TokenManagement();
            private AuthorizationManagement authorizationManagement = new AuthorizationManagement();

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {

                string userEmail = authorizationManagement.IsUserLogedIn();
                Account currentUser = context.Accounts.Include("Orders").Include("Reviews").FirstOrDefault(a => a.Email == userEmail);
                string email = value.ToString().ToLower();

                    Account account = context.Accounts.FirstOrDefault(a => a.Email == email);
                    if (account == null)
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                            if (account.Email == userEmail)
                            {
                                return ValidationResult.Success;
                            }
                    return new ValidationResult("يوجد حساب بنفس البريد الإلكتروني");
                    }
               
            }
        }

        
        }
}