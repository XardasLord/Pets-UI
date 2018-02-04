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

        [HttpPost]
        public async Task<ActionResult> Register(User user)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "There is some problem with the user model. Try again later.";

                return View();
            }

            var url = $"http://www.pets.pawelkowalewicz.pl/users";

            try
            {
                using (var handler = new HttpClientHandler() { CookieContainer = GetCookieContainer() })
                {
                    var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    var response = await MyHttpClient.GetInstance(handler).PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.Message = "There is some problem and the register cannot be done. Try again later.";

                        return View();
                    }
                }
                
            }
            catch (Exception)
            {
                ViewBag.Message = "There is some problem with the external API login system. Try again later.";

                return View();
            }
        }

        private void SetLoggedInInformation(bool isLogged, string email = null)
        {
            Session["LoggedIn"] = isLogged;
            Session["Email"] = email;
        }

        private CookieContainer GetCookieContainer()
        {
            CookieContainer cookieContainer = new CookieContainer();

            if(Session["Cookies"] != null)
            {
                CookieCollection cookies = (CookieCollection)Session["Cookies"];
                cookieContainer.Add(new Uri("http://www.pets.pawelkowalewicz.pl/"), cookies);
            }

            return cookieContainer;
        }
    }
}