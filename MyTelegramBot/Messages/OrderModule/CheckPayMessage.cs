using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// сообщение со статусом платежа найден / не найден 
    /// </summary>
    public class CheckPayMessage:BotMessage
    {
        public Orders Order { get; set; }

        private int OrderId { get; set; }

        MarketBotDbContext db;

        Services.ICryptoCurrency CryptoCurrency { get; set; }

        Services.BitCoinCore.BitCoin BitCoinCore { get; set; }

        public CheckPayMessage(int OrderID)
        {
            this.OrderId = OrderID;

            db = new MarketBotDbContext();
        }


        public CheckPayMessage(Orders Order)
        {
            this.Order = Order;

            
        }

        private Orders GetOrder(int OrderId)
        {
             return db.Orders.Where(o => o.Id == OrderId).Include(o=>o.Invoice).FirstOrDefault();
        }

        public async Task<CheckPayMessage> BuildMessage()
        {

            db = new MarketBotDbContext();

            if (this.Order==null)
                this.Order = GetOrder(this.OrderId);



            if (Order.Paid == true)
                base.TextMessage = "Ваш заказ оплачен";


            return this;
        }






    }
}
