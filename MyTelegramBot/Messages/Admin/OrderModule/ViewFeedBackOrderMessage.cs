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
using MyTelegramBot.BusinessLayer;

namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// сообщение отзывом к заказу (по всем позициям в заказе)
    /// </summary>
    public class ViewFeedBackOrderMessage:BotMessage
    {

        int OrderId { get; set; }

        List<FeedBack> FeedBackList { get; set; }



        public ViewFeedBackOrderMessage(int OrderId)
        {
            this.OrderId = OrderId;
        }

        public override BotMessage BuildMsg()
        {
            FeedBackList = FeedbackFunction.GetFeedBackByOrderId(OrderId);

            if (FeedBackList.Count > 0)
            {
                BackBtn = BuildInlineBtn("К заказу", BuildCallData(OrderProccesingBot.CmdBackToOrder, OrderProccesingBot.ModuleName, OrderId),base.Previuos2Emodji);

                int i = 1;

                foreach(var feed in FeedBackList)
                {
                    base.TextMessage += i.ToString() + ") " + feed.Product.Name + NewLine() +
                        Bold("Оценка:") + feed.RaitingValue.ToString() + NewLine() +
                        Bold("Комментарий:") + feed.Text + NewLine() +
                        Bold("Время:")+feed.DateAdd.ToString()+NewLine()
                        + NewLine();

                    i++;
                }

                base.MessageReplyMarkup = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        BackBtn
                    }
                });

                return this;
            }

            return null;
        }
    }
}
