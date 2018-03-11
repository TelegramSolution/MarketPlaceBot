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
    public class OrderStatusEditorMessage:BotMessage
    {
        private MarketBotDbContext db { get; set; }
        private InlineKeyboardCallbackButton [][] StatusBtns { get; set; }

        private List<Status> StatusList { get; set; }

        private Orders Order { get; set; }

        public OrderStatusEditorMessage (Orders order)
        {
            this.Order = order;
            BackBtn = BuildInlineBtn("Назад", BuildCallData(OrderProccesingBot.CmdBackToOrder, OrderProccesingBot.ModuleName, Order.Id));

        }

        public override BotMessage BuildMsg()
        {
            db = new MarketBotDbContext();

            StatusList = db.Status.Where(s => s.Enable).ToList();

            StatusBtns = new InlineKeyboardCallbackButton[StatusList.Count + 1][];

            int counter = 0;

            foreach (Status status in StatusList)
            {
                StatusBtns[counter] = new InlineKeyboardCallbackButton[1];
                StatusBtns[counter][0] = base.BuildInlineBtn(status.Name, 
                    BuildCallData(OrderProccesingBot.CmdUpdateOrderStatus, OrderProccesingBot.ModuleName, Order.Id, status.Id));
                counter++;
            }

            StatusBtns[StatusBtns.Length-1] = new InlineKeyboardCallbackButton[1];
            StatusBtns[StatusBtns.Length - 1][0] = BackBtn;

            base.TextMessage =base.BlueRhombus+ " Изменить статус заказа № " + Order.Number.ToString();

            base.MessageReplyMarkup = new InlineKeyboardMarkup(StatusBtns);

            return this;
        }
    }
}
