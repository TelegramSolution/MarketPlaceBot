using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// CallBack сообщение с обновленным количеством товара в заказе
    /// </summary>
    public class OrderPositionProccess: Bot.BotMessage
    {
        private int PositionId { get; set; }

        private int Amount { get; set; }

        private int Quantity { get; set; }

        OrderProduct Position { get; set; }

        private OrderPositionEditMessage orderPositionEditMessage { get; set; }
        public OrderPositionProccess(int PositionId, int Amount)
        {
            this.PositionId = PositionId;
            this.Amount = Amount;
            orderPositionEditMessage = new OrderPositionEditMessage(PositionId);
        }

        public OrderPositionEditMessage BuildMessage()
        {
            if (Amount > 0)
                Quantity= Add();

            if (Amount < 0)
                Quantity= Remove();

            orderPositionEditMessage.BuildMsg();
            orderPositionEditMessage.CallBackTitleText= "Всего " + Quantity.ToString();
            return orderPositionEditMessage;
        }

        private int Add()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                Position = db.OrderProduct.Where(p => p.Id == PositionId).FirstOrDefault();

                if (Position != null)
                {
                    Position.Count += 1;
                    db.SaveChanges();
                    return Position.Count;
                }

                else
                    return 0;
            } 

        }

        private int Remove()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var position = db.OrderProduct.Where(p => p.Id == PositionId).FirstOrDefault();

                if (position.Count > 0)
                    position.Count -= 1;

                db.SaveChanges();

                return position.Count;
            }
        }
    }
}
