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
    [Route("bot/")]

    [Produces("application/json")]
    public class ValuesController : Controller
    {
        private CategoryBot Category { get; set; }

        private BotCommand Command { get; set; }

        private ProductBot Product { get; set; }

        private BasketBot Basket { get; set; }

        private AddressBot Address { get; set; }

        private OrderBot OrderBot { get; set; }

        private FollowerBot FollowerBot { get; set; }

        private OrderPositionBot PositionBot { get; set; }

        private ProductEditBot ProductEditBot { get; set; }

        private CategoryEditBot CategoryEditBot { get; set; }

        private MainMenuBot MainMenuBot { get; set; }

        private AdminBot AdminBot { get; set; }

        private HelpDeskBot HelpDeskBot { get; set; }

        private Bot.AdminModule.OrderProccesingBot OrderProccesingBot { get; set; }

        private HelpDeskProccessingBot HelpDeskProccessingBot { get; set; }

        private MoreSettingsBot MoreSettingsBot { get; set; }

        private ProductAddBot ProductAddBot { get; set; }

        protected OkResult OkResult { get; set; }

        protected NotFoundResult NotFoundResult { get; set; }

        private string ModuleName { get; set; }

        IActionResult Result { get; set; }
        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            OkResult = this.Ok();
            NotFoundResult = this.NotFound();

            if (Result == null && update.CallbackQuery == null)
            {
                ProductAddBot = new ProductAddBot(update);
                Result = await ProductAddBot.Response();

            }

            if (update != null && update.CallbackQuery != null && update.CallbackQuery.Data != null)
            {
                ModuleName = JsonConvert.DeserializeObject<BotCommand>(update.CallbackQuery.Data).M;
            }

            if (Result == null && ModuleName != null && ModuleName == MoreSettingsBot.ModuleName || Result == null && ModuleName == null)
            {
                MoreSettingsBot = new MoreSettingsBot(update);
                Result = await MoreSettingsBot.Response();
            }

            if (Result == null && ModuleName != null && ModuleName == HelpDeskProccessingBot.ModuleName || Result == null && ModuleName == null)
            {
                HelpDeskProccessingBot = new HelpDeskProccessingBot(update);
                Result = await HelpDeskProccessingBot.Response();
            }

            if (Result == null && ModuleName != null && ModuleName == OrderProccesingBot.ModuleName || Result == null && ModuleName == null)
            {
                OrderProccesingBot = new OrderProccesingBot(update);
                Result = await OrderProccesingBot.Response();
            }

            if (Result == null && ModuleName != null && ModuleName == CategoryBot.ModuleName || Result == null && ModuleName == null)
            {
                Category = new CategoryBot(update);
                Result = await Category.Response();
            }

            if (Result == null && ModuleName != null && ModuleName == ProductBot.ModuleName || Result == null && ModuleName == null)
            {
                Product = new ProductBot(update);
                Result = await Product.Response();
            }

            if (Result == null && ModuleName != null && ModuleName == BasketBot.ModuleName || Result == null && ModuleName == null)
            {
                Basket = new BasketBot(update);
                Result = await Basket.Response();
            }

            if (Result == null && ModuleName != null && ModuleName == AddressBot.ModuleName || Result == null && ModuleName == null)
            {
                Address = new AddressBot(update);
                Result = await Address.Response();
            }

            if (Result == null && ModuleName != null && ModuleName == OrderBot.ModuleName || Result == null && ModuleName == null
            || Result == null && update.PreCheckoutQuery != null)
            {
                OrderBot = new OrderBot(update);
                Result = await OrderBot.Response();
            }

            if (update.Message != null && Result == null)
            {
                FollowerBot = new FollowerBot(update);
                Result = await FollowerBot.Response();
            }

            if (Result == null && ModuleName != null && ModuleName == OrderPositionBot.ModuleName || Result == null && ModuleName == null)
            {
                PositionBot = new OrderPositionBot(update);
                Result = await PositionBot.Response();
            }

            if (Result == null && ModuleName != null && ModuleName == AdminBot.ModuleName || Result == null && ModuleName == null)
            {
                AdminBot = new AdminBot(update);
                Result = await AdminBot.Response();
            }

            if (Result == null && ModuleName != null && ModuleName == ProductEditBot.ModuleName || Result == null && ModuleName == null)
            {
                ProductEditBot = new ProductEditBot(update);
                Result = await ProductEditBot.Response();
            }

            if (Result == null && ModuleName != null && ModuleName == CategoryEditBot.ModuleName || Result == null && ModuleName == null)
            {
                CategoryEditBot = new CategoryEditBot(update);
                Result = await CategoryEditBot.Response();
            }

            if (Result == null && ModuleName != null && ModuleName == MainMenuBot.ModuleName || Result == null && ModuleName == null)
            {
                MainMenuBot = new MainMenuBot(update);
                Result = await MainMenuBot.Response();
            }


            if (Result == null && ModuleName != null && ModuleName == HelpDeskBot.ModuleName || Result == null && ModuleName == null)
            {
                HelpDeskBot = new HelpDeskBot(update);
                Result = await HelpDeskBot.Response();
            }



                await AddUpdateMsgToDb(update);

                if (update.Message != null)
                    await UpdateFollowerInfo(update.Message.Chat);

                if (update.CallbackQuery != null && update.CallbackQuery.Message != null)
                    await UpdateFollowerInfo(update.CallbackQuery.Message.Chat);

                //делаем так что бы наше приложние всегда отвечало телеграму ОК. В противном случаем телеграм так и будет слать нам это сообщения в ожиданиее ответа ОК
                if (Result == null || Result != null)
                    Result = Ok();


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
            async Task<int> UpdateFollowerInfo(Chat ChatInfo)
            {
                try
                {
                    using (MarketBotDbContext db = new MarketBotDbContext())
                    {
                        var follower = db.Follower.Where(f => f.ChatId == ChatInfo.Id).FirstOrDefault();

                        if (follower != null && follower.FirstName != ChatInfo.FirstName)
                        {
                            follower.FirstName = ChatInfo.FirstName;
                            return await db.SaveChangesAsync();
                        }

                        if (follower != null && follower.LastName != ChatInfo.LastName)
                        {
                            follower.LastName = ChatInfo.LastName;
                            return await db.SaveChangesAsync();
                        }

                        if (follower != null && follower.UserName != ChatInfo.Username)
                        {
                            follower.UserName = ChatInfo.Username;
                            return await db.SaveChangesAsync();
                        }

                        else
                            return -1;
                    }
                }

                catch
                {
                    return -1;
                }
            }
        }
    }
