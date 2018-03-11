using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// Мини сообщение с деталями последнего действия по заявке и кнопкой Подробнее
    /// </summary>
    public class HelpDeskMiniViewMessage : BotMessage
    {
        private int HelpDeskId { get; set; }

        private InlineKeyboardCallbackButton OpenBtn { get; set; }

        public HelpDeskMiniViewMessage(string TextMessage, int HelpDeskId)
        {
            this.HelpDeskId = HelpDeskId;
            base.TextMessage = TextMessage;
        }

        public override BotMessage BuildMsg()
        {
            OpenBtn = new InlineKeyboardCallbackButton("Открыть", BuildCallData(Bot.AdminModule.HelpDeskProccessingBot.GetHelpDeskCmd, Bot.AdminModule.HelpDeskProccessingBot.ModuleName, HelpDeskId));

            base.MessageReplyMarkup = new InlineKeyboardMarkup(new[] { new[] { OpenBtn } });

            return this;
        }

    }
}
