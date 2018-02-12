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

        public new async Task<ActionResult> Profile()
        {
            User user = null;

            try
            {
                user = await GetUserAsync(Session["Email"].ToString());
            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message;
            }

            return View(user);
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

        [HttpPost]
        public new async Task<ActionResult> Profile(User user)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "There is some problem with the user model. Try again later.";

                return View();
            }

            var url = $"http://www.pets.pawelkowalewicz.pl/users/{user.Email}";

            try
            {
                using (var handler = new HttpClientHandler() { CookieContainer = GetCookieContainer() })
                {
                    var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    var response = await MyHttpClient.GetInstance(handler).PutAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        SetLoggedInInformation(true, user.Email);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.Message = "There is some problem and the profile edit cannot be done. Try again later.";

                        return View();
                    }
                }

            }
            catch (Exception)
            {
                ViewBag.Message = "There is some problem with the external API profile editting system. Try again later.";

                return View();
            }
        }

        public async Task<User> GetUserAsync(string email)
        {
            if (email == null)
                throw new Exception("You have to be logged in to see your own animals.");

            User user = null;
            var url = $"http://www.pets.pawelkowalewicz.pl/users/{email}";

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string jsondata = await response.Content.ReadAsStringAsync();
                        user = JsonConvert.DeserializeObject<User>(jsondata);
                    }
                    else
                    {
                        throw new Exception("There is some problem with retrieving your profile information from the external API. Try again later.");
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("There is some problem with the external API. Try again later.");
            }

            return user;
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