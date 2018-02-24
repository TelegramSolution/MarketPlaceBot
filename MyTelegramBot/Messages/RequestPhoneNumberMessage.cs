using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using MyTelegramBot.Bot;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Сообщение с просьбой прислать свой номер телефона
    /// </summary>
    public class RequestPhoneNumberMessage : BotMessage
    {
        private int FollowerId { get; set; }

        private Follower Follower { get; set; }

        public RequestPhoneNumberMessage(int FollowerId)
        {
            this.FollowerId = FollowerId;
        }

        public override BotMessage BuildMsg()
        {
            using(MarketBotDbContext db=new MarketBotDbContext())
             Follower= db.Follower.Where(f => f.Id == FollowerId).FirstOrDefault();

            if (Follower != null && Follower.Telephone ==null)
            {
                base.TextMessage = "Для дальнейшего оформления заказа отправьте нам свой номер телефона, что бы могли связаться с вами!" + NewLine() +
                                    "Что отправить свой номер нажмите кнопку \"Отправить номер телефона\" ";
                base.CallBackTitleText = "Укажите номер телефона";

                Telegram.Bot.Types.KeyboardButton[] b = new KeyboardButton[1];
                b[0] = SendTelephoneNumber();
                MessageReplyMarkup = new ReplyKeyboardMarkup(b, true, true);
                return this;
            }

            else
                return null;
        }

        private KeyboardButton SendTelephoneNumber()
        {
            Telegram.Bot.Types.KeyboardButton btn = new Telegram.Bot.Types.KeyboardButton("Отправить номер телефона");
            btn.RequestContact = true;
            return btn;
        }
    }
}
