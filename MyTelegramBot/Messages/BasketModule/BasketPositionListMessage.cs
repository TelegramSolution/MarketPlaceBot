using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Все позиции товаров из корзины в виде кнопок
    /// </summary>
    public class BasketPositionListMessage:Bot.BotMessage
    {
        private int FollowerId { get; set; }

        private InlineKeyboardCallbackButton [][] ProductBtn { get; set; }

        List<IGrouping<Product, Basket>> Basket { get; set; }
        public BasketPositionListMessage(int FollowerId)
        {
            this.FollowerId = FollowerId;
            BackBtn = new InlineKeyboardCallbackButton("Назад", BuildCallData(Bot.BasketBot.BackToBasketCmd,Bot.BasketBot.ModuleName));
        }

        public override BotMessage BuildMsg()
        {            
            using (MarketBotDbContext db=new MarketBotDbContext())
                Basket=db.Basket.Where(b => b.FollowerId == FollowerId).Include(b => b.Product).GroupBy(b => b.Product).ToList();            

            if (Basket!=null)
            {
                ProductBtn = new InlineKeyboardCallbackButton[Basket.Count()+1][];

                int counter = 0;

                foreach(var position in Basket)
                {
                    ProductBtn[counter] = new InlineKeyboardCallbackButton[1];
                    ProductBtn[counter][0] = new InlineKeyboardCallbackButton((counter + 1).ToString() + ") " + position.ElementAt(0).Product.Name, 
                                                                                BuildCallData(Bot.BasketBot.EditBasketProductCmd,Bot.BasketBot.ModuleName ,position.ElementAt(0).ProductId));
                    counter++;
                }

                ProductBtn[ProductBtn.Length - 1] = new InlineKeyboardCallbackButton[1];
                ProductBtn[ProductBtn.Length - 1][0] = BackBtn;
                base.MessageReplyMarkup = new InlineKeyboardMarkup(ProductBtn);
                base.TextMessage = "Выберите позицию";

            }

            return this;
        }
    }
}
