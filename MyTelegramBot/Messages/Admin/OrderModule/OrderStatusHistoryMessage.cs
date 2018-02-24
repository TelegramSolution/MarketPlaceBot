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

namespace MyTelegramBot.Messages.Admin
{
    public class OrderStatusHistoryMessage:BotMessage
    {

        private List<OrderStatus> OrderStatusList { get; set; }

        private MarketBotDbContext db;

        private int OrderId { get; set;}


        public OrderStatusHistoryMessage (int order_id)
        {
            OrderId = order_id;
            base.BackBtn=BuildInlineBtn("Назад",BuildCallData(OrderProccesingBot.CmdBackToOrder,OrderProccesingBot.ModuleName,OrderId));
        }

        public override BotMessage BuildMsg()
        {
            db = new MarketBotDbContext();

            OrderStatusList = db.OrderStatus.Where(s => s.OrderId == OrderId && s.Enable).Include(s => s.Status).OrderBy(s=>s.Id).ToList();

            int counter = 1;

            foreach (OrderStatus os in OrderStatusList)
            {
               base.TextMessage+=counter.ToString() + ") Статус: " + os.Status.Name + " | Комментарий: " + os.Text + " | Пользователь: "
                    + Bot.GeneralFunction.FollowerFullName(os.FollowerId) + " | Время: " + os.Timestamp.ToString() + NewLine() + NewLine();

                counter++;
            }

            base.MessageReplyMarkup = new InlineKeyboardMarkup(
            new[]{
                    new[]
                    {
                        BackBtn
                    },
                });

            return this;
        }
    }
}
