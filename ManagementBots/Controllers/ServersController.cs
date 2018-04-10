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
    public class ServersController : Controller
    {
        BotMngmntDbContext dbContext { get; set; }

        [HttpGet]
        public IActionResult Index()
        {
            BotMngmntDbContext botMngmntDb = new BotMngmntDbContext();

            var list = botMngmntDb.Server.ToList();

            botMngmntDb.Dispose();

            return View("Index2", list);
        }


        [HttpPost]
        public IActionResult Save([FromBody] Server server)
        {
            dbContext = new BotMngmntDbContext();

            try
            {
                if (server != null && server.ServerName != "" && server.Ip != "" && server.WanIp != "" && server.Id==0)
                {
                    if (dbContext.Server.Where(s => s.ServerName == server.ServerName).FirstOrDefault() != null)
                        return Json("Сервер с таким именем существует");

                    if (dbContext.Server.Where(s => s.Ip == server.Ip).FirstOrDefault() != null)
                        return Json("Сервер с таким ip - адресом существует");

                    if (dbContext.Server.Where(s => s.WanIp == server.WanIp).FirstOrDefault() != null)
                        return Json("Сервер с таким внешним ip - адресом существует");

                    if (server.Id == 0 && InsertServer(server) != null)
                        return Json("Добавлено");

                }

                if (server.Id > 0)
                {
                    UpdateServer(server);
                    return Json("Сохранено");
                }

                else
                    return Json("Ошибка");
            }

            catch (Exception e)
            {
                return Json(e.Message);
            }

            finally
            {
                dbContext.Dispose();
            }
        }


        [HttpGet]
        public IActionResult Delete (int Id)
        {
            dbContext = new BotMngmntDbContext();

            try
            {
                var server = dbContext.Server.Find(Id);

                dbContext.Remove<Server>(server);

                dbContext.SaveChanges();

                return Json("Удалено");
            }

            catch (Exception e)
            {
                return Json(e.Message);
            }

            finally
            {
                dbContext.Dispose();
            }
        }

        [HttpGet]
        public IActionResult WebApps(int ServerId)
        {
            try
            {
                dbContext = new BotMngmntDbContext();

                var AppsLsit = dbContext.WebApp.Where(w => w.ServerId == ServerId).Include(w=>w.Server).ToList();

                var Server = dbContext.Server.Find(ServerId);

                ViewBag.ServerId = ServerId;

                ViewBag.Title = Server.ServerName + " | " + Server.Ip + " | " + Server.WanIp;  

                return View("WebApps", AppsLsit);
            }

            catch
            {
                return NotFound();
            }

            finally
            {
                dbContext.Dispose();
            }
        }

        private Server InsertServer(Server server)
        {

            dbContext.Server.Add(server);

            dbContext.SaveChanges();

            return server;
            

        }

        private Server UpdateServer(Server server)
        {

            dbContext.Update<Server>(server);

            dbContext.SaveChanges();

            return server;
            

        }
    }
}
