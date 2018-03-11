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
    /// Если заказ или заявка уже обрабатываются кем-то, то после нажатия на кнопку "взять в работу"
    /// придет сообщение с вопросом о назначении исполнителем себя вместо кого-то
    /// </summary>
    public class OverridePerformerConfirmMessage:BotMessage
    {
        private InlineKeyboardCallbackButton ConfirmOverrideBtn { get; set; }

        private Orders Order { get; set; }

        private OrdersInWork OrdersInWork { get; set; }

        private HelpDesk HelpDesk { get; set; }

        private HelpDeskInWork HelpDeskInWork { get; set; }

        public OverridePerformerConfirmMessage (Orders order, OrdersInWork ordersInWork)
        {
            Order = order;
            OrdersInWork=ordersInWork;
            BackBtn = BuildInlineBtn("Назад", BuildCallData(OrderProccesingBot.CmdBackOverride, OrderProccesingBot.ModuleName, Order.Id));

        }

        public OverridePerformerConfirmMessage(HelpDesk help, HelpDeskInWork helpDeskInWork)
        {
            HelpDesk = help;
            HelpDeskInWork = helpDeskInWork;
        }

        public override BotMessage BuildMsg()
        {

            if (Order != null)
                OrderOverrideMsg();

            if (HelpDesk != null)
                HelpOverrideMsg();

            return this;
        }

        private void HelpOverrideMsg()
        {
            base.TextMessage = "Заявка №" + this.HelpDesk.Number + " находится в обработке у пользователя:" +
           Bot.GeneralFunction.FollowerFullName(this.HelpDeskInWork.FollowerId) + NewLine() +
           "Время:" + this.HelpDeskInWork.Timestamp.ToString() + NewLine();

            ConfirmOverrideBtn = BuildInlineBtn("Назначить себя исполнителем", BuildCallData(HelpDeskProccessingBot.CmdOverridePerformerHelp, HelpDeskProccessingBot.ModuleName, this.Order.Id));


            base.MessageReplyMarkup = new InlineKeyboardMarkup(
            new[]{
                   new[]
                    {
                        ConfirmOverrideBtn
                    },

                    new[]
                    {
                        BackBtn
                    },
                });
        }

        private void OrderOverrideMsg()
        {
            base.TextMessage = "Заказ №" + this.Order.Number + " находится в обработке у пользователя:" +
               Bot.GeneralFunction.FollowerFullName(this.OrdersInWork.FollowerId) + NewLine() +
               "Время:" + this.OrdersInWork.Timestamp.ToString() + NewLine();

            ConfirmOverrideBtn = BuildInlineBtn("Назначить себя исполнителем", BuildCallData(OrderProccesingBot.CmdOverridePerformerOrder, OrderProccesingBot.ModuleName, this.Order.Id));


            base.MessageReplyMarkup = new InlineKeyboardMarkup(
            new[]{
                   new[]
                    {
                        ConfirmOverrideBtn
                    },

                    new[]
                    {
                        BackBtn
                    },
                });
        }
    }
}
