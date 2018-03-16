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
    /// <summary>
    /// сообщение со списком всех разосланных Рассылок
    /// </summary>
    public class NotificationListMessage:BotMessage
    {

        List<MyTelegramBot.Notification> NotificationList { get; set; }

        Dictionary<int,List<MyTelegramBot.Notification>> Pages { get; set; }

        private InlineKeyboardCallbackButton CreateNotificationBtn { get; set; }

        public NotificationListMessage(int SelectPageNumber=1)
        {
            base.SelectPageNumber = SelectPageNumber;
        }

        public override BotMessage BuildMsg()
        {
            NotificationList = NotificationFunction.NotificationList();

            base.PageSize = 5;

            Pages = base.BuildDataPage<MyTelegramBot.Notification>(NotificationList, base.PageSize);

            CreateNotificationBtn = BuildInlineBtn("Создать рассылку", BuildCallData(NotificationBot.NotificationCreateCmd,NotificationBot.ModuleName));

            MessageReplyMarkup = PageNavigatorKeyboard<MyTelegramBot.Notification>(Pages, 
                NotificationBot.NotificationViewCmd, NotificationBot.ModuleName, BackToAdminPanelBtn(),
                new InlineKeyboardButton[] { CreateNotificationBtn });

            base.TextMessage = "Список рассылок:"+NewLine()+NewLine();

            if(Pages!=null && Pages.Count>0 && Pages.Count >= SelectPageNumber && Pages[SelectPageNumber]!=null)
            {
                var page = Pages[SelectPageNumber];

                foreach(var notifi in page)
                {
                    base.TextMessage += "Рассылка №" + notifi.Id.ToString() + " /notifi" + notifi.Id.ToString() + NewLine() +
                                       "Дата:" + notifi.DateAdd.ToString() + NewLine() + NewLine();
                }
            }

            return this;
        }
    }
}
