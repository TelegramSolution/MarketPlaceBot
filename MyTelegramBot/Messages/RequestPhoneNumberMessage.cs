using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Сообщение с просьбой прислать свой номер телефона
    /// </summary>
    public class RequestPhoneNumberMessage : BotMessage
    {

        public RequestPhoneNumberMessage()
        {
            
        }

        public override BotMessage BuildMsg()
        {

            base.TextMessage = "Для дальнейшего оформления заказа отправьте нам свой номер телефона, что бы могли связаться с вами!" + NewLine() +
                                "Что отправить свой номер нажмите кнопку \"Отправить номер телефона\" ";
            base.CallBackTitleText = "Укажите номер телефона";

            Telegram.Bot.Types.KeyboardButton[] b = new KeyboardButton[1];
            b[0] = SendTelephoneNumber();
            MessageReplyMarkup = new ReplyKeyboardMarkup(b, true, true);
            return this;

        }

        private KeyboardButton SendTelephoneNumber()
        {
            Telegram.Bot.Types.KeyboardButton btn = new Telegram.Bot.Types.KeyboardButton("Отправить номер телефона");
            btn.RequestContact = true;
            return btn;
        }
    }
}
