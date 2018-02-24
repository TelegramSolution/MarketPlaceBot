using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyTelegramBot.Controllers
{
    public class UsersController : Controller
    {
        MarketBotDbContext db;

        public IActionResult Index()
        {
            db = new MarketBotDbContext();

            var list = db.Follower.ToList();

            return View(list);
        }

        [HttpGet]
        public IActionResult Blocked(int Id)
        {

            db = new MarketBotDbContext();


            if (Id > 0)
            {
                var follower = db.Follower.Find(Id);

                follower.Blocked = true;

                db.Update(follower);

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            else
                return NotFound();
        }

        [HttpGet]
        public IActionResult UnBlocked(int Id)
        {

            db = new MarketBotDbContext();

            if (Id > 0)
            {
                var follower = db.Follower.Find(Id);

                follower.Blocked = false;

                db.Update(follower);

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            else
                return NotFound();
        }
    }
}