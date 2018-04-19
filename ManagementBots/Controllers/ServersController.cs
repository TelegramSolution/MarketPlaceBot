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

            var list = botMngmntDb.ServerWebApp.ToList();

            botMngmntDb.Dispose();

            return View("Index2", list);
        }


        [HttpPost]
        public IActionResult Save([FromBody] ServerWebApp server)
        {
            dbContext = new BotMngmntDbContext();

            try
            {
                if (server != null && server.ServerName != "" && server.Ip != "" && server.WanIp != "" && server.Id==0)
                {
                    if (dbContext.ServerWebApp.Where(s => s.ServerName == server.ServerName).FirstOrDefault() != null)
                        return Json("Сервер с таким именем существует");

                    if (dbContext.ServerWebApp.Where(s => s.Ip == server.Ip).FirstOrDefault() != null)
                        return Json("Сервер с таким ip - адресом существует");

                    if (dbContext.ServerWebApp.Where(s => s.WanIp == server.WanIp).FirstOrDefault() != null)
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
                var server = dbContext.ServerWebApp.Find(Id);

                dbContext.Remove<ServerWebApp>(server);

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

                var AppsLsit = dbContext.WebApp.Where(w => w.ServerWebAppId == ServerId).Include(w=>w.ServerWebApp).ToList();

                var Server = dbContext.ServerWebApp.Find(ServerId);

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

        public IActionResult WebAppHistory(int WebAppId)
        {
            try
            {
                dbContext = new BotMngmntDbContext();

                var history = dbContext.WebAppHistory.Where(w => w.WebAppId == WebAppId)
                    .Include(w => w.WebApp)
                    .Include(w => w.Bot).ToList();

                return View("History", history);
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
        private ServerWebApp InsertServer(ServerWebApp server)
        {

            dbContext.ServerWebApp.Add(server);

            dbContext.SaveChanges();

            return server;
            

        }

        private ServerWebApp UpdateServer(ServerWebApp server)
        {

            dbContext.Update<ServerWebApp>(server);

            dbContext.SaveChanges();

            return server;
            

        }
    }
}
