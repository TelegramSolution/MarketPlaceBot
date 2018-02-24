using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using MyTelegramBot.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyTelegramBot.Controllers
{
    [Produces("application/json")]
    public class HomeController : Controller
    {
        MarketBotDbContext db;

        TelegramBotClient TelegramBot;

        BotInfo botInfo;

        Company company;


        public IActionResult Index()
        {

            db = new MarketBotDbContext();

            string name = GetBotName();

            if (name != null)
            {
                botInfo = db.BotInfo.Where(b => b.Name == name).Include(b => b.Configuration).FirstOrDefault();
                company = db.Company.FirstOrDefault();
                
                if(botInfo!=null && botInfo.Configuration!=null)
                    ViewBag.Currency = new SelectList(db.Currency.ToList(), "Id", "Name", botInfo.Configuration.CurrencyId);

            }

            if (botInfo == null)
            {
                botInfo = new BotInfo
                {
                    Name = "",
                    Token = "",
                    ServerVersion=false,
                    HomeVersion=false
                };

                company = new Company();
            }

            Tuple<BotInfo, Company> tuple = new Tuple<BotInfo, Company>(botInfo, company);

            db.Dispose();
            return View(tuple);
            

        }

        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Add()
        {
            return RedirectToAction("Editor");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save ([FromBody] Configuration _configuration)
        {
            Configuration conf=new Configuration();

            if (db == null)
                db = new MarketBotDbContext();

            if (_configuration.Delivery == false && _configuration.Delivery == false)
                return Json("Должен быть доступен хотя бы один способ получения заказов");

            if(_configuration!=null)
                conf=db.Configuration.Where(c => c.Id == _configuration.Id).FirstOrDefault();


            if (conf != null && conf.Id>0 && _configuration.FreeShipPrice!=0 && _configuration.ShipPrice>0  ||
                conf != null && conf.Id > 0 && _configuration.FreeShipPrice == 0 && _configuration.ShipPrice == 0 )
            {
                conf.OwnerPrivateNotify = _configuration.OwnerPrivateNotify;
                conf.VerifyTelephone = _configuration.VerifyTelephone;
                conf.Pickup = _configuration.Pickup;
                conf.Delivery = _configuration.Delivery;
                conf.StartTime = _configuration.StartTime;
                conf.EndTime = _configuration.EndTime;
                conf.ShipPrice = _configuration.ShipPrice;
                conf.FreeShipPrice = _configuration.FreeShipPrice;

                if (conf.CurrencyId != _configuration.CurrencyId) // если изенился тип валюты, то меняем тип валюты во всех ценах на товары
                {
                    conf.CurrencyId = _configuration.CurrencyId;
                    UpdatePriceCurrency(Convert.ToInt32(_configuration.CurrencyId));
                    db.Dispose();
                }
                    

                if (db.SaveChanges() >= 0)
                    return Json("Сохранено");

                else
                    return Json("Ошибка");
            }

            if(_configuration.ShipPrice>0 && _configuration.FreeShipPrice==0)
                return Json("Ошибка. Если стоимость доставки больше 0, то значение поля \"Бесплатная доставка от\" должна быть больше 0");
            
            else
                return Json("Ошибка");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ContactUpdate([FromBody] Company _company)
        {
            company = new Company();

            using (MarketBotDbContext db = new MarketBotDbContext())
            {

                if (_company != null && _company.Id > 0)
                    company = db.Company.Where(c => c.Id == _company.Id).FirstOrDefault();

                if (company != null)
                {
                    company.Instagram = _company.Instagram;
                    company.Vk = _company.Vk;
                    company.Chanel = _company.Chanel;
                    company.Chat = _company.Chat;
                    company.Text = _company.Text;

                    if (db.SaveChanges() >= 0)
                        return Json("Сохранено");

                    else
                        return Json("Ошибка");
                }

                else
                    return Json("Ошибка");
            }
        }


        private void UpdatePriceCurrency (int CurrencyId)
        {
            if (db == null)
                db = new MarketBotDbContext();

            var PriceList = db.ProductPrice.Where(p => p.Enabled).ToList();

            foreach (ProductPrice pr in PriceList)
            {
                pr.CurrencyId = CurrencyId;
            }

            db.SaveChanges();
        }

        public async Task<IActionResult> Editor()
        {
            db = new MarketBotDbContext();

            var bot = db.BotInfo.Where(b => b.Name == GetBotName()).FirstOrDefault();

            if (bot != null)
            {

                TelegramBot = new TelegramBotClient(bot.Token);

                var webhook = await TelegramBot.GetWebhookInfoAsync();

                var botinfo = await TelegramBot.GetMeAsync();

                string info = "Название бота:" + botinfo.Username +
                    "; WebHook адрес: " + webhook.Url + "; Самоподписанный сертификат: " + webhook.HasCustomCertificate + "; Время последней ошибки: " + webhook.LastErrorDate + "; Текст ошибки: " + webhook.LastErrorMessage;

                ViewBag.Webhook = info;
            }

            else // Если данных еще нет
            {
                bot = new BotInfo
                {
                    Name = "",
                    Token = ""
                };


            }

            db.Dispose();
            return View(bot);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveInfo(BotInfo _bot, IFormFile file = null)
        {

                if (_bot != null)
                {
                    TelegramBot = new TelegramBotClient(_bot.Token);

                    db = new MarketBotDbContext();

                    var reapet_bot = db.BotInfo.Where(b => b.Name == Bot.GeneralFunction.GetBotName()).FirstOrDefault();

                    botInfo = db.BotInfo.Where(b => b.Id == _bot.Id).FirstOrDefault();

                    Telegram.Bot.Types.FileToSend toSend;

                    if (file != null)
                        toSend = ConvertToFileToSend(file);

                    if (_bot.WebHookUrl != null && TelegramBot != null && _bot.WebHookUrl != null && file != null) // обновляем вебхук
                        await TelegramBot.SetWebhookAsync(_bot.WebHookUrl + "/bot/", toSend);

                    if (_bot.WebHookUrl != null && TelegramBot != null && _bot.WebHookUrl != null && file == null) // обновляем вебхук
                        await TelegramBot.SetWebhookAsync(_bot.WebHookUrl + "/bot/");

                    //TelegramBot.ExportChatInviteLinkAsync()

                    if (_bot.Id == 0 && reapet_bot == null) //Бот еще не настроен. Добавляем новые данные
                    {
                        _bot.Name = Bot.GeneralFunction.GetBotName();
                        _bot.ServerVersion = false;
                        _bot.HomeVersion = false;
                        _bot.Configuration = new Configuration { VerifyTelephone = false, OwnerPrivateNotify = false, Delivery = true, Pickup = false, ShipPrice = 0, FreeShipPrice = 0, CurrencyId = 1 };
                        _bot = InsertBotInfo(_bot);
                        Company company = new Company { Instagram = "https://www.instagram.com/", Vk = "https://vk.com/", Chanel = "https://t.me/", Chat = "https://t.me/" };
                        db.Company.Add(company);
                        return View("Own", "/owner" + _bot.Token.Split(':').ElementAt(1).Substring(0, 15));

                    }

                    if (_bot.Id > 0) // редактируем уже сущестующие данные
                    {
                        UpdateBotInfo(_bot);

                        //если по каким то причинам пользователь не подрвердил себя как владельца
                        if (_bot.OwnerChatId == null)
                        {
                            return View("Own", "/owner" + _bot.Token.Split(':').ElementAt(1).Substring(0, 15));
                        }

                        return RedirectToAction("Index");

                    }

                    else
                        return RedirectToAction("Index");
                }


                else
                    return RedirectToAction("Index");

            
        }

        private int UpdateBotInfo(BotInfo bot)
        {
            if (db == null)
                db = new MarketBotDbContext();

            botInfo = db.BotInfo.Where(b => b.Id == bot.Id).Include(b => b.Configuration).FirstOrDefault();
            botInfo.Token = bot.Token;
            botInfo.WebHookUrl = bot.WebHookUrl;
            return db.SaveChanges();
        }

        private BotInfo InsertBotInfo(BotInfo bot)
        {
            if (db == null)
                db = new MarketBotDbContext();

            var spl = bot.Token.Split(':');
            int chat_id = Convert.ToInt32(spl[0]);

            BotInfo botInfo = new BotInfo
            {
                Name = bot.Name,
                Token = bot.Token,
                ChatId = chat_id,
                Timestamp = DateTime.Now,
                WebHookUrl=bot.WebHookUrl,
                Configuration=bot.Configuration
                ,ServerVersion=bot.ServerVersion,
                HomeVersion=bot.HomeVersion
                
            };

            db.BotInfo.Add(botInfo);
            db.SaveChanges();
            return bot;
        }

        private AdminKey AddOwnerKey(string key)
        {
            if (db == null)
                db = new MarketBotDbContext();

            AdminKey adminKey = new AdminKey
            {
                DateAdd = DateTime.Now,
                Enable = true,
                KeyValue = key
            };

            db.AdminKey.Add(adminKey);
            db.SaveChanges();
            return adminKey;
        }

        private string GetBotName()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            string name = builder.Build().GetSection("BotName").Value;
            return name;
        }

        private Telegram.Bot.Types.FileToSend ConvertToFileToSend (IFormFile file)
        {
            if (file != null)
            {
                System.IO.MemoryStream memory = new System.IO.MemoryStream();
                file.CopyTo(memory);
                Telegram.Bot.Types.FileToSend toSend = new Telegram.Bot.Types.FileToSend
                {
                    Content = memory,
                    Filename = "@" + file.FileName
                };

                return toSend;
            }

            return new Telegram.Bot.Types.FileToSend { };
        }
    }
}
