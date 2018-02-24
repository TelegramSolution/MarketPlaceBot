using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.Controllers
{
    public class OperatorsController : Controller
    {

        MarketBotDbContext db;
        public IActionResult Index()
        {
            db = new MarketBotDbContext();

            var operators = db.Admin.Include(a => a.Follower).ToList();

            return View(operators);
        }

        [HttpGet]

        public IActionResult Delete(int Id)
        {
            db = new MarketBotDbContext();

            var admin= db.Admin.Where(a => a.Id == Id).FirstOrDefault();

            if (admin != null)
            {
                db.Admin.Remove(admin);

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        /// <summary>
        /// дать права оператора 
        /// </summary>
        /// <param name="id">follower id</param>
        /// <returns></returns>
        public IActionResult Add(int id)
        {
            db = new MarketBotDbContext();

            var Bot = db.BotInfo.Where(b => b.Name == MyTelegramBot.Bot.GeneralFunction.GetBotName()).FirstOrDefault();

            var Follower = db.Follower.Find(id);

            var admin= db.Admin.Where(a => a.FollowerId == id).FirstOrDefault();

            if (admin == null && Bot.OwnerChatId!=Follower.ChatId) //Проверям что бы этот пользователь уже не был владельцем бота
            {
                Admin adm = new Admin
                {
                    Enable = true,
                    DateAdd = DateTime.Now,
                    NotyfiActive = true,
                    FollowerId = id

                };

                db.Admin.Add(adm);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}