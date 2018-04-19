using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ManagementBots.Db;
using ManagementBots.BusinessLayer;

namespace ManagementBots.Controllers
{
    [Produces("application/json")]

    public class NotificationController : Controller
    {
        BotMngmntDbContext DbContext { get; set; }


        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                DbContext = new BotMngmntDbContext();

                return View(DbContext.Notification.ToList());
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
        public IActionResult SaveAndSend(string Text)
        {
            try
            {
                DbContext = new BotMngmntDbContext(); 
                if (Insert(Text).Id > 0)
                {
                    var BotToken = DbContext.BotInfo.LastOrDefault().Token;

                    var Followers = DbContext.Follower.ToList();

                    foreach (var follower in Followers)
                    {
                        System.Threading.Thread.Sleep(300);
                        TelegramFunction.SendTextMessage(Text, Convert.ToInt32(follower.ChatId), BotToken);
                    }
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
                if(DbContext!=null)
                DbContext.Dispose();
            }
        }

        private Notification Insert(string Text)
        {
            Notification notification = new Notification
            {
                DateAdd = DateTime.Now,
                Text = Text,
                Sended = true

            };

            DbContext.Notification.Add(notification);

            DbContext.SaveChanges();

            return notification;
        }
    }
}