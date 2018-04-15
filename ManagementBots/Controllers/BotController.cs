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

        public IActionResult Create()
        {
            return Ok();
        }
    }
}