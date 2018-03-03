using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;
using MyTelegramBot.Bot.AdminModule;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// через бота можно настраивать только Qiwi, Яндекс кассу
    /// </summary>
    public class AdminPayMethodsSettings:BotMessage
    {
        private InlineKeyboardCallbackButton QiwiBtn { get; set; }

        private InlineKeyboardCallbackButton DebitCardYandexBtn { get; set; }



        public override BotMessage BuildMsg()
        {
            base.TextMessage = "Выберите";

            QiwiBtn = BuildInlineBtn("Qiwi", BuildCallData(MoreSettingsBot.QiwiListCmd, MoreSettingsBot.ModuleName));

            DebitCardYandexBtn = BuildInlineBtn("Яндекс касса", BuildCallData(MoreSettingsBot.ViewYandexKassaCmd, MoreSettingsBot.ModuleName));

            BackBtn = BuildInlineBtn("Назад", BuildCallData(MoreSettingsBot.BackToMoreSettingsCmd, MoreSettingsBot.ModuleName),base.Previuos2Emodji,false);


            base.MessageReplyMarkup = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    QiwiBtn
                },
                new[]
                {
                    DebitCardYandexBtn
                },
                new[]
                {
                    BackBtn
                }
            });

            return this;
        }
    }
}
