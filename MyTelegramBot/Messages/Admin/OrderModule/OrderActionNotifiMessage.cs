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
    /// сообщение описывает только что совершенное действие по заказу. Взят в обработку, изменен статус заказа
    /// </summary>
    public class OrderActionNotifiMessage:BotMessage
    {

        private OrderStatus OrderStatus { get; set; }

        private OrdersInWork OrdersInWork { get; set; }

        private Orders Order { get; set; }

        private InlineKeyboardCallbackButton OpenOrderBtn { get; set; }

        private InlineKeyboardButton ToBotDialogBtn { get; set; }

        private InlineKeyboardButton ToUserDialogBtn { get; set; }

        public OrderActionNotifiMessage(Orders order,OrdersInWork ordersInWork)
        {
            this.OrdersInWork = ordersInWork;
            this.Order = order;
        }

        public OrderActionNotifiMessage (Orders order,OrderStatus orderStatus)
        {
            this.OrderStatus = orderStatus;
            this.Order = order;
        }

        public override BotMessage BuildMsg()
        {

            if (this.OrdersInWork != null && this.OrdersInWork.Follower != null && Order != null)
            {
                base.TextMessage = "Пользователь " + this.OrdersInWork.Follower.FirstName + " " + this.OrdersInWork.Follower.LastName
                                 + NewLine() + " взял в работу заказ №" + Order.Number.ToString();

                this.ToBotDialogBtn = InlineKeyboardButton.WithUrl("Перейти к боту", "https://t.me/" + GeneralFunction.GetBotName());

                this.ToUserDialogBtn = InlineKeyboardButton.WithUrl(OrdersInWork.Follower.FirstName + " " + OrdersInWork.Follower.LastName, "https://t.me/" + OrdersInWork.Follower.UserName);

                this.OpenOrderBtn = BuildInlineBtn("Открыть заказ", BuildCallData(OrderProccesingBot.CmdGetOrderAdmin, OrderProccesingBot.ModuleName, this.Order.Id));

                base.MessageReplyMarkup = SetKeyboard();

                return this;
            }

            if(this.OrderStatus !=null && this.OrderStatus.Follower!=null && this.OrderStatus.Status != null && this.Order!=null)
            {
                base.TextMessage = "Пользователь " + this.OrderStatus.Follower.FirstName + " " + this.OrderStatus.Follower.LastName
                 + NewLine() + " изменил статус заказа №" + Order.Number.ToString() +NewLine()+
                 "Статус:" + this.OrderStatus.Status.Name+ NewLine()+
                 "Комментарий:"+ this.OrderStatus.Text;

                this.ToBotDialogBtn = InlineKeyboardButton.WithUrl("Перейти к боту", "https://t.me/" + GeneralFunction.GetBotName());

                this.ToUserDialogBtn = InlineKeyboardButton.WithUrl(OrderStatus.Follower.FirstName + " " + OrderStatus.Follower.LastName, "https://t.me/" + OrderStatus.Follower.UserName);

                this.OpenOrderBtn = BuildInlineBtn("Открыть заказ", BuildCallData(OrderProccesingBot.CmdGetOrderAdmin, OrderProccesingBot.ModuleName, this.Order.Id));

                base.MessageReplyMarkup = SetKeyboard();

                return this;
            }


            else
                return null;
        }

        private InlineKeyboardMarkup SetKeyboard()
        {
          return  new InlineKeyboardMarkup(
                   new[]{
                    new[]
                    {
                        OpenOrderBtn
                    },
                    new[]
                    {
                        ToBotDialogBtn
                    },
                    new[]
                    {
                        ToUserDialogBtn
                    }

                    });
        }
    }
}
