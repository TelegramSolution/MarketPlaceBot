using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using MyTelegramBot.Bot;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Сообщение с кол-во товара в корзине.
    /// </summary>
    public class AddProductToBasketMessage:BotMessage
    {
        private int ProductId { get; set; }

        private int FollowerId { get; set; }

        private int Amount { get; set; }

        private int BotId { get; set; }

        public AddProductToBasketMessage(int FollowerId, int ProductId,int BotId, int Amount=1)
        {
            this.FollowerId = FollowerId;
            this.ProductId = ProductId;
            this.Amount = Amount;
            this.BotId = BotId;
        }

        public override BotMessage BuildMsg ()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                db.Basket.Add(new Basket
                {
                    ProductId = ProductId,
                    DateAdd = DateTime.Now,
                    Enable = true,
                    FollowerId = FollowerId,
                    Amount = Amount,
                    BotInfoId=BotId

                });

                int save = db.SaveChanges();

                int? Quantity =db.Basket.Where(b => b.ProductId == ProductId && b.FollowerId == FollowerId && b.Enable && b.BotInfoId== BotId).Sum(b => b.Amount);

                string ProductName = db.Product.Where(p => p.Id == ProductId).FirstOrDefault().Name;

                base.CallBackTitleText = ProductName + " Всего: " + Quantity.ToString();

                return this;
            }
        }


        
    }
}
