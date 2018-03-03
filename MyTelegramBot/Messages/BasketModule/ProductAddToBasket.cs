using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;

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

        public Basket Basket { get; set; }

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
                var list = db.Basket.Where(b => b.ProductId == ProductId && b.BotInfoId == BotId && b.FollowerId == FollowerId).ToList();

                var CurrentStock = db.Stock.Where(s => s.ProductId == ProductId).LastOrDefault();

                if (CurrentStock != null && CurrentStock.Balance >= list.Count + 1)
                {

                    Basket = new Basket
                    {
                        ProductId = ProductId,
                        DateAdd = DateTime.Now,
                        Enable = true,
                        FollowerId = FollowerId,
                        Amount = Amount,
                        BotInfoId = BotId

                    };

                    db.Basket.Add(this.Basket);

                    db.SaveChanges();

                    int? Quantity = db.Basket.Where(b => b.ProductId == ProductId && b.FollowerId == FollowerId && b.Enable && b.BotInfoId == BotId).Sum(b => b.Amount);

                    string ProductName = db.Product.Where(p => p.Id == ProductId).FirstOrDefault().Name;

                    base.CallBackTitleText = ProductName + " Всего: " + Quantity.ToString();
                }

                if (CurrentStock != null && CurrentStock.Balance < list.Count + 1)
                {
                    base.CallBackTitleText = "В наличии только " + CurrentStock.Balance.ToString();
                }

                return this;
            }
        }


        
    }
}
