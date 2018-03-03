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
    public class NewFeedBackAddedMessage:BotMessage
    {
        private Orders Order { get; set; }

        private InlineKeyboardCallbackButton OpenOrderBtn { get; set; }

        private int OrderId { get; set; }
        public NewFeedBackAddedMessage(Orders order)
        {
            this.Order = order;
        }

        public NewFeedBackAddedMessage(int OrderId)
        {
            this.OrderId = OrderId;
        }

        public override BotMessage BuildMsg()
        {
            if (Order == null && OrderId > 0)
                using (MarketBotDbContext db = new MarketBotDbContext())
                    this.Order = db.Orders.Where(o => o.Id == OrderId).Include(o => o.FeedBack).FirstOrDefault();


                    if (Order != null)
                    {
                        base.TextMessage = Bold("Новый отзыв:") + NewLine() + "Добавлен отзыв к заказу №" + Order.Number.ToString();
                        OpenOrderBtn = new InlineKeyboardCallbackButton("Посмотреть детали заказа", BuildCallData(OrderProccesingBot.CmdGetOrderAdmin, OrderProccesingBot.ModuleName, Order.Id));

                        base.MessageReplyMarkup = new InlineKeyboardMarkup(
                        new[]{
                        new[]
                        {
                            OpenOrderBtn
                        },


                        });

                    }

            return this;
        }
    }
}
