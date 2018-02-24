using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyTelegramBot.Controllers
{
    public class PickupPointController : Controller
    {
        MarketBotDbContext db;

        public IActionResult Index()
        {
            db = new MarketBotDbContext();

            return View(db.PickupPoint.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            PickupPoint pickupPoint = new PickupPoint
            {
                Enable = true,
                Name = ""
            };

            return View("Editor", pickupPoint);
        }

        public IActionResult Editor(int id)
        {
            db = new MarketBotDbContext();

            if (id > 0)
            {
                var pickupPoint = db.PickupPoint.Find(id);

                if (pickupPoint != null)
                    return View(pickupPoint);

                else
                    return NoContent();
            }

            else
                return NotFound();
        }

        [HttpPost]
        public IActionResult Save(PickupPoint pickupPoint)
        {
            db = new MarketBotDbContext();

            //добавление нового
            if(pickupPoint!=null && pickupPoint.Name!=null && pickupPoint.Id < 1)
            {
                db.PickupPoint.Add(pickupPoint);

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            //редактирование

            if (pickupPoint != null && pickupPoint.Name != null && pickupPoint.Id > 0)
            {
                //var old = db.PickupPoint.Find(pickupPoint.Id);

                db.PickupPoint.Update(pickupPoint);

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            else
                return NotFound();

        }
    }
}