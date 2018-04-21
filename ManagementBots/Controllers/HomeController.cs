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

    public class HomeController : Controller
    {
        BotMngmntDbContext DbContext { get; set; }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                DbContext = new BotMngmntDbContext();

                var BotInfo = DbContext.BotInfo.LastOrDefault();

                if (BotInfo != null)
                    return View(BotInfo);

                else
                    return View(new BotInfo { Name = "", Token = "" });
            }

            catch (Exception e)
            {
                return Json(e.Message);
            }

            finally
            {

            }
        }

        [HttpPost]
        public async Task<IActionResult> SetWebHook([FromBody] BotInfo botInfo)
        {
            try
            {
                DbContext = new BotMngmntDbContext();

                if(botInfo!=null && botInfo.Id > 0)
                {
                    var _botInfo = DbContext.BotInfo.Find(botInfo.Id);

                    _botInfo.Name = botInfo.Name;
                    _botInfo.Token = botInfo.Token;
                    _botInfo.WebHookUrl = botInfo.WebHookUrl;

                    DbContext.SaveChanges();
                }

                if(botInfo!=null && botInfo.Id == 0)
                {
                    DbContext.Add<BotInfo>(botInfo);
                    DbContext.SaveChanges();


                }

                await BusinessLayer.TelegramFunction.SetWebHook(botInfo.Token, botInfo.WebHookUrl);

                return Ok();
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