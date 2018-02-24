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
    /// Предложение оставить отзыв к заказу
    /// </summary>
    public class FeedBackOfferMessage:Bot.BotMessage
    {
        private int OrderId { get; set; }

        private InlineKeyboardCallbackButton AddFeedBackBtn { get; set; }

        private Orders Order { get; set; }

        private InlineKeyboardCallbackButton [][] FeedBackProductsBtn { get; set; }

        private List<FeedBack> FeedBackList { get; set; }

        private List<IGrouping<int, OrderProduct>> ProductGroup { get; set; }


        /// <summary>
        /// товары у которых еще нет отзывов
        /// </summary>
        private List<Product> ProductNoFeedback { get; set; }

        MarketBotDbContext db;

        public FeedBackOfferMessage(int OrderId)
        {
            this.OrderId = OrderId;
            BackBtn = BuildInlineBtn("Назад", BuildCallData("BackToMyOrder", Bot.OrderBot.ModuleName, OrderId));
        }

        public FeedBackOfferMessage (Orders orders)
        {
            this.Order = orders;
            BackBtn = BuildInlineBtn("Назад", BuildCallData("BackToMyOrder", Bot.OrderBot.ModuleName, Order.Id));
        }

        public override BotMessage BuildMsg()
        {
            db = new MarketBotDbContext();

            if(this.Order==null)
                Order = db.Orders.Where(o => o.Id == OrderId).FirstOrDefault();

            if(this.Order!=null)
                Order.OrderProduct = db.OrderProduct.Where(o => o.OrderId == Order.Id).Include(o => o.Product).ToList();


            FeedBackList = db.FeedBack.Where(f => f.OrderId == Order.Id && f.Enable).Include(f=>f.Product).ToList();

            ProductGroup = Order.OrderProduct.GroupBy(o => o.ProductId).ToList();

            ProductNoFeedback = new List<Product>();

            //проверяем для какие товаров уже есть отзыв. Если отзыв есть то кнопку не рисуем
            foreach (var group in ProductGroup)
                if (FeedBackList.Where(f => f.ProductId == group.FirstOrDefault().ProductId).FirstOrDefault() == null 
                        || FeedBackList.Where(f => f.ProductId == group.FirstOrDefault().ProductId).FirstOrDefault()!=null &&
                        FeedBackList.Where(f => f.ProductId == group.FirstOrDefault().ProductId).FirstOrDefault().Enable==false)
                    ProductNoFeedback.Add(group.FirstOrDefault().Product);


            FeedBackProductsBtn = new InlineKeyboardCallbackButton[ProductNoFeedback.Count+1][];
            int counter = 0;
            foreach(var product in ProductNoFeedback)
            {
                FeedBackProductsBtn[counter] = new InlineKeyboardCallbackButton[1];
                FeedBackProductsBtn[counter][0]= BuildInlineBtn(product.Name, BuildCallData(Bot.OrderBot.CmdAddFeedBackProduct, Bot.OrderBot.ModuleName, Order.Id, product.Id));
                counter++;
            }


            FeedBackProductsBtn[FeedBackProductsBtn.Length - 1] = new InlineKeyboardCallbackButton[1];
            FeedBackProductsBtn[FeedBackProductsBtn.Length - 1][0] = BackBtn;

            string SavedFeedBack = "";

            counter = 1;

            foreach(FeedBack feed in FeedBackList)
            {
                SavedFeedBack+=counter.ToString() + ") " + feed.Product.Name + NewLine()+
                    Bold("Оценка:") + feed.RaitingValue.ToString() + NewLine() +
                    Bold("Комментарий:") + feed.Text + NewLine();
            }

            base.MessageReplyMarkup = new InlineKeyboardMarkup(FeedBackProductsBtn);
            base.TextMessage =base.GoldRhobmus+ "Заказ №" + this.Order.Number.ToString() + base.GoldRhobmus + NewLine() + "Добавьте отзывы к товарам" + NewLine() + SavedFeedBack;

            return this;

        }
    }
}
