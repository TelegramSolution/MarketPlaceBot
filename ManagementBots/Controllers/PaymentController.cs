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
   
    public class PaymentController : Controller
    {
        BotMngmntDbContext DbContext { get; set; }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                DbContext = new BotMngmntDbContext();

                return View(DbContext.Payment.Include(p=>p.Invoice).ToList());
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