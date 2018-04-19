using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ManagementBots.Db;
using Microsoft.EntityFrameworkCore;

namespace ManagementBots.Controllers
{
    [Produces("application/json")]

    public class WebHookUrlController : Controller
    {
        BotMngmntDbContext DbContext { get; set; }
        public IActionResult Index()
        {
            try
            {
                DbContext = new BotMngmntDbContext();

                return View(DbContext.WebHookUrl
                    .Include(w=>w.Dns)
                    .Include(w => w.ReserveWebHookUrl.Bot)
                    .Include(w => w.Port).ToList());
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

        public IActionResult History(int Id)
        {
            try
            {
                DbContext = new BotMngmntDbContext();

                var history = DbContext.WebHookUrlHistory
                    .Where(h => h.WebHookUrlId == Id)
                    .Include(h => h.WebHookUrl.Dns).
                    Include(h=>h.WebHookUrl.Port)
                    .Include(h => h.Bot).ToList();

                return View(history);
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
    }
}