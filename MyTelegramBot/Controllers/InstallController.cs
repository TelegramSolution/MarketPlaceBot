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
using HtmlAgilityPack;

namespace MyTelegramBot.Controllers
{
    public class InstallController : Controller
    {

        MarketBotDbContext db;

        TelegramBotClient TelegramBot;

        public IActionResult Index()
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> InstallHomeVersion(string token)
        {
            db = new MarketBotDbContext();
            string name = Bot.GeneralFunction.GetBotName().Trim();
            string ngrok = GetNgrokUrl().Trim();

            if (token != null)
            {
                var BotInf = db.BotInfo.Where(b => b.Name == name).FirstOrDefault();

                //в базе уже есть инф. об этом боте
                if (BotInf != null)
                {
                    if (await SetWebhookAsync(token, ngrok, new Telegram.Bot.Types.FileToSend { }))
                    {
                        BotInf.Token = token;
                        BotInf.WebHookUrl = ngrok;
                        BotInf.ServerVersion = false;
                        BotInf.HomeVersion = true;
                        db.SaveChanges();

                        db.Dispose();
                        return Ok();
                    }

                    else
                        return NotFound();
                }
                    

                else // инф. в базе еще нет. Добавляем
                {
                    if (await SetWebhookAsync(token, ngrok, new Telegram.Bot.Types.FileToSend { }))
                    {
                        InsertNewBotToDb(token, name, ngrok);
                        db.Dispose();
                        return Json(token.Split(':').ElementAt(1).Substring(0, 15));
                    }

                    else
                        return NotFound();
                }
            }

            else
                return NotFound();
        }

        [HttpGet]
        public IActionResult InstallServerVersion(string token, string domainname)
        {
            db = new MarketBotDbContext();
            string name = Bot.GeneralFunction.GetBotName();

            if (token != null && domainname != null)
            {
                var BotInf = db.BotInfo.Where(b => b.Name == name).FirstOrDefault();

                //в базе уже есть инф. об этом боте
                if (BotInf != null)
                {
                    BotInf.Token = token;
                    BotInf.WebHookUrl = "https://" + domainname;
                    BotInf.ServerVersion = true;
                    BotInf.HomeVersion = false;
                    db.SaveChanges();
                    db.Dispose();
                    return Ok();
                }


                else // инф. в базе еще нет. Добавляем
                {               
                    InsertNewBotToDb(token, name, "https://" + domainname,true);
                    db.Dispose();
                    return Json(token.Split(':').ElementAt(1).Substring(0, 15));
                    
                }
            }

            else
                return NotFound();
        }

        /// <summary>
        /// Удаляем веб хук 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> StopHomeVersion()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {

                try
                {
                    string name = Bot.GeneralFunction.GetBotName();
                    var botinfo = db.BotInfo.Where(b => b.Name == name).FirstOrDefault();

                    if (botinfo != null)
                    {
                        TelegramBot = new TelegramBotClient(botinfo.Token);
                        //await TelegramBot.DeleteWebhookAsync();
                        await TelegramBot.SendTextMessageAsync(botinfo.OwnerChatId, "Бот остановлен");
                        return Ok();
                    }

                    if (botinfo == null)
                        return NotFound();

                    else
                        return NotFound();
                }

                catch(Exception e)
                {
                    return Json(e.Message + e.Source);
                }
            }
        }

        /// <summary>
        /// Запускаем бота. Вытаскиваем ngrok url и обновляем веб хук
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> StartHomeVersion()
        {
            using (MarketBotDbContext db=new MarketBotDbContext())
            {
                try
                {
                    string NgrokUrl = GetNgrokUrl();
                    var BotInfo = db.BotInfo.Where(b => b.Name == Bot.GeneralFunction.GetBotName()).FirstOrDefault();

                    if (BotInfo != null && NgrokUrl!=null)
                    {
                        TelegramBot = new TelegramBotClient(BotInfo.Token);
                        await TelegramBot.SetWebhookAsync(NgrokUrl + "/bot/");
                        BotInfo.WebHookUrl = NgrokUrl;
                        await TelegramBot.SendTextMessageAsync(BotInfo.OwnerChatId, "Запуск");
                        db.SaveChanges();
                        return Ok();
                    }

                    if (BotInfo == null)
                    {
                        return NotFound();
                    }

                    if (BotInfo != null && NgrokUrl == null)
                    {
                        TelegramBot = new TelegramBotClient(BotInfo.Token);
                        await TelegramBot.SendTextMessageAsync(BotInfo.OwnerChatId, "Ошибка. Ngrok не запущен!");
                        return NotFound();
                    }
                        

                    else
                        return NotFound();

                }

                catch (Exception e)
                {
                    return Json(e.Message + e.Source);
                }
            }
        }

        private async Task<bool> SetWebhookAsync(string token, string url, Telegram.Bot.Types.FileToSend fileToSend)
        {
            try
            {
                TelegramBot = new TelegramBotClient(token);

                if(fileToSend.Content==null)
                    await TelegramBot.SetWebhookAsync(url + "/bot/", null);

                if (fileToSend.Content != null)
                    await TelegramBot.SetWebhookAsync(url + "/bot/", fileToSend);


                return true;
            }

            catch
            {
                return false;
            }
        }

        [HttpGet]
        public string GetNgrokUrl()
        {
            try
            {
                var url = "http://127.0.0.1:4040/status";
                var web = new HtmlWeb();
                var doc = web.Load(url);

                var res = doc.ParsedText.IndexOf(".ngrok.io");

                return doc.ParsedText.Substring(res - 16, 25);
            }

            catch
            {
                return null;
            }
        }

        private void InsertNewBotToDb(string token, string name, string Url, bool IsServerVersion=false)
        {
            if(db==null)
                db = new MarketBotDbContext();

            if (token != null && name != null && Url != null)
            {
                var spl = token.Split(':');
                int chat_id = Convert.ToInt32(spl[0]);

                BotInfo botInfo = new BotInfo
                {
                    Token = token,
                    Name = name,
                    WebHookUrl = Url,
                    Timestamp = DateTime.Now,
                    HomeVersion = !IsServerVersion,
                    ServerVersion = IsServerVersion,
                    ChatId = chat_id

                };

                db.BotInfo.Add(botInfo);
                db.SaveChanges();

                var conf = new Configuration { BotInfoId=botInfo.Id, VerifyTelephone = false, OwnerPrivateNotify = false, Delivery = true, Pickup = false, ShipPrice = 0, FreeShipPrice = 0, CurrencyId = 1 };
                db.Configuration.Add(conf);
                db.SaveChanges();

                Company company = new Company { Instagram = "https://www.instagram.com/", Vk = "https://www.vk.com/", Chanel = "https://t.me/", Chat = "https://t.me/" };
                db.Company.Add(company);
                db.SaveChanges();
            }
        }
    }
}