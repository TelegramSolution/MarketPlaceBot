using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using MyTelegramBot.Messages;
using MyTelegramBot.Bot.Core;
using MyTelegramBot.BusinessLayer;

namespace MyTelegramBot.Bot
{
    public class FollowerBot:BotCore
    {
        RequestPhoneNumberMessage RequestPhoneNumberMessageMsg { get; set; }

        OrderTempMessage OrderPreviewMsg { get; set; }
        public FollowerBot(Update _update) : base(_update)
        {

        }
        protected override void Initializer()
        {
            RequestPhoneNumberMessageMsg = new RequestPhoneNumberMessage();
            OrderPreviewMsg = new OrderTempMessage(base.FollowerId,BotInfo.Id);
        }

        public async override Task<IActionResult> Response()
        {
           

            //Пользователь отрпвил СВОЙ номер телефона
            if (Update.Message != null && Update.Message.Contact != null && Update.Message.From.Id == Update.Message.Contact.UserId)
                return await InsertTelephoneNumber();



            // Пользователь отправил НЕ СВОЙ номер телефона
            if (Update.Message != null && Update.Message.Contact != null && Update.Message.From.Id != Update.Message.Contact.UserId && Update.Message.ReplyToMessage != null)
                return await Error();


            else
                return null;
        }

        /// <summary>
        /// Добавить номер телефона клиента
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> InsertTelephoneNumber()
        {
            if (FollowerFunction.AddTelephoneNumber(FollowerId, Update.Message.Contact.PhoneNumber) != null)
                await SendMessage(OrderPreviewMsg.BuildMessage());

            else
                await SendMessage(new BotMessage { TextMessage = "Ошибка!" });

            return OkResult;
        }

        private async Task<IActionResult> Error()
        {
            if (await SendMessage(new BotMessage { TextMessage = "Вы должны отправить нам свой номер телефона!" }) != null)
                return base.OkResult;

            else
                return base.OkResult;
        }


    }
}
