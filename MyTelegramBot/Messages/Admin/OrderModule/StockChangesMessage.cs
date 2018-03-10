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


namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// Сообщение с текущими остатками по товорам которые только что бы приобретены. 
    /// Это сообщение отсылается после того как Оператор нажал "Выполнено"
    /// </summary>
    public class StockChangesMessage:BotMessage
    {
        private List<IGrouping<Product, Stock>> StockList { get; set; }

        MarketBotDbContext db { get; set; }

        private InlineKeyboardCallbackButton OpenOrderBtn { get; set; }

        private int OrderId { get; set; }
        public StockChangesMessage(List<IGrouping<Product, Stock>> stock, int OrderID)
        {
            this.StockList = stock;
            this.OrderId = OrderID;
        }

        public override BotMessage BuildMsg()
        {
            // db = new MarketBotDbContext();
            if (StockList!=null && OrderId > 0)
            {
                base.TextMessage = Bold("Изменения в остатках:") + NewLine();


                foreach (var group in StockList)
                {
                    base.TextMessage += NewLine() + base.BlueRhombus + group.Key.Name + " /adminproduct" + group.Key.Id.ToString() + NewLine() +
                        Bold("Было:") + (group.OrderBy(s => s.Balance).LastOrDefault().Balance - group.OrderBy(s => s.Balance).LastOrDefault().Quantity).ToString()
                        + " " + group.Key.Unit.ShortName + " " + NewLine() +
                        Bold("Стало:") + group.OrderBy(s => s.Balance).FirstOrDefault().Balance + " " + group.Key.Unit.ShortName + " " + NewLine();
                }

                OpenOrderBtn = BuildInlineBtn("Открыть заказ", BuildCallData(Bot.AdminModule.OrderProccesingBot.CmdOpenOrder, Bot.AdminModule.OrderProccesingBot.ModuleName, this.OrderId));

                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                    new[]
                    {
                        OpenOrderBtn
                    },
                    });

                return this;
            }

            else
            return null;
        }
    }
}
