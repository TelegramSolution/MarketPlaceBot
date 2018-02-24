using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyTelegramBot.Controllers
{
    [Produces("application/json")]
    public class AvailableСitiesController : Controller
    {

        MarketBotDbContext db;
        public IActionResult Index()
        {
            db = new MarketBotDbContext();


            return View(db.AvailableСities.ToList());
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            db = new MarketBotDbContext();

            var city = db.AvailableСities.Where(a => a.Id == id).FirstOrDefault();

            if (city != null)
            {
                db.Remove(city);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Add(string name)
        {
            db = new MarketBotDbContext();

            if (name != null && name != "")
            {
                AvailableСities available = new AvailableСities
                {
                    CityName = name,
                    Timestamp = DateTime.Now
                };

                db.AvailableСities.Add(available);

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}