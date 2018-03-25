using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Весь ассортимент категории одним сообщением
    /// </summary>
    public class ViewAllProductInCategoryMessage : BotMessage
    {
        private List<Product> ProductList { get; set; }

        private Category Category { get; set; }

        private InlineKeyboardCallbackButton NextCategoryBtn { get; set; }

        private InlineKeyboardCallbackButton PreviousCategoryBtn { get; set; }

        private int CategoryId { get; set; }

        MarketBotDbContext db;


        /// <summary>
        /// Сформированные страницы с товарами
        /// </summary>
        private Dictionary<int, List<Product>> Pages { get; set; }



        /// <summary>
        /// Общее кол-во стр. товаро этой категории 
        /// </summary>
        private int PageCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CategoryId">товары какой категории отобразить в этом сообщениее. Если ни чего не предалть, то будет отображать товары самой первой категории</param>
        public ViewAllProductInCategoryMessage(int CategoryId=0,int PageNumber=1)
        {
            BackBtn = new InlineKeyboardCallbackButton("Назад", BuildCallData("BackCategoryList",Bot.CategoryBot.ModuleName));
            this.CategoryId = CategoryId;
            this.SelectPageNumber = PageNumber;
            this.PageSize = 5;
        }

        public override BotMessage BuildMsg()
        {
            db = new MarketBotDbContext();

                if (CategoryId > 0)
                    Category = db.Category.Find(CategoryId);

                if (CategoryId == 0)
                {
                    Category = db.Category.Where(c => c.Enable).FirstOrDefault();
                    CategoryId = Category.Id;
                }

                if(Category.Product.Count==0)
                    Category.Product= db.Product.Where(p => p.CategoryId== Category.Id && p.Enable == true)
                        .Include(p => p.ProductPrice).Include(p => p.Category).Include(p => p.Stock).ToList();


                //определям следующую категорию
                var NextCategory = GetNextCategoryId(CategoryId);
                if (NextCategory.Id != CategoryId)
                    NextCategoryBtn = BuildInlineBtn(NextCategory.Name, BuildCallData("GetCategory", Bot.CategoryBot.ModuleName, NextCategory.Id),base.Next2Emodji);

                //определяем предыдующую категорию
                var PreviousCategory= GetPreviousCategoryId(CategoryId);
                if(PreviousCategory.Id!=CategoryId)
                    PreviousCategoryBtn = BuildInlineBtn(PreviousCategory.Name, BuildCallData("GetCategory", Bot.CategoryBot.ModuleName, PreviousCategory.Id),base.Previuos2Emodji,false);

                ProductList = db.Product.Where(p => p.Enable && p.CategoryId == CategoryId)
                    .Include(p => p.ProductPrice).Include(p => p.Category).Include(p => p.Stock).ToList();

                Pages = base.BuildDataPage<Product>(ProductList, this.PageSize);

                string message = base.GoldRhobmus + Bold(Category.Name) + base.GoldRhobmus +
                       NewLine() + "Всего товаров в категории: " + ProductList.Count.ToString() +
                       NewLine() + "Страница " + SelectPageNumber.ToString() + " из " + Pages.Count.ToString() + NewLine();
     

            if (Pages.Count > 0 && Pages.Count >= SelectPageNumber && Pages[SelectPageNumber]!=null)
            {
                var Page = Pages[SelectPageNumber];

                foreach (Product product in Page)
                {
                    message += NewLine() + base.BlueRhombus + product.Name + base.BlueRhombus + NewLine() +
                    "Цена:" + product.CurrentPrice.ToString();

                    if (product.Stock != null && product.Stock.Count == 0 ||
                    product.Stock != null && product.Stock.Count > 0 && product.Stock.OrderByDescending(s => s.Id).FirstOrDefault().Balance == 0)
                        message += NewLine() + "нет в наличии";

                    message += NewLine() + "Показать: /item" + product.Id.ToString() + NewLine();

                }

                if(PreviousCategoryBtn!=null && NextCategoryBtn!=null)
                    base.MessageReplyMarkup= base.PageNavigatorKeyboard<Product>(Pages, Bot.ProductBot.CmdProductPage, Bot.ProductBot.ModuleName, BackBtn, 
                        new InlineKeyboardButton[] { PreviousCategoryBtn, NextCategoryBtn }, CategoryId);

                else
                    base.MessageReplyMarkup = base.PageNavigatorKeyboard<Product>(Pages, Bot.ProductBot.CmdProductPage, Bot.ProductBot.ModuleName, BackBtn,
                        null, CategoryId);


                db.Dispose();
            }


            base.TextMessage = message;

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

            var Category = db.Category.Where(c => c.Id < CategoryId && c.Enable).OrderByDescending(c=>c.Id).FirstOrDefault();

            if (Category == null)
                Category = db.Category.Where(c => c.Enable).OrderByDescending(c => c.Id).FirstOrDefault();

            else
                db.Category.Find(CategoryId);

            return Category;
            
        }


    }
}
