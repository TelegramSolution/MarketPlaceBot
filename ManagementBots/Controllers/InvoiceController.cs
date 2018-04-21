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

    public class InvoiceController : Controller
    {
        BotMngmntDbContext DbContext { get; set; }

        public IActionResult Get(int Id)
        {
            try
            {
                DbContext = new BotMngmntDbContext();

                var service = DbContext.Service.Where(s => s.InvoiceId == Id)
                                .Include(s => s.Invoice.PaymentSystem)
                                .Include(s=>s.ServiceType).LastOrDefault();


                return View(service);
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