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

                if (repeat != null)
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