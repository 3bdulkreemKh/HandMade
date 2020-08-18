using HandMade.Models;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace HandMade.Helprer
{   
    public class TokenManagement
    {
        private HandMadeContext context = new HandMadeContext();

        public string generatToken(string userName) {

            
            int ExpiringHours = 240;

            var claim = new[]
            {
                new Claim("User", userName),
                new Claim("Expires", DateTime.Now.AddHours(ExpiringHours).ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["Secret"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                null,
                null,
                claim,
                expires: DateTime.Now.AddHours(ExpiringHours),
                signingCredentials: credentials
            );
            string token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            bool saveCookie = SaveToCookie("TokenValue", token);

            Account account = context.Accounts.FirstOrDefault(a => a.UserName == userName);

            account.Token = token;
            context.SaveChanges();

            return token.ToString();        

        }

        public ClaimsPrincipal ValidateToken(string jwtToken)
        {
            IdentityModelEventSource.ShowPII = true;

            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();

            validationParameters.ValidateLifetime = true;

            validationParameters.ValidateAudience = false;
            validationParameters.ValidateIssuer = false;
            validationParameters.IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["Secret"]));

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);


            return principal;
        }

        public string GetClaimValueFromToken(string claim, string token)
        {
            return ValidateToken(token)?.FindFirst(claim)?.Value;
        }

        public bool IsUserAUthorized(string userName, string Password)
        {
            Account account = context.Accounts.FirstOrDefault(a => a.UserName == userName && a.Password == Password);

            if (account== null)
            {
                return false;
            }

            string token = generatToken(userName);
            

            return true;
        }

        public bool SaveToCookie(string cookie, string value)
        {
            try
            {
                if (HttpContext.Current.Request.Cookies.AllKeys.Contains(cookie))
                {
                    HttpContext.Current.Response.Cookies[cookie].Value = value;
                    HttpContext.Current.Response.Cookies[cookie].Expires = DateTime.Now.AddDays(30);
                    return true;
                }
                else if (!HttpContext.Current.Request.Cookies.AllKeys.Contains(cookie))
                {
                    HttpContext.Current.Response.Cookies[cookie].Value = value;
                    HttpContext.Current.Response.Cookies[cookie].Expires = DateTime.Now.AddDays(30);
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        public string GetCookieValue()
        {
            try
            {
                if (HttpContext.Current.Request.Cookies.AllKeys.Contains("TokenValue"))
                {
                    return HttpContext.Current.Request.Cookies["TokenValue"].Value;
                }
                else if (!HttpContext.Current.Request.Cookies.AllKeys.Contains("TokenValue"))
                {
                    HttpContext.Current.Response.Cookies["TokenValue"].Value = "";
                    HttpContext.Current.Response.Cookies["TokenValue"].Expires = DateTime.Now.AddDays(30);

                    return "";
                }

                return "";
            }
            catch
            {
                return "";
            }
        }

        public bool RemoveCookieValue()
        {
            try
            {
                if (HttpContext.Current.Request.Cookies.AllKeys.Contains("TokenValue"))
                {
                    HttpContext.Current.Response.Cookies["TokenValue"].Value = "";
                    return true;
                }
                else if (!HttpContext.Current.Request.Cookies.AllKeys.Contains("TokenValue"))
                {
                    HttpContext.Current.Response.Cookies["TokenValue"].Value = "";
                    HttpContext.Current.Response.Cookies["TokenValue"].Expires = DateTime.Now.AddDays(30);

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}