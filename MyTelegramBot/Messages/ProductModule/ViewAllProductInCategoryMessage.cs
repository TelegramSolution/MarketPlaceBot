using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;


namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Весь ассортимент категории одним сообщением
    /// </summary>
    public class ViewAllProductInCategoryMessage : Bot.BotMessage
    {
        private List<Product> ProductList { get; set; }

        private Category Category { get; set; }

        private InlineKeyboardCallbackButton NextCategoryBtn { get; set; }

        private InlineKeyboardCallbackButton PreviousCategoryBtn { get; set; }

        private int CategoryId { get; set; }

        MarketBotDbContext db;

        public const string NextPageCmd = "NxtProdPage";

        public const string PreviuousPageCmd = "PrvProdPage";


        /// <summary>
        /// Сформированные страницы с товарами
        /// </summary>
        private Dictionary<int, List<Product>> Pages { get; set; }

        /// <summary>
        /// След. стр с товарами этой категории
        /// </summary>
        private InlineKeyboardCallbackButton NextPageBtn { get; set; }

        /// <summary>
        /// Предыдущая стр. с товарами этой категории
        /// </summary>
        private InlineKeyboardCallbackButton PreviusPageBtn { get; set; }

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
        }

        public override Bot.BotMessage BuildMsg()
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

            Pages = BuildPages(Category.Id);

            string message = base.GoldRhobmus + Bold(Category.Name) + base.GoldRhobmus +
                   NewLine() + "Всего товаров в категории: " + ProductList.Count.ToString() +
                   NewLine() + "Страница " + SelectPageNumber.ToString() + " из " + Pages.Count.ToString() + NewLine();

          

            if (Pages.Count > 0 && Pages.Count >= SelectPageNumber)
            {
                var Page = Pages[SelectPageNumber];

                foreach (Product product in Page)
                {
                    message += NewLine() + base.BlueRhombus + product.Name + base.BlueRhombus + NewLine() +
                    "Цена:" + product.ProductPrice.Where(p => p.Enabled).FirstOrDefault().Value
                    + db.Currency.Where(c => c.Id == product.ProductPrice.Where(p => p.Enabled).FirstOrDefault().CurrencyId).FirstOrDefault().ShortName;

                    if (product.Stock != null && product.Stock.Count == 0 ||
                    product.Stock != null && product.Stock.Count > 0 && product.Stock.OrderByDescending(s => s.Id).FirstOrDefault().Balance == 0)
                        message += NewLine() + "нет в наличии";

                    message += NewLine() + "Показать: /product" + product.Id.ToString() + NewLine();

                }

                message += NewLine() + "для поиска используйте Inline-режим. @" + Bot.GeneralFunction.GetBotName() + " запрос";

                NextPageBtn = SetNextPage();

                PreviusPageBtn = SetPreviousPage();

                db.Dispose();
            }

            SetKeyboard();

            base.TextMessage = message;

            return this;
            

        }

        private InlineKeyboardCallbackButton SetNextPage()
        {
            if (Pages.Keys.Last() != SelectPageNumber && Pages[SelectPageNumber + 1] != null) // Находим следующую страницу 
                return BuildInlineBtn("Следующая стр.", BuildCallData(NextPageCmd, Bot.ProductBot.ModuleName, SelectPageNumber + 1,CategoryId), base.Next2Emodji);

            if (Pages.Keys.Last() == SelectPageNumber && SelectPageNumber != 1 && Pages[1] != null)
                // Если выбранная пользователем страница является последней, то делаем кнопку с сылкой на первую,
                //но при это проверяем не является ли выбранная пользователем  страница первой
                return BuildInlineBtn("Следующая стр.", BuildCallData(NextPageCmd, Bot.ProductBot.ModuleName, 1,CategoryId), base.Next2Emodji);

            else
                return null;
                
        }

        /// <summary>
        /// создать кнопку назад, для навигации по страницам
        /// </summary>
        /// <returns></returns>
        private InlineKeyboardCallbackButton SetPreviousPage()
        {
            //находим предыдующую стр.
            if (SelectPageNumber > 1 && Pages[SelectPageNumber - 1] != null)
                return BuildInlineBtn("Предыдущая стр.", BuildCallData(PreviuousPageCmd, Bot.ProductBot.ModuleName, SelectPageNumber - 1, CategoryId), base.Previuos2Emodji, false);

            if (SelectPageNumber == 1 && Pages.Keys.Last() != 1)
                return BuildInlineBtn("Предыдущая стр.", BuildCallData(PreviuousPageCmd, Bot.ProductBot.ModuleName, Pages.Keys.Last(), CategoryId), base.Previuos2Emodji, false);

            else
                return null;

        }

        private void SetKeyboard()
        {
            if (NextCategoryBtn == null && PreviousCategoryBtn == null && NextPageBtn != null && PreviusPageBtn != null)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
            new[]{
            new[]
                    {
                        PreviusPageBtn, NextPageBtn
                    },
            new[]
                    {
                        BackBtn
                    },

             });

            if (NextCategoryBtn==null && PreviousCategoryBtn==null)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
            new[]{
            new[]
                    {
                        BackBtn
                    },

             });

            if (NextCategoryBtn != null && PreviousCategoryBtn == null && NextPageBtn!=null && PreviusPageBtn!=null)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
            new[]{
            new[]
                    {
                        PreviusPageBtn, NextPageBtn
                    },
            new[]
                    {
                        NextCategoryBtn
                    },
            new[]
                    {
                        BackBtn
                    },

             });

            if (NextCategoryBtn != null && PreviousCategoryBtn == null)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
            new[]{
            new[]
                    {
                        NextCategoryBtn
                    },
            new[]
                    {
                        BackBtn
                    },

             });

            if (NextCategoryBtn == null && PreviousCategoryBtn != null && NextPageBtn!=null && PreviusPageBtn!=null)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
            new[]{
            new[]
                    {
                        PreviusPageBtn, NextPageBtn
                    },
            new[]
                    {
                        PreviousCategoryBtn
                    },
            new[]
                    {
                        BackBtn
                    },

             });

            if (NextCategoryBtn == null && PreviousCategoryBtn != null)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
            new[]{
            new[]
                    {
                        PreviousCategoryBtn
                    },
            new[]
                    {
                        BackBtn
                    },

             });


            if (NextCategoryBtn != null && PreviousCategoryBtn != null && NextPageBtn!=null && PreviusPageBtn!=null)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
            new[]{

            new[]
                    {
                        PreviusPageBtn, NextPageBtn
                    },
            new[]
                    {
                        PreviousCategoryBtn, NextCategoryBtn
                    },
            new[]
                    {
                        BackBtn
                    },

             });

            if (NextCategoryBtn != null && PreviousCategoryBtn != null && NextPageBtn == null && PreviusPageBtn == null)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
            new[]{

            new[]
                    {
                        PreviousCategoryBtn, NextCategoryBtn
                    },
            new[]
                    {
                        BackBtn
                    },

             });
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

        private Dictionary<int, List<Product>> BuildPages(int CategoryId)
        {
            if(db==null)
            db = new MarketBotDbContext();

            ProductList = db.Product.Where(p=> p.Enable && p.CategoryId==CategoryId)
                .Include(p => p.ProductPrice).Include(p => p.Category).Include(p => p.Stock).ToList();


            if (ProductList.Count % PageSize > 0) // Определяем сколько всего будет страниц
                PageCount = (ProductList.Count / PageSize) + 1;

            else
                PageCount = ProductList.Count / PageSize;


            Pages = new Dictionary<int, List<Product>>();

            //начинаем заполнять

            for (int i = 0; i < PageCount; i++)
            {
                List<Product> list = new List<Product>();

                for (int j = 0; j < PageSize; j++)
                {
                    if ((i * PageSize + j) < ProductList.Count)
                        list.Add(ProductList.ElementAt(i * PageSize + j));

                    else
                        break;
                }
                Pages.Add(i + 1, list);

            }

            return Pages;
        }
    }
}
