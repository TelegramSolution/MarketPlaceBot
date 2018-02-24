using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot.AdminModule;
using MyTelegramBot.Bot;

namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// сообщение с подтверждением нового статуса заказа
    /// </summary>
    public class OrderStatusConfirmMessage:BotMessage
    {

        private InlineKeyboardCallbackButton SaveStatusBtn { get; set; }

        private InlineKeyboardCallbackButton AddCommentBtn { get; set; }

        private InlineKeyboardCallbackButton OpenBtn { get; set; }

        private Orders Order { get; set; }

        private OrderStatus OrderStatus { get; set; }

        MarketBotDbContext db;

        public OrderStatusConfirmMessage(Orders order, OrderStatus NewOrderStatus)
        {
            this.Order = order;
            this.OrderStatus = NewOrderStatus;
        }

        public override BotMessage BuildMsg()
        {
            db = new MarketBotDbContext();

            if (this.OrderStatus != null && this.OrderStatus.Status == null)
                this.OrderStatus.Status = db.Status.Find(this.OrderStatus.StatusId);
                
            SaveStatusBtn=BuildInlineBtn("Сохранить",BuildCallData(OrderProccesingBot.CmdConfirmNewStatus, OrderProccesingBot.ModuleName,this.Order.Id,this.OrderStatus.Id),base.DoneEmodji);

            AddCommentBtn = BuildInlineBtn("Добавить комментарий", BuildCallData("StatusAddComment", OrderProccesingBot.ModuleName, this.OrderStatus.Id), base.PenEmodji);

            BackBtn = BuildInlineBtn("Назад", BuildCallData(OrderProccesingBot.CmdBackToStatusEditor, OrderProccesingBot.ModuleName, this.Order.Id, this.OrderStatus.Id));

            base.TextMessage = "Заказ № " + this.Order.Number.ToString() + NewLine() +
                Bold("Новый статус:") + this.OrderStatus.Status.Name + NewLine() + 
                Bold("Комментарий:")+this.OrderStatus.Text+NewLine()+
                Bold("Пользователь:") + Bot.GeneralFunction.FollowerFullName(this.OrderStatus.FollowerId);

            base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            BackBtn
                        },
                new[]
                        {
                            AddCommentBtn
                        },

                new[]
                        {
                            SaveStatusBtn
                        },

                });

            db.Dispose();

            return this;

        }

        public OrderStatusConfirmMessage BuildNotyfiMessage()
        {
            db = new MarketBotDbContext();

            if (this.OrderStatus != null && this.OrderStatus.Status == null)
                this.OrderStatus.Status = db.Status.Find(this.OrderStatus.StatusId);


            base.TextMessage = "Заказ № " + this.Order.Number.ToString() + NewLine() +
                Bold("Cтатус:") + this.OrderStatus.Status.Name + NewLine() +
                Bold("Комментарий:") + this.OrderStatus.Text + NewLine() +
                Bold("Пользователь:") + Bot.GeneralFunction.FollowerFullName(this.OrderStatus.FollowerId) + NewLine() +
                Bold("Время:") + this.OrderStatus.Timestamp.ToString();

            OpenBtn = BuildInlineBtn("Открыть", BuildCallData(OrderProccesingBot.CmdOpenOrder, OrderProccesingBot.ModuleName,Convert.ToInt32(this.OrderStatus.OrderId)));

            base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            OpenBtn
                        },

                 });

            return this;

        }
    }
}
