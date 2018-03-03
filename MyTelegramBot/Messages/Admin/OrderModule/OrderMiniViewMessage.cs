using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot.AdminModule;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages.Admin
{
    public class OrderMiniViewMessage:BotMessage
    {
        private int OrderId { get; set; }

        private InlineKeyboardCallbackButton OpenBtn { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Text">текст сообщения</param>
        /// <param name="OrderId">айди заказа</param>
        public OrderMiniViewMessage(string Text, int OrderId)
        {
            this.OrderId = OrderId;

            base.TextMessage = Text;
        }

        public override BotMessage BuildMsg()
        {
            OpenBtn = new InlineKeyboardCallbackButton("Открыть", BuildCallData("OpenOrder", OrderProccesingBot.ModuleName, OrderId));

            base.MessageReplyMarkup = new InlineKeyboardMarkup(new[] { new[] { OpenBtn } });

            return this;
        }
    }
}
