using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using MyTelegramBot.Bot;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using MyTelegramBot.Bot.AdminModule;
using Microsoft.Extensions.Configuration;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Controllers
{
    /// <summary>
    /// ВЕБХУК
    /// </summary>
    [Route("bot/")]

    [Produces("application/json")]

   
    public class ValuesController : Controller
    {
        
       
        private OkResult OkResult { get; set; }

        private BotCore BotCore { get; set; }

        private string ModuleName { get; set; }

        IActionResult Result { get; set; }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            OkResult = this.Ok();

            if (Result == null && update.CallbackQuery == null && update.InlineQuery==null)
            {
                BotCore = new ProductAddBot(update);
                Result = await BotCore.Response();

            }

            if (update.InlineQuery != null)
            {
                InlineFind find = new InlineFind(update.InlineQuery);
                await find.Response();
                return Ok();
            }


            if (update != null && update.CallbackQuery != null && update.CallbackQuery.Data != null && update.InlineQuery == null)
            {
                ModuleName = JsonConvert.DeserializeObject<BotCommand>(update.CallbackQuery.Data).M;
                
            }

            if(update.InlineQuery == null)
            {
                if (Result == null && ModuleName != null && ModuleName == MoreSettingsBot.ModuleName || Result == null && ModuleName == null)
                {
                    BotCore = new MoreSettingsBot(update);
                    Result = await BotCore.Response();
                }

                if (Result == null && ModuleName != null && ModuleName == HelpDeskProccessingBot.ModuleName || Result == null && ModuleName == null && update.InlineQuery == null)
                {
                    BotCore = new HelpDeskProccessingBot(update);
                    Result = await BotCore.Response();
                }

                if (Result == null && ModuleName != null && ModuleName == OrderProccesingBot.ModuleName || Result == null && ModuleName == null)
                {
                    BotCore = new OrderProccesingBot(update);
                    Result = await BotCore.Response();
                }

                if (Result == null && ModuleName != null && ModuleName == CategoryBot.ModuleName || Result == null && ModuleName == null)
                {
                    BotCore = new CategoryBot(update);
                    Result = await BotCore.Response();
                }

                if (Result == null && ModuleName != null && ModuleName == ProductBot.ModuleName || Result == null && ModuleName == null)
                {
                    BotCore = new ProductBot(update);
                    Result = await BotCore.Response();
                }

                if (Result == null && ModuleName != null && ModuleName == BasketBot.ModuleName || Result == null && ModuleName == null)
                {
                    BotCore = new BasketBot(update);
                    Result = await BotCore.Response();
                }

                if (Result == null && ModuleName != null && ModuleName == AddressBot.ModuleName || Result == null && ModuleName == null ||
                    Result==null && update.Message!=null && update.Message.Location!=null)
                {
                    BotCore = new AddressBot(update);
                    Result = await BotCore.Response();
                }

                if (Result == null && ModuleName != null && ModuleName == OrderBot.ModuleName || Result == null && ModuleName == null
                || Result == null && update.PreCheckoutQuery != null)
                {
                    BotCore = new OrderBot(update);
                    Result = await BotCore.Response();
                }

                if (update.Message != null && Result == null)
                {
                    BotCore = new FollowerBot(update);
                    Result = await BotCore.Response();
                }

                if (Result == null && ModuleName != null && ModuleName == OrderPositionBot.ModuleName || Result == null && ModuleName == null)
                {
                    BotCore = new OrderPositionBot(update);
                    Result = await BotCore.Response();
                }

                if (Result == null && ModuleName != null && ModuleName == AdminBot.ModuleName || Result == null && ModuleName == null)
                {
                    BotCore = new AdminBot(update);
                    Result = await BotCore.Response();
                }

                if (Result == null && ModuleName != null && ModuleName == ProductEditBot.ModuleName || Result == null && ModuleName == null)
                {
                    BotCore = new ProductEditBot(update);
                    Result = await BotCore.Response();
                }

                if (Result == null && ModuleName != null && ModuleName == CategoryEditBot.ModuleName || Result == null && ModuleName == null)
                {
                    BotCore = new CategoryEditBot(update);
                    Result = await BotCore.Response();
                }

                if (Result == null && ModuleName != null && ModuleName == MainMenuBot.ModuleName || Result == null && ModuleName == null)
                {
                    BotCore = new MainMenuBot(update);
                    Result = await BotCore.Response();
                }


                if (Result == null && ModuleName != null && ModuleName == HelpDeskBot.ModuleName || Result == null && ModuleName == null)
                {
                    BotCore = new HelpDeskBot(update);
                    Result = await BotCore.Response();
                }
            }

            //делаем так что бы наше приложние всегда отвечало телеграму ОК. 
            //В противном случаем телеграм так и будет слать нам это сообщения в ожиданиее ответа ОК
            if (Result == null || Result != null)
                Result = Ok();

            if (update.Message != null && update.Message.Chat!=null)
            {
                BusinessLayer.FollowerFunction.InsertFollower(Convert.ToInt32(update.Message.Chat.Id), 
                    update.Message.Chat.FirstName, update.Message.Chat.LastName, update.Message.Chat.Username);
            }

            if (update.CallbackQuery != null && update.CallbackQuery.Message != null && update.CallbackQuery.Message.Chat!=null)
                BusinessLayer.FollowerFunction.InsertFollower(Convert.ToInt32(update.CallbackQuery.Message.Chat.Id), 
                    update.CallbackQuery.Message.Chat.FirstName, update.CallbackQuery.Message.Chat.LastName, update.CallbackQuery.Message.Chat.Username);


            AddUpdateMsgToDb(update);

             return Result;
            }

            async Task<int> AddUpdateMsgToDb(Update _update)
            {
                Follower follower = new Follower();
                string MessageId = "";
                int BotId = 0;
                try
                {
                    using (MarketBotDbContext db = new MarketBotDbContext())
                    {
                        string name = GeneralFunction.GetBotName();

                        if (_update.CallbackQuery != null && _update.InlineQuery == null)
                        {
                            follower = await db.Follower.Where(f => f.ChatId == _update.CallbackQuery.From.Id).FirstOrDefaultAsync();

                        if(_update.CallbackQuery.Message!=null)
                            MessageId = _update.CallbackQuery.Message.MessageId.ToString();


                        }

                        if (_update.Message != null && _update.InlineQuery == null)
                        {
                            follower = await db.Follower.Where(f => f.ChatId == _update.Message.From.Id).FirstOrDefaultAsync();
                            MessageId = _update.Message.MessageId.ToString();

                        }

                        BotId = db.BotInfo.Where(b => b.Name == name).FirstOrDefault().Id;

                        if (follower != null && db.TelegramMessage.Where(t => t.UpdateId == _update.Id).FirstOrDefault() == null && BotId > 0 && _update.InlineQuery == null)
                        {

                            TelegramMessage telegramMessage = new TelegramMessage
                            {
                                FollowerId = follower.Id,
                                MessageId = MessageId,
                                UpdateId = _update.Id,
                                UpdateJson = JsonConvert.SerializeObject(_update),
                                DateAdd = DateTime.Now,
                                BotInfoId = BotId
                            };

                            db.TelegramMessage.Add(telegramMessage);
                            return await db.SaveChangesAsync();
                        }

                        else
                            return -1;

                    }
                }

                catch (Exception exp)
                {
                    return -1;
                }
            }

            /// <summary>
            /// Проверяем информацию о пользователе. Если есть изменения, то вносим их в БД
            /// </summary>
            /// <returns></returns>
            //async Task<int> UpdateFollowerInfo(Chat ChatInfo)
            //{
            //    try
            //    {
            //        using (MarketBotDbContext db = new MarketBotDbContext())
            //        {
            //            var follower = db.Follower.Where(f => f.ChatId == ChatInfo.Id).FirstOrDefault();

            //            if (follower != null && follower.FirstName != ChatInfo.FirstName)
            //            {
            //                follower.FirstName = ChatInfo.FirstName;
            //                return await db.SaveChangesAsync();
            //            }

            //            if (follower != null && follower.LastName != ChatInfo.LastName)
            //            {
            //                follower.LastName = ChatInfo.LastName;
            //                return await db.SaveChangesAsync();
            //            }

            //            if (follower != null && follower.UserName != ChatInfo.Username)
            //            {
            //                follower.UserName = ChatInfo.Username;
            //                return await db.SaveChangesAsync();
            //            }

            //            else
            //                return -1;
            //        }
            //    }

            //    catch
            //    {
            //        return -1;
            //    }
            //}
        }
    }
