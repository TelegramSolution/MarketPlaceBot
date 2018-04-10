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

    public class WebAppController : Controller
    {

        BotMngmntDbContext DbContext { get; set; }

        [HttpGet]
        public IActionResult Delete (int Id)
        {
            try
            {
                DbContext = new BotMngmntDbContext();

                var WebApp = DbContext.WebApp.Find(Id);

                DbContext.Remove<WebApp>(WebApp);

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
        public IActionResult Post ([FromBody] WebApp webApp)
        {
            DbContext = new BotMngmntDbContext();

            try
            {
                var repeat = DbContext.WebApp.Where(w => w.ServerId == webApp.ServerId && w.Port == webApp.Port).FirstOrDefault();

                if (repeat != null && webApp!=null && webApp.Id==0)
                    return Json(String.Format("Веб приложения с портом {0} уже существует", webApp.Port));

                if(webApp!=null && webApp.Port!="" && webApp.ServerId>0 && webApp.Id == 0 && IsnertWebApp(webApp).Id>0)
                    return Json("Добавлено");

                if(webApp != null && webApp.Port != "" && webApp.ServerId > 0 && webApp.Id > 0)
                {
                    UpdateWebApp(webApp);
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
                DbContext.Dispose();
            }
        }

        [HttpGet]
        public IActionResult History(int WebAppId)
        {
            DbContext = new BotMngmntDbContext();

            try
            {
                var list = DbContext.WebAppHistory.Where(h => h.WebAppId == WebAppId).Include(h => h.Bot).ToList();

                List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();


                foreach(var history in list)
                {
                    Dictionary<string, string> row = new Dictionary<string, string>();

                    row.Add("Id",history.Id.ToString());
                    row.Add("TimeStamp", history.TimeStamp.ToString());
                    row.Add("BotName", history.Bot.BotName);
                    row.Add("BotId", history.BotId.ToString());

                    result.Add(row);
                }

                return Json(result);
            }

            catch
            {
                return NotFound();
            }

            finally
            {
                DbContext.Dispose();
            }
        }

        private WebApp IsnertWebApp(WebApp webApp)
        {
            DbContext.WebApp.Add(webApp);

            DbContext.SaveChanges();

            return webApp;

        }

        private WebApp UpdateWebApp(WebApp webApp)
        {
            DbContext.Update<WebApp>(webApp);

            DbContext.SaveChanges();

            return webApp;
        }

    }
}