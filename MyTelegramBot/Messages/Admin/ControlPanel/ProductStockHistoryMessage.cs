using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// Сообщение с историей изменения остатков определенного товара
    /// </summary>
    public class ProductStockHistoryMessage:BotMessage
    {

        private List<Stock> StockList { get; set; }

        private MarketBotDbContext db { get; set; }

        private int ProductId { get; set; }

        private Dictionary<int,List<Stock>> Pages { get; set; }

        public ProductStockHistoryMessage (int ProductId,int SelectPageNumber = 1)
        {
            base.SelectPageNumber = SelectPageNumber;
            this.ProductId = ProductId;
            base.PageSize=6;
        }

        public override BotMessage BuildMsg()
        {
            db = new MarketBotDbContext();

            StockList = db.Stock.Where(s => s.ProductId == ProductId).Include(s => s.Product).OrderByDescending(s=>s.Id).ToList();

            Pages = base.BuildDataPage<Stock>(StockList, base.PageSize);

            base.MessageReplyMarkup = base.PageNavigatorKeyboard<Stock>(Pages, Bot.AdminModule.AdminBot.ViewStockHistoryProdCmd, Bot.AdminModule.AdminBot.ModuleName, base.BackToAdminPanelBtn(),Argument: ProductId);

            if(Pages!=null && Pages.Count>0 && Pages[SelectPageNumber] != null)
            {
                var page = Pages[SelectPageNumber];

                int counter = 0;

                base.TextMessage =base.BlueRhombus+" "+ StockList.FirstOrDefault().Product.Name+NewLine() + "Всего записей:"+ StockList.Count.ToString()+
                    NewLine()+ "Страница " +SelectPageNumber.ToString()+ " из " + Pages.Count.ToString()+NewLine()+NewLine();

                foreach(var stock in page)
                {
                    base.TextMessage+="Дата: " + stock.DateAdd.ToString() + " | Было " + (stock.Balance - stock.Quantity).ToString()
                        + " | Изменение: " + stock.Quantity.ToString() + " | Стало: " + stock.Balance.ToString() + " | комментарий: " +stock.Text + NewLine();

                    counter++;
                }

                
            }

            return this;
        }
    }
}
