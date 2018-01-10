using Newtonsoft.Json;
using Pets_UI.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Pets_UI.Mvc.Controllers
{
    public class AnimalController : Controller
    {
        // GET: Animal
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
                throw new Exception("There is some problem with the external API login system. Try again later.");
            }

            return animals;
        }
    }
}