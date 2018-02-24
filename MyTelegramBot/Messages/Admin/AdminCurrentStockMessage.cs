using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.Messages.Admin
{
    public class AdminCurrentStockMessage:Bot.BotMessage
    {
        private List<Product> ProductList { get; set; }

        private Category Category { get; set; }

        private InlineKeyboardCallbackButton NextCategoryBtn { get; set; }

        private InlineKeyboardCallbackButton PreviousCategoryBtn { get; set; }

        private int CategoryId { get; set; }

        MarketBotDbContext db;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CategoryId">товары какой категории отобразить в этом сообщениее. Если ни чего не предалть, то будет отображать товары самой первой категории</param>
        public AdminCurrentStockMessage(int CategoryId = 0)
        {
            BackBtn = new InlineKeyboardCallbackButton("Назад", BuildCallData(Bot.AdminModule.AdminBot.BackToAdminPanelCmd , Bot.AdminModule.AdminBot.ModuleName));
            this.CategoryId = CategoryId;
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
            if (Category.Product.Count == 0)
                Category.Product = db.Product.Where(p => p.CategoryId == Category.Id && p.Enable == true)
                    .Include(p => p.ProductPrice).Include(p => p.Category).Include(p => p.Stock).ToList();


            //определям следующую категорию
            var NextCategory = GetNextCategoryId(CategoryId);
            if (NextCategory.Id != CategoryId)
                NextCategoryBtn = new InlineKeyboardCallbackButton(NextCategory.Name, BuildCallData("GetCategoryStock", Bot.AdminModule.AdminBot.ModuleName, NextCategory.Id));

            //определяем предыдующую категорию
            var PreviousCategory = GetPreviousCategoryId(CategoryId);
            if (PreviousCategory.Id != CategoryId)
                PreviousCategoryBtn = new InlineKeyboardCallbackButton(PreviousCategory.Name, BuildCallData("GetCategoryStock", Bot.AdminModule.AdminBot.ModuleName, PreviousCategory.Id));



            string message = Bold(Category.Name);
            int counter = 1;
            foreach (Product product in Category.Product)
            {

                message += NewLine() + counter.ToString() + ") " + product.Name + NewLine() +
                  "Остаток:"+  product.Stock.LastOrDefault().Balance.ToString();

                if (product.Stock != null && product.Stock.Count == 0 ||
                product.Stock != null && product.Stock.Count > 0 && product.Stock.OrderByDescending(s => s.Id).FirstOrDefault().Balance == 0)
                    message += NewLine() + "нет в наличии";

                message += NewLine() + "Изменить: /adminproduct" + product.Id.ToString() + NewLine();

                counter++;

            }


           
            base.TextMessage = message;

            SetKeyboard();

            return this;


        }

        private void SetKeyboard()
        {

            if (NextCategoryBtn == null && PreviousCategoryBtn == null)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
            new[]{
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

            if (NextCategoryBtn != null && PreviousCategoryBtn != null)
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

            var Category = db.Category.Where(c => c.Id < CategoryId && c.Enable).OrderByDescending(c => c.Id).FirstOrDefault();

            if (Category == null)
                Category = db.Category.Where(c => c.Enable).OrderByDescending(c => c.Id).FirstOrDefault();

            else
                db.Category.Find(CategoryId);

            return Category;

        }
    }
}

