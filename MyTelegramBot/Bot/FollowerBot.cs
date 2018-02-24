using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using MyTelegramBot.Messages;

namespace MyTelegramBot.Bot
{
    public class FollowerBot:Bot.BotCore
    {
        RequestPhoneNumberMessage RequestPhoneNumberMessageMsg { get; set; }

        OrderTempMessage OrderPreviewMsg { get; set; }
        public FollowerBot(Update _update) : base(_update)
        {

        }
        protected override void Constructor()
        {
            RequestPhoneNumberMessageMsg = new RequestPhoneNumberMessage(base.FollowerId);
            OrderPreviewMsg = new OrderTempMessage(base.FollowerId,BotInfo.Id);
        }

        public async override Task<IActionResult> Response()
        {
           

            //Пользователь отрпвил СВОЙ номер телефона
            if (Update.Message != null && Update.Message.Contact != null && Update.Message.From.Id == Update.Message.Contact.UserId)
                return await PreviewOrder();



            // Пользователь отправил НЕ СВОЙ номер телефона
            if (Update.Message != null && Update.Message.Contact != null && Update.Message.From.Id != Update.Message.Contact.UserId && Update.Message.ReplyToMessage != null)
                return await Error();


            else
                return null;
        }

        private async Task<IActionResult> PreviewOrder()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var follower =db.Follower.Where(f => f.ChatId == ChatId).FirstOrDefault();
                follower.Telephone = Update.Message.Contact.PhoneNumber;

                if (db.SaveChanges()>0 && await SendMessage(OrderPreviewMsg.BuildMessage()) != null)
                    return base.OkResult;

                else
                    return base.NotFoundResult;
            }
        }

        private async Task<IActionResult> Error()
        {
            if (await SendMessage(new BotMessage { TextMessage = "Вы должны отправить нам свой номер телефона!" }) != null)
                return base.OkResult;

            else
                return base.NotFoundResult;
        }


    }
}
