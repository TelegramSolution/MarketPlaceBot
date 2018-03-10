using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
using System.Data.SqlClient;
using Newtonsoft.Json;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.InlineResult
{
    /// <summary>
    /// Результат поиска через встраиваемый режим
    /// </summary>
    public class ProductSearchInline:BotInline
    {      

        public ProductSearchInline(string Query):base(Query)
        {
            this.Query = Query;


            SqlQuery = "SELECT TOP 20 Product.* FROM Product Inner Join Category On Category.Id=Product.CategoryId "
                + "WHERE Product.Name LIKE @name and Product.Enable=1 OR Category.Name LIKE @name and Product.Enable=1 OR "
                + "Product.Text LIKE @name and Product.Enable=1";
        }

        private List<Product> GetProductList()
        {
            try
            {
                List<Product> product = new List<MyTelegramBot.Product>();
                SqlParameter param = new SqlParameter("@name", "%" + Query + "%");
                return db.Product.FromSql(SqlQuery, param).Include(p=>p.CurrentPrice).Include(p=>p.Category).
                        Include(p => p.CurrentPrice).Include(p => p.Stock).Include(p => p.Unit).ToList();
            }

            catch
            {
                return null;
            }
        }


        public override InlineQueryResult[] GetResult()
        {
            db = new MarketBotDbContext();

            var ProductList = GetProductList();

            InputTextMessageContent[] textcontent = new InputTextMessageContent[ProductList.Count];
            InlineQueryResultArticle[] article = new InlineQueryResultArticle[ProductList.Count];
            InlineQueryResult[] result = new InlineQueryResult[ProductList.Count];

            int i = 0;

            foreach (Product prod in ProductList)
            {
                textcontent[i] = new InputTextMessageContent();
                textcontent[i].ParseMode = Telegram.Bot.Types.Enums.ParseMode.Html;
                textcontent[i].DisableWebPagePreview = true;
                textcontent[i].MessageText = "/product"+prod.Id.ToString();

                article[i] = new InlineQueryResultArticle();
                article[i].HideUrl = true;
                article[i].Id = prod.Id.ToString();
                article[i].Title = prod.Name;
                article[i].Description = "Категория:" + prod.Category.Name + BotMessage.NewLine() + "Цена:" + prod.CurrentPrice.Value;

                article[i].ThumbUrl = "https://cdn2.iconfinder.com/data/icons/shop-payment-vol-6/128/shop-05-256.png";
                article[i].InputMessageContent = textcontent[i];
                result[i] = article[i];
                i++;
            }


            db.Dispose();
            return result;
        }


    }
}
