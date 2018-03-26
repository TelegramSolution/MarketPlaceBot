using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;
using MyTelegramBot.Bot.AdminModule;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages.Admin.Notification
{
    public class NotificationDetailsMessage:BotMessage
    {
        private MyTelegramBot.Notification Notification { get; set; }

        public NotificationDetailsMessage(MyTelegramBot.Notification notification)
        {
            Notification = notification;
        }

        public override BotMessage BuildMsg()
        {
            if (Notification != null)
            {
                base.TextMessage = "Рассылка №"+Notification.Id.ToString() + NewLine()
                                    +Bold("Время: ") + Notification.DateAdd.ToString()+NewLine()
                                    +Bold("Текст: ") + Notification.Text+NewLine();

                base.MessageReplyMarkup = new InlineKeyboardMarkup(new InlineKeyboardButton [] { BackToAdminPanelBtn() });

                return this;
            }

            else
                return null;
        }
    }
}
