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
using MyTelegramBot.BusinessLayer;

namespace MyTelegramBot.Messages.Admin.Notification
{
    public class EditorNotificationMessage:BotMessage
    {

        private MyTelegramBot.Notification Notification { get; set; }


        private InlineKeyboardCallbackButton SaveAndSendBtn { get; set; }


        private InlineKeyboardCallbackButton RemoveBtn { get; set; }

        public EditorNotificationMessage(MyTelegramBot.Notification notification)
        {
            Notification = notification;
        }

        public override BotMessage BuildMsg()
        {
            if (Notification != null)
            {
                base.TextMessage = base.GoldRhobmus + "Рассылка №" + Notification.Id.ToString() + NewLine() +
                    Bold("Дата: ") + Notification.DateAdd.ToString() + NewLine() +
                    Bold("Текст: ") + Notification.Text;

                RemoveBtn= BuildInlineBtn("Удалить", BuildCallData(NotificationBot.NotificationRemoveCmd, NotificationBot.ModuleName, Notification.Id), base.CrossEmodji);

                SaveAndSendBtn= BuildInlineBtn("Сохранить и отправить", BuildCallData(NotificationBot.NotificationSendCmd, NotificationBot.ModuleName, Notification.Id), base.SenderEmodji);

                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                    new[]{
                new[]
                        {
                            RemoveBtn
                        },
                new[]
                        {
                            SaveAndSendBtn
                        }

                     });

                return this;
            }

            else
            {
                return null;
            }

        }
    }
}
