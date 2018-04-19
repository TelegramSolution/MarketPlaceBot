using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManagementBots.Db;

namespace ManagementBots.Controllers
{
    [Produces("application/json")]

    public class BotController : Controller
    {
        BotMngmntDbContext DbContext { get; set; }

        [HttpGet]

        public IActionResult Index()
        {
            try
            {
                DbContext = new BotMngmntDbContext();

                var bots = DbContext.Bot.Include(b => b.Follower)
                    .Include(b => b.WebApp).Include(b => b.Service.ServiceType).ToList();

                return View(bots);
            }

            catch (Exception e)
            {
               return Json(e.Message);
            }

            finally
            {
                DbContext.Dispose();
            }
        }

        [HttpGet]
        public IActionResult Get(int Id)
        {
            try
            {
                DbContext = new BotMngmntDbContext();

                var bot = DbContext.Bot.Where(b => b.Id == Id).
                    Include(b => b.Follower)
                    .Include(b => b.WebApp.ServerWebApp)
                    .Include(b => b.Service.ServiceType)
                    .Include(b => b.ProxyServer)
                    .Include(b=>b.WebHookUrl.Dns)
                    .Include(b=>b.WebHookUrl.Port)
                    .Include(b=>b.ServiceNavigation)
                    .FirstOrDefault();

                foreach (var service in bot.ServiceNavigation)
                    service.ServiceType = DbContext.ServiceType.Find(service.ServiceTypeId);

                return View(bot);
            }

            catch (Exception e)
            {
                return Json(e.Message);
            }

            finally
            {
                DbContext.Dispose();
            }
        }

        public IActionResult Create()
        {
            return Ok();
        }
    }
}