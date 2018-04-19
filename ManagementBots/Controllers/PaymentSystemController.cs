using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ManagementBots.Db;

namespace ManagementBots.Controllers
{
    [Produces("application/json")]

    public class PaymentSystemController : Controller
    {
        BotMngmntDbContext DbContext { get; set; }

        const int Qiwi = 1;

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                DbContext = new BotMngmntDbContext();

                List<PaymentSystemConfig> list = new List<PaymentSystemConfig>();

                list = DbContext.PaymentSystemConfig.Where(p => p.PaymentSystemId == Qiwi).OrderByDescending(p => p.Id).ToList();

                return View(list);
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
        public IActionResult Delete (int Id)
        {
            try
            {
                DbContext = new BotMngmntDbContext();

                var cfg = DbContext.PaymentSystemConfig.Find(Id);

                DbContext.Remove<PaymentSystemConfig>(cfg);

                DbContext.SaveChanges();

                return Json("Удалено");
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

        [HttpPost]
        public IActionResult Save([FromBody] PaymentSystemConfig config)
        {
            try
            {
                if(config!=null && config.Login!="" && config.Pass != "" && config.Id>0)
                {
                    DbContext = new BotMngmntDbContext();
                    var cfg = DbContext.PaymentSystemConfig.Find(config.Id);

                    cfg.Login = config.Login;
                    cfg.Pass = config.Pass;

                    DbContext.Update<PaymentSystemConfig>(cfg);
                    DbContext.SaveChanges();

                    return Json("Сохранено");
                }

                if (config != null && config.Login != "" && config.Pass != "" && config.Id == 0)
                {
                    config.TimeStamp = DateTime.Now;

                    DbContext = new BotMngmntDbContext();

                    DbContext.PaymentSystemConfig.Add(config);

                    DbContext.SaveChanges();

                    return Json("Добавлено");
                }

                else
                    return Json("Ошика");
            }

            catch (Exception e)
            {
                return Json(e.Message);
            }

            finally
            {
                if (DbContext != null)
                    DbContext.Dispose();
            }
            
        }


        [HttpGet]
        public IActionResult QiwiTest(string Login, string Pass)
        {
            return Ok();
        }


    }
}