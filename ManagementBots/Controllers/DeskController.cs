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

    public class DeskController : Controller
    {
        BotMngmntDbContext DbContext { get; set; }

        [HttpGet]

        public IActionResult Index()
        {
            try
            {
                DbContext = new BotMngmntDbContext();

                var list = DbContext.HelpDesk.Include(h => h.Follower).ToList();      

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
        public IActionResult Get(int Id)
        {
            DbContext = new BotMngmntDbContext();

            try
            {
                var desk = DbContext.HelpDesk.Where(h => h.Id == Id)
                                    .Include(h => h.HelpDeskAnswer)
                                    .Include(h => h.Follower)
                                    .Include(h => h.HelpDeskAttachment).FirstOrDefault();

                foreach (var answer in desk.HelpDeskAnswer)
                    answer.Follower = DbContext.Follower.Find(answer.Follower.Id);

                return View(desk);
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
        public IActionResult AddAnswer (int DeskId, string Text)
        {
            DbContext = new BotMngmntDbContext();

            try
            {
                var Desk = DbContext.HelpDesk.Find(DeskId);

                if (Text != null && Text != "" && Desk!=null && !Desk.Closed)
                {
                    HelpDeskAnswer deskAnswer = new HelpDeskAnswer
                    {
                        HelpDeskId = DeskId,
                        Closed = false,
                        FollowerId = 1,
                        Text = Text,
                        Timestamp = DateTime.Now
                    };

                    DbContext.HelpDeskAnswer.Add(deskAnswer);

                    DbContext.SaveChanges();


                    return Json("Добавлено");
                }

                if (Desk != null && Desk.Closed)
                    return Json("Заявка уже закрыта");

                else
                    return Json("Ошибка");
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
        public IActionResult DeskClose(int DeskId)
        {

            try
            {
                DbContext = new BotMngmntDbContext();

                var desk = DbContext.HelpDesk.Find(DeskId);

                if (desk != null && !desk.Closed)
                {
                    HelpDeskAnswer deskAnswer = new HelpDeskAnswer
                    {
                        Closed = true,
                        ClosedTimestamp = DateTime.Now,
                        FollowerId = 1,
                        HelpDeskId = DeskId,
                        Text = "Заявка закрыта",
                        Timestamp = DateTime.Now
                    };

                    desk.Closed = true;

                    DbContext.HelpDeskAnswer.Add(deskAnswer);

                    DbContext.SaveChanges();

                    return Json("Сохранено");
                }

                if (desk != null && desk.Closed)
                    return Json("Заявка уже закрыта");

                else
                   return Json("Ошибка");
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