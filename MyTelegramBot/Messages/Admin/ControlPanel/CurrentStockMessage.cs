using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// сообещние с текущими остатками по товарам
    /// </summary>
    public class CurrentStockMessage:BotMessage
    {
        private List<Product> ProductList { get; set; }

        private Dictionary<int, List<Product>> Pages { get; set; }

        MarketBotDbContext db;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CategoryId">товары какой категории отобразить в этом сообщениее. Если ни чего не предалть, то будет отображать товары самой первой категории</param>
        public CurrentStockMessage(int SelectPage=1)
        {
            base.PageSize = 5;
            base.SelectPageNumber = SelectPage;
        }

        public override BotMessage BuildMsg()
        {
            db = new MarketBotDbContext();

            ProductList = db.Product.Include(p => p.ProductPrice)
                .Include(p => p.Category).Include(p => p.Stock).ToList();

                Pages = BuildDataPage<Product>(ProductList);
      
                base.MessageReplyMarkup = base.PageNavigatorKeyboard<Product>(Pages, Bot.AdminModule.AdminBot.ViewStockCmd, Bot.AdminModule.AdminBot.ModuleName, BackToAdminPanelBtn());

            if (Pages != null && Pages.Count > 0 && Pages.Count >= SelectPageNumber 
                && Pages[SelectPageNumber] != null)
            {
                string message = "";
                base.TextMessage = message;
                int counter = 1; //счетчик
                var page = Pages[SelectPageNumber];
                int number = 1; // порядковый номер записи

                base.TextMessage = "Остатки " + NewLine() +
                "Страница " + SelectPageNumber.ToString() + " из " + Pages.Count.ToString() + NewLine();


                foreach (Product product in page)
                {
                    number = PageSize * (SelectPageNumber - 1) + counter;

                    message += NewLine() + number.ToString() + ") " + product.Name + NewLine();

                    if (product.Stock.Count > 0)
                        message +="Остаток:" + product.Stock.LastOrDefault().Balance.ToString();

                    if (product.Stock != null && product.Stock.Count == 0 ||
                    product.Stock != null && product.Stock.Count > 0 && product.Stock.OrderByDescending(s => s.Id).FirstOrDefault().Balance == 0)
                        message +="нет в наличии";

                    message += NewLine() + "Изменить: /adminproduct" + product.Id.ToString() + NewLine() +
                        "История: /stockhistory" + product.Id.ToString() + NewLine();

                    counter++;
                    
                }
                base.TextMessage += message;
            }

            return this;


        }


        private Category GetNextCategoryId(int CategoryId)
        {

            var Category = db.Category.Where(c => c.Id > CategoryId && c.Enable).FirstOrDefault();

            if (Category == null)
                Category = db.Category.Where(c => c.Enable).OrderBy(c => c.Id).FirstOrDefault();

            else
                db.Category.Find(CategoryId);

            return Category;

        }

        private Category GetPreviousCategoryId(int CategoryId)
        {

            var Category = db.Category.Where(c => c.Id < CategoryId && c.Enable).OrderByDescending(c => c.Id).FirstOrDefault();

            if (Category == null)
                Category = db.Category.Where(c => c.Enable).OrderByDescending(c => c.Id).FirstOrDefault();

            else
                db.Category.Find(CategoryId);

            return Category;

        }
    }
}

