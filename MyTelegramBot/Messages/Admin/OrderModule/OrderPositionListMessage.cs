using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.ReplyMarkups;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// Список позиций заказа  в виде кнопок
    /// </summary>
    public class OrderPositionListMessage: BotMessage
    {
        InlineKeyboardCallbackButton [][] PostitionsBtn { get; set; }

        private int OrderId { get; set; }

        private List<OrderProduct> OrderProductList { get; set; }

        private decimal? OrderNumber { get; set; }

        public OrderPositionListMessage (int OrderId)
        {
            this.OrderId = OrderId;
        }

               
        public override BotMessage BuildMsg ()
        {

            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                OrderProductList = db.OrderProduct.Where(o => o.OrderId == OrderId).Include(o => o.Product).ToList();
                OrderNumber = db.Orders.Where(o => o.Id == OrderId).FirstOrDefault().Number;
            }

            if (OrderProductList != null)
            {
                int Amount = OrderProductList.Count();

                PostitionsBtn = new InlineKeyboardCallbackButton[Amount + 1][];
                base.BackBtn = new InlineKeyboardCallbackButton("Назад",BuildCallData("BackToOrder", OrderBot.ModuleName, OrderId));

                int Counter = 0;

                foreach (var product in OrderProductList)
                {
                    PostitionsBtn[Counter] = new InlineKeyboardCallbackButton[1];
                    PostitionsBtn[Counter][0] = ProductPosition(product.Id, (Counter + 1).ToString() + ") " + product.Product.Name + " - " + product.Count.ToString() + " шт.");
                    Counter++;
                }
                PostitionsBtn[Amount] = new InlineKeyboardCallbackButton[1];
                PostitionsBtn[Amount][0] = BackBtn;
                base.MessageReplyMarkup = new InlineKeyboardMarkup(PostitionsBtn);
                base.TextMessage = "Изменить содержание заказа "+ OrderNumber.ToString();
            }

            return this;

        }

        private InlineKeyboardCallbackButton ProductPosition (int PositionId, string BtnText)
        {
            string data = BuildCallData(Bot.OrderPositionBot.GetPositionCmd,Bot.OrderPositionBot.ModuleName ,PositionId);
            InlineKeyboardCallbackButton btn = new InlineKeyboardCallbackButton(BtnText, data);
            return btn;
        }
    }
}
