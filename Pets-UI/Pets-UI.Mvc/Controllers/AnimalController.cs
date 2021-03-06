﻿using Newtonsoft.Json;
using Pets_UI.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Pets_UI.Mvc.Controllers
{
    public class AnimalController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Show()
        {
            List<AnimalToCare> animals = null;
            
            try
            {
                animals = await GetAnimalsToCareAsync();
            }
            catch(Exception e)
            {
                ViewBag.Message = e.Message;
            }

            return View(animals);
        }

        [HttpGet]
        public async Task<ActionResult> ShowArchive()
        {
            List<AnimalToCare> animals = null;

            try
            {
                animals = await GetAnimalsToCareAsync(true);
            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message;
            }

            return View(animals);
        }

        [HttpGet]
        public async Task<ActionResult> Details(Guid animalId)
        {
            AnimalToCare animal = null;

            try
            {
                animal = await GetAnimalToCareDetailsAsync(animalId);
            }
            catch(Exception e)
            {
                ViewBag.Message = e.Message;
            }

            return View(animal);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string animalName)
        {
            Animal animal = null;

            try
            {
                animal = await GetAnimalDetailsAsync(animalName);
            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message;
            }

            return View(animal);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Animal animal, FormCollection form)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "There is some problem with the editing animal model. Try again later.";

                return View();
            }
            
            if (Session["Email"] == null)
            {
                ViewBag.Message = "You have to be logged in to edit your own animals.";

                return View();
            }

            var url = $"http://www.pets.pawelkowalewicz.pl/users/{Session["Email"]}/Animals/{form["OldName"]}";

            try
            {
                using (var handler = new HttpClientHandler() { CookieContainer = GetCookieContainer() })
                {
                    var content = new StringContent(JsonConvert.SerializeObject(animal), Encoding.UTF8, "application/json");
                    var response = await MyHttpClient.GetInstance(handler).PutAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("MyAnimals", "Animal");
                    }
                    else
                    {
                        ViewBag.Message = "There is some problem and the animal cannot be edited. Try again later.";

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

        [HttpGet]
        public async Task<ActionResult> AddToCareList(string animalName)
        {
            Animal animal = null;

            try
            {
                animal = await GetAnimalDetailsAsync(animalName);
            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message;
            }

            ViewBag.AnimalId = animal.Id;

            var animalToCare = new AnimalToCare();
            animalToCare.DateFrom = DateTime.Today;
            animalToCare.DateTo = DateTime.Today;

            return View(animalToCare);
        }

        [HttpPost]
        public async Task<ActionResult> AddToCareList(AnimalToCare animal, FormCollection form)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "There is some problem with the animal model. Try again later.";

                return View();
            }

            if (Session["Email"] == null)
            {
                ViewBag.Message = "You have to be logged in to see your own animals.";

                return View();
            }

            var url = $"http://www.pets.pawelkowalewicz.pl/animals_to_care/add";
            var values = new Dictionary<string, string>
            {
                { "animalId", form["AnimalId"]},
                { "dateFrom", animal.DateFrom.ToShortDateString() },
                { "dateTo", animal.DateTo.ToShortDateString() }
            };

            try
            {
                using (var handler = new HttpClientHandler() { CookieContainer = GetCookieContainer() })
                {
                    var content = new StringContent(JsonConvert.SerializeObject(values), Encoding.UTF8, "application/json");
                    var response = await MyHttpClient.GetInstance(handler).PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Show", "Animal");
                    }
                    else
                    {
                        ViewBag.Message = "There is some problem and the animal cannot be added to the care list. Try again later.";

                        return View();
                    }
                }

            }
            catch (Exception)
            {
                ViewBag.Message = "There is some problem with the external API system. Try again later.";

                return View();
            }
        }

        [HttpGet]
        public async Task<ActionResult> TakeToCare(Guid animalId)
        {
            User user = null;

            try
            {
                user = await GetUserAsync(Session["Email"].ToString());
            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message;

                return RedirectToAction("Show", "Animal");
            }

            var url = "http://www.pets.pawelkowalewicz.pl/animals_to_care/care";
            var values = new Dictionary<string, string>
            {
                { "userId", user.Id.ToString()},
                { "animalId", animalId.ToString() }
            };

            try
            {
                using (var handler = new HttpClientHandler() { CookieContainer = GetCookieContainer() })
                {
                    var content = new StringContent(JsonConvert.SerializeObject(values), Encoding.UTF8, "application/json");
                    var response = await MyHttpClient.GetInstance(handler).PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Show", "Animal");
                    }
                    else
                    {
                        ViewBag.Message = "There is some problem and the animal cannot be get to carring. Try again later.";

                        return RedirectToAction("Show", "Animal");
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.Message = "There is some problem with the external API system. Try again later.";

                return RedirectToAction("Show", "Animal");
            }
        }

        [HttpGet]
        public async Task<ActionResult> MyAnimals()
        {
            List<Animal> animals = null;

            try
            {
                animals = await GetMyAnimalsAsync();
            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message;
            }

            return View(animals);
        }

        [HttpGet]
        public async Task<ActionResult> MyTakenAnimals()
        {
            List<AnimalToCare> animals = null;

            try
            {
                animals = await GetMyTakenAnimalsAsync(false);
            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message;
            }

            return View(animals);
        }

        [HttpGet]
        public async Task<ActionResult> MyTakenAnimalsArchive()
        {
            List<AnimalToCare> animals = null;

            try
            {
                animals = await GetMyTakenAnimalsAsync(true);
            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message;
            }

            return View(animals);
        }

        [HttpPost]
        public async Task<ActionResult> Add(Animal animal)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "There is some problem with the animal model. Try again later.";

                return View();
            }

            if (Session["Email"] == null)
            {
                ViewBag.Message = "You have to be logged in to see your own animals.";

                return View();
            }

            var url = $"http://www.pets.pawelkowalewicz.pl/users/{Session["Email"]}/Animals";
            var values = new Dictionary<string, string>
            {
                { "name", animal.Name },
                { "yearOfBirth", animal.YearOfBirth.ToString() }
            };

            try
            {
                using (var handler = new HttpClientHandler() { CookieContainer = GetCookieContainer() })
                {
                    var content = new StringContent(JsonConvert.SerializeObject(values), Encoding.UTF8, "application/json");
                    var response = await MyHttpClient.GetInstance(handler).PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("MyAnimals", "Animal");
                    }
                    else
                    {
                        ViewBag.Message = "There is some problem and the animal cannot be added. Try again later.";

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

        public ActionResult Add()
        {
            return View();
        }
        
        public async Task<ActionResult> Delete(string animalName)
        {
            try
            {
                await DeleteAsync(animalName);
            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message;
            }

            return RedirectToAction("MyAnimals");
        }

        private async Task<List<AnimalToCare>> GetAnimalsToCareAsync(bool archive = false)
        {
            List<AnimalToCare> animals = null;
            var url = "";

            if (archive)
                url = "http://www.pets.pawelkowalewicz.pl/animals_to_care/archive";
            else
                url = "http://www.pets.pawelkowalewicz.pl/animals_to_care";

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Found)
                    {
                        string jsondata = await response.Content.ReadAsStringAsync();
                        animals = JsonConvert.DeserializeObject<List<AnimalToCare>>(jsondata);
                    }
                    else
                    {
                        throw new Exception("There is some problem with retrieving the animals from the external API. Try again later.");
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("There is some problem with the external API. Try again later.");
            }

            return animals;
        }

        private async Task<AnimalToCare> GetAnimalToCareDetailsAsync(Guid animalId)
        {
            AnimalToCare animal = null;
            var url = $"http://www.pets.pawelkowalewicz.pl/animals_to_care/{animalId}";

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string jsondata = await response.Content.ReadAsStringAsync();
                        animal = JsonConvert.DeserializeObject<AnimalToCare>(jsondata.Substring(1, jsondata.Length - 2));
                    }
                    else
                    {
                        throw new Exception("There is some problem with retrieving the animal details from the external API. Try again later.");
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("There is some problem with the external API. Try again later.");
            }

            return animal;
        }

        private async Task<Animal> GetAnimalDetailsAsync(string animalName)
        {
            Animal animal = null;
            var url = $"http://www.pets.pawelkowalewicz.pl/users/{Session["Email"]}/Animals/{animalName}";

            try
            {
                using (var handler = new HttpClientHandler() { CookieContainer = GetCookieContainer() })
                {
                    var response = await MyHttpClient.GetInstance(handler).GetAsync(url);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string jsondata = await response.Content.ReadAsStringAsync();
                        animal = JsonConvert.DeserializeObject<Animal>(jsondata);
                    }
                    else
                    {
                        throw new Exception("There is some problem with retrieving the animal details from the external API. Try again later.");
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("There is some problem with the external API. Try again later.");
            }

            return animal;
        }

        private async Task<List<Animal>> GetMyAnimalsAsync()
        {
            if (Session["Email"] == null)
                throw new Exception("You have to be logged in to see your own animals.");

            List<Animal> animals = null;
            var url = $"http://www.pets.pawelkowalewicz.pl/users/{Session["Email"]}/animals";

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string jsondata = await response.Content.ReadAsStringAsync();
                        animals = JsonConvert.DeserializeObject<List<Animal>>(jsondata);
                    }
                    else
                    {
                        throw new Exception("There is some problem with retrieving your animals from the external API. Try again later.");
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("There is some problem with the external API. Try again later.");
            }

            return animals;
        }

        private async Task<List<AnimalToCare>> GetMyTakenAnimalsAsync(bool archiveAnimals = false)
        {
            if (Session["Email"] == null)
                throw new Exception("You have to be logged in to see your own animals.");

            List<AnimalToCare> animals = null;
            var url = "";

            if(archiveAnimals)
                url = $"http://www.pets.pawelkowalewicz.pl/users/{Session["Email"]}/animals_to_care/archive";
            else
                url = $"http://www.pets.pawelkowalewicz.pl/users/{Session["Email"]}/animals_to_care";


            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string jsondata = await response.Content.ReadAsStringAsync();
                        animals = JsonConvert.DeserializeObject<List<AnimalToCare>>(jsondata);
                    }
                    else
                    {
                        throw new Exception("There is some problem with retrieving your animals taken for care from the external API. Try again later.");
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("There is some problem with the external API. Try again later.");
            }

            return animals;
        }

        private async Task DeleteAsync(string animalName)
        {
            if (Session["Email"] == null)
                throw new Exception("You have to be logged in to delete your own animal from the list.");

            var url = $"http://www.pets.pawelkowalewicz.pl/users/{Session["Email"]}/Animals/{animalName}";

            try
            {
                using (var handler = new HttpClientHandler() { CookieContainer = GetCookieContainer() })
                {
                     var response = await MyHttpClient.GetInstance(handler).DeleteAsync(url);

                     if (!response.IsSuccessStatusCode)
                        throw new Exception("There is some problem with deleting your animals from the list by the external API. Try again later.");
                }
            }
            catch (Exception)
            {
                throw new Exception("There is some problem with the external API. Try again later.");
            }
        }

        private async Task<User> GetUserAsync(string email)
        {
            var url = $"http://www.pets.pawelkowalewicz.pl/users/{email}";

            try
            {
                using (var handler = new HttpClientHandler() { CookieContainer = GetCookieContainer() })
                {
                    var response = await MyHttpClient.GetInstance(handler).GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsondata = await response.Content.ReadAsStringAsync();
                        var user = JsonConvert.DeserializeObject<User>(jsondata);

                        return user;
                    }
                    else
                    {
                        throw new Exception("There is some problem and the animal cannot be get to carring. Try again later.");
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("There is some problem with the external API system. Try again later.");
            }
        }

        private CookieContainer GetCookieContainer()
        {
            CookieContainer cookieContainer = new CookieContainer();

            if (Session["Cookies"] != null)
            {
                CookieCollection cookies = (CookieCollection)Session["Cookies"];
                cookieContainer.Add(new Uri("http://www.pets.pawelkowalewicz.pl/"), cookies);
            }

            return cookieContainer;
        }
    }
}