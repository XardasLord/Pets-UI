using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using Pets_UI.Mvc.Models;

namespace Pets_UI.Mvc.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Logout()
        {
            SetLoggedInInformation(false);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<ActionResult> Login(string email, string password)
        {
            var values = new Dictionary<string, string>
            {
                { "email", email },
                { "password", password }
            };

            try
            {
                using (var handler = new HttpClientHandler())
                {
                    var content = new StringContent(JsonConvert.SerializeObject(values), Encoding.UTF8, "application/json");
                    var response = await MyHttpClient.GetInstance(handler).PostAsync("http://www.pets.pawelkowalewicz.pl/users/login", content);

                    if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Found)
                    {
                        SetLoggedInInformation(true, email);
                        Session["Cookies"] = handler.CookieContainer.GetCookies(new Uri("http://www.pets.pawelkowalewicz.pl/"));

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        SetLoggedInInformation(false);
                        ViewBag.Message = "Incorrect e-mail or password!";

                        return View();
                    }    
                }
                
            }
            catch (Exception)
            {
                SetLoggedInInformation(false);
                ViewBag.Message = "There is some problem with the external API login system. Try again later.";

                return View();
            }
        }

        private void SetLoggedInInformation(bool isLogged, string email = null)
        {
            Session["LoggedIn"] = isLogged;
            Session["Email"] = email;
        }
    }
}