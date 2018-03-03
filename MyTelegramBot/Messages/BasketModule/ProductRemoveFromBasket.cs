using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Сообщение с обновленными данными из корзины по конкретному товару. После удаления одной еденицы
    /// </summary>
    public class ProductRemoveFromBasket:BotMessage
    {
        private int ProductId { get; set; }

        private int FollowerId { get; set; }

        private int BotId { get; set; }

        Basket Basket { get; set; }

        public ProductRemoveFromBasket(int FollowerId, int ProductId, int BotId)
        {
            this.ProductId = ProductId;
            this.FollowerId = FollowerId;
            this.BotId = BotId;
        }
        public override BotMessage BuildMsg()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                Basket = db.Basket.Where(b => b.ProductId == ProductId && b.BotInfoId==BotId && b.FollowerId == FollowerId && b.Enable).FirstOrDefault();

                if (Basket != null)
                {
                    db.Basket.Remove(Basket);
                    db.SaveChanges();
                }

                int Quantity = db.Basket.Where(b => b.ProductId == ProductId && b.BotInfoId == BotId && b.FollowerId == FollowerId && b.Enable).Sum(b => b.Amount);

                string ProductName = db.Product.Where(p => p.Id == ProductId).FirstOrDefault().Name;

                base.CallBackTitleText = ProductName + " Всего: " + Quantity.ToString();

                return this;
            }
        }
    }
}
