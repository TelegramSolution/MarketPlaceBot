using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementBots.Db;
using Telegram.Bot.Types;
using Microsoft.EntityFrameworkCore;
using ManagementBots.Models;

namespace ManagementBots.BusinessLayer
{
    public class BotConnectFunction
    {
        private BotMngmntDbContext DbContext { get; set; }

        public BotConnectFunction()
        {
            DbContext = new BotMngmntDbContext();
        }
        private Db.Bot InsertBot(string Name, string Token, int Owner, bool Visable=false)
        {

            try
            {
                Db.Bot bot = new Db.Bot
                {
                    Blocked = false,
                    BotName = Name,
                    FollowerId = Owner,
                    Launched = false,
                    Token = Token,
                    Deleted = false,
                    Visable = Visable,
                    
                };

                DbContext.Bot.Add(bot);
                DbContext.SaveChanges();
                return bot;
            }

            catch
            {
                return null;
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="BotInfo"></param>
        /// <param name="FollowerId">Владелец</param>
        /// <returns></returns>
        public Db.Bot AddBot(User BotInfo, string BotToken, int FollowerId)
        {
           var reapet= DbContext.Bot.Where(b => b.BotName == BotInfo.Username).FirstOrDefault();

            if (reapet != null && reapet.Visable==true)
                throw new Exception(String.Format("{0} уже существует",BotInfo.Username));

            if (reapet != null && reapet.Visable == false)
                return reapet;

            else
                return InsertBot(BotInfo.Username, BotToken, FollowerId);
            
        }

        public async Task<Db.Bot> SelectServiceType(int BotId, int ServiceTypeId)
        {
            var ServiceType = DbContext.ServiceType.Find(ServiceTypeId);

            var Bot = DbContext.Bot.Where(b => b.Id == BotId).Include(b => b.Follower).FirstOrDefault(); ;

            if(Bot.Launched)
                throw new Exception("Бот уже запущен");

            if (ServiceType.IsDemo)
                return await InstallDemo(Bot, ServiceType);

            else
                return null;
            
        }

        private async Task<Db.Bot> InstallDemo(Db.Bot Bot, ServiceType ServiceType)
        {
            //проверяем брал ли когда нибудь этот пользователь пробную версию если брал то вызываем исключение
            if (UsedDemo(Bot.FollowerId))
                throw new Exception("Вы уже использовали пробную версию");

            var WebApp = SearchFreeWebApp();

            var WebHookUrl = SearchWebHookUrl();

            var ProxyServer = DbContext.ProxyServer.Where(p => p.Enable).FirstOrDefault();

            //Веб приложени свободно токен действителен. Устанавливаем бота
            string result = await WebApp.Install(
                            new HostInfo
                            {
                                Token = Bot.Token,
                                BotName = Bot.BotName,
                                IsDemo = ServiceType.IsDemo,
                                UrlWebHook = WebHookUrl.ToString(),
                                OwnerChatId = Convert.ToInt32(Bot.Follower.ChatId)
                            }
             );


            var Response = Newtonsoft.Json.JsonConvert.DeserializeObject<BotResponse>(result);

            //Установка бота на веб приложение прошла успешно.Создаем файл для прокси сервера и заливаем на сервер,Перезапускаем службу прокси сервера (nginx)
            if (Response.Ok && ProxyServer.CreateConfigFile(WebHookUrl.Dns.Name, WebApp.ToString(), Convert.ToInt32(WebHookUrl.Port.PortNumber)))
            {
                //Если все хорошо вызываем метод SetWebhook
                await TelegramFunction.SetWebHook(Bot.Token, WebHookUrl.Dns.PublicKeyPathOnMainServer(), WebHookUrl.ToString());

                // Добавляем услугу в бд
                Service service = new Service { ServiceTypeId = ServiceType.Id, CreateTimeStamp = DateTime.Now, DayDuration = ServiceType.MaxDuration, IsStart = true, Visable = true, StartTimeStamp = DateTime.Now };
                service = InsertService(service);

                InsertServiceBotHistory(Bot, service);
                InsertWebAppHistory(Bot, WebApp);
                InsertWebHookHistory(Bot, WebHookUrl);

                WebApp.IsFree = false;
                WebHookUrl.IsFree = false;

                //привязываем  услугу, прокси сервер, веб приложение, доменное имя к боту
                Bot.ProxyServeId = ProxyServer.Id;
                Bot.ServiceId = service.Id;
                Bot.CreateTimeStamp = DateTime.Now;
                Bot.WebHookUrlId = WebHookUrl.Id;
                Bot.WebAppId = WebApp.Id;
                Bot.Launched = true;

                DbContext.SaveChanges();

                service.ServiceType = ServiceType;
                Bot.Service = service;
                
                return Bot;

            }
            else // Ошибка во время установки бота на вебприложение
                throw new Exception(Response.Result);
        }

        private void InsertServiceBotHistory(Db.Bot bot , Service service)
        {
            DbContext.ServiceBotHistory.Add(new ServiceBotHistory { ServiceId = service.Id, BotId = bot.Id });
            DbContext.SaveChanges();
        }

        private void InsertWebHookHistory(Db.Bot bot, WebHookUrl hookUrl)
        {
            WebHookUrlHistory webHookUrlHistory = new WebHookUrlHistory
            {
                BotId = bot.Id,
                WebHookUrlId = hookUrl.Id,
                Timestamp = DateTime.Now
            };

            DbContext.WebHookUrlHistory.Add(webHookUrlHistory);

        }

        private void InsertWebAppHistory(Db.Bot bot, WebApp webApp)
        {
            WebAppHistory webAppHistory = new WebAppHistory
            {
                BotId = bot.Id,
                TimeStamp = DateTime.Now,
                WebAppId = webApp.Id
            };

            DbContext.WebAppHistory.Add(webAppHistory);
        }

        private Service InsertService(Service service)
        {
            DbContext.Service.Add(service);

            DbContext.SaveChanges();

            return service;
        }

        private WebApp SearchFreeWebApp()
        {
            var Webapp = DbContext.WebApp.Where(w => w.IsFree).Include(w => w.ServerWebApp).FirstOrDefault();


            if (Webapp != null)
                return Webapp;

            else
                throw new Exception("Нет свободных вычислительных ресурсов. Обратитесь в службу поддержки /help");

        }

        private WebHookUrl SearchWebHookUrl()
        {
            var Url = DbContext.WebHookUrl.Where(w => w.IsFree).Include(u => u.Dns).Include(u=>u.Port).FirstOrDefault();

            if (Url != null)
                return Url;

            else
                throw new Exception("Нет свободных доменных имен. Обратитесь в службу поддержки /help");
        }

        private bool UsedDemo(int? FollowerId)
        {
            return false;
        }

        public void Dispose()
        {
            if (DbContext != null)
                DbContext.Dispose();
        }
    }
}
