using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages
{
    public class MyOrdersMessage:BotMessage
    {
        private int FollowerId { get; set; }

        private int BotId { get; set; }
        public MyOrdersMessage(int FollowerId, int BotId)
        {
            this.FollowerId = FollowerId;
            this.BotId = BotId;
        }

        public override BotMessage BuildMsg()
        {
            using(MarketBotDbContext db=new MarketBotDbContext())
            {
                List<Orders> orders = new List<Orders>();

                orders = db.Orders.Where(o => o.FollowerId == FollowerId && o.BotInfoId==BotId).Include(o=>o.OrderProduct).Include(o=>o.Done).Include(o=>o.Confirm).Include(o=>o.Delete).OrderBy(o=>o.Id).ToList();

                if (orders != null && orders.Count>0)
                {
                    base.TextMessage = Bold("Мои заказы") + NewLine();

                    int Counter = 1;

                    foreach (Orders order in orders)
                    {
                        if (order.Delete == null)
                        {
                            base.TextMessage += Counter.ToString() + ") Заказ №" + order.Number.ToString() + " от " + order.DateAdd.ToString() +
                                    NewLine() + "открыть /myorder" + order.Number.ToString() + NewLine();
                            Counter++;
                        }
                    }

                    base.TextMessage += NewLine() + "Главная /start";
                }

                else
                    base.CallBackTitleText = "У вас еще нет заказов";


                return this;
            }
        }
    }
}
