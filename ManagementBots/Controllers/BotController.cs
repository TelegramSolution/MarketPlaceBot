using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManagementBots.Db;
using ManagementBots.BusinessLayer;
using ManagementBots.Models;


namespace ManagementBots.Controllers
{
    [Produces("application/json")]

    public class BotController : Controller
    {
        BotMngmntDbContext DbContext { get; set; }

        [HttpGet]

        public IActionResult Index()
        {
            try
            {
                DbContext = new BotMngmntDbContext();

                var bots = DbContext.Bot.Include(b => b.Follower)
                    .Include(b => b.WebApp).Include(b => b.Service.ServiceType).ToList();

                return View(bots);
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
            try
            {
                DbContext = new BotMngmntDbContext();

                var bot = DbContext.Bot.Where(b => b.Id == Id).
                    Include(b => b.Follower)
                    .Include(b => b.WebApp.ServerWebApp)
                    .Include(b => b.Service.ServiceType)
                    .Include(b => b.ProxyServer)
                    .Include(b=>b.WebHookUrl.Dns)
                    .Include(b=>b.WebHookUrl.Port)
                    .Include(b=>b.ServiceBotHistory)
                    .FirstOrDefault();

                foreach (var service in bot.ServiceBotHistory)
                    service.Service = DbContext.Service.Where(s=>s.Id==service.ServiceId).Include(s=>s.ServiceType).FirstOrDefault();

                return View(bot);
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
        public IActionResult Create()
        {
            try
            {
                DbContext = new BotMngmntDbContext();

                var WebHookList = DbContext.WebHookUrl
                    .Where(w => w.IsFree).Include(w=>w.Port)
                    .Include(w=>w.Dns).ToList();

                var WebAppList = DbContext.WebApp.Where(w => w.IsFree)
                                .Include(w => w.ServerWebApp).ToList();

                var ServiceTypeList = DbContext.ServiceType.Where(s => s.Enable).ToList();

                var FollowerList = DbContext.Follower.ToList();

                List<SelectItem> Urllist = new List<SelectItem>();

                List<SelectItem> AppList = new List<SelectItem>();

                List<SelectItem> ServiceList = new List<SelectItem>();

                List<SelectItem> FollowerSelectList = new List<SelectItem>();

                foreach (var url in WebHookList)
                    Urllist.Add(new SelectItem { Id = url.Id, Name = url.ToString() });

                foreach (var app in WebAppList)
                    AppList.Add(new SelectItem { Id = app.Id, Name = app.ToString() });

                foreach (var service in ServiceTypeList)
                    ServiceList.Add(new SelectItem { Id = service.Id, Name = service.Name });

                foreach (var follower in FollowerList)
                    FollowerSelectList.Add(new SelectItem { Id = follower.Id, Name = follower.ToString() });


                ViewBag.WebHookUrl = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(Urllist, "Id", "Name", Urllist.FirstOrDefault().Id);
                ViewBag.WebApp = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(AppList, "Id", "Name", AppList.FirstOrDefault().Id);
                ViewBag.ServiceType = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(ServiceList, "Id", "Name", ServiceList.FirstOrDefault().Id);
                ViewBag.Follower = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(FollowerSelectList, "Id", "Name", FollowerSelectList.FirstOrDefault().Id);


                var bot = new Db.Bot
                {
                    Service = new Service { DayDuration = 0, ServiceTypeId = ServiceList.FirstOrDefault().Id }
                };


                return View(bot);
            }

            catch(Exception e)
            {
                return Json(e.Message);
            }

            finally
            {
                DbContext.Dispose();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Db.Bot _bot)
        {
            try
            {
                HostInfo hostInfo = new HostInfo();
                
                DbContext = new BotMngmntDbContext();

                var WebApp = DbContext.WebApp.Where(w => w.Id == _bot.WebAppId).Include(w => w.ServerWebApp).FirstOrDefault();

                var WebHookUrl = DbContext.WebHookUrl.Where(w => w.Id == _bot.WebHookUrlId).Include(w => w.Port).Include(w => w.Dns).FirstOrDefault();

                var ServiceType = DbContext.ServiceType.Find(_bot.Service.ServiceTypeId);

                var Follower = DbContext.Follower.Find(_bot.FollowerId);

                var ProxyServer = DbContext.ProxyServer.Where(p => p.Enable).FirstOrDefault();

                if (DbContext.Bot.Where(b => b.Token == _bot.Token).FirstOrDefault() != null)
                    return Json("Бот с таким токеном уже существует");

                //Вызываем метод GetMe на сервер телеграм, тем самым проверяем 
                //корректный ли токен мы ввели
                var User= await TelegramFunction.GetMe(_bot.Token);

                if (User != null)
                    _bot.BotName = User.Username;

                //Подключаемся к веб приложению и проверяем свободно ли оно
                hostInfo = WebApp.GetInfo();

                if (hostInfo.IsFree)
                {
                    //Веб приложени свободно токен действителен. Устанавливаем бота
                    string result =await WebApp.Install( 
                        new HostInfo{
                                        Token =_bot.Token,
                                        BotName =_bot.BotName,
                                        IsDemo =ServiceType.IsDemo,
                                        UrlWebHook =WebHookUrl.ToString(),
                                        OwnerChatId =Convert.ToInt32(Follower.ChatId),
                                        DbName=_bot.BotName+ GeneralFunction.UnixTimeNow().ToString()
                        });

                    var Response = Newtonsoft.Json.JsonConvert.DeserializeObject<BotResponse>(result);

                    //Установка бота на веб приложение прошла успешно. 
                    //Создаем файл для прокси сервера и заливаем на сервер,
                    //Перезапускаем службу прокси сервера (nginx)
                    if (Response.Ok && ProxyServer.CreateConfigFile(WebHookUrl.Dns.Name, WebApp.ToString(), Convert.ToInt32(WebHookUrl.Port.PortNumber)))
                    {

                        //Если все хорошо вызываем метод SetWebhook
                        var Info = await TelegramFunction.SetWebHook(_bot.Token, WebHookUrl.Dns.PublicKeyPathOnMainServer(), WebHookUrl.ToString());

                        if (Info.LastErrorMessage != null && Info.LastErrorMessage != "")
                            throw new Exception(Info.LastErrorMessage);

                    }

                    else // Ошибка во время установки бота на вебприложение
                        throw new Exception(Response.Result);
                }

                else
                    throw new Exception("Ошибка! Веб приложение " + WebApp.ToString() + " Занято ботом " + hostInfo.BotName);


                //Вызываем метод WebHookInfo если все ок то заносим инфу в базу данных

                //если нет то удаляем бота из веб приложеия и конрфигурационный файл
                // на прокси сервере

            }

            catch (Exception e)
            {
                return Json(e.Message);
            }

            return Ok();
        }


    }
}