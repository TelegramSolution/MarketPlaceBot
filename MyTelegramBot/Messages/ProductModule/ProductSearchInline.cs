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

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Результат поиска через встраиваемый режим
    /// </summary>
    public class ProductSearchInline
    {

        private string Query { get; set; }

        private InlineKeyboardCallbackButton OpenBtn { get; set; }

        MarketBotDbContext db { get; set; }

        public ProductSearchInline(string Query)
        {
            this.Query = Query;
            db = new MarketBotDbContext();
        }

        public InlineQueryResult[] ProductInlineSearch()
        {
            List<Product> product = new List<MyTelegramBot.Product>();
            SqlParameter param = new SqlParameter("@name", "%" + Query + "%");
            product = db.Product.FromSql("SELECT Product.* FROM Product Inner Join Category On Category.Id=Product.CategoryId WHERE Product.Name LIKE @name and Product.Enable=1 OR Category.Name LIKE @name and Product.Enable=1 OR Product.Text LIKE @name and Product.Enable=1", param).
                    Include(p=>p.ProductPrice).Include(p=>p.Stock).Include(p=>p.ProductPrice).Include(p=>p.Unit).ToList();


            InputTextMessageContent[] textcontent = new InputTextMessageContent[product.Count];
            InlineQueryResultArticle[] article = new InlineQueryResultArticle[product.Count];
            InlineQueryResult[] result = new InlineQueryResult[product.Count];
            

            for (int i = 0; i < product.Count; i++)
            {
                textcontent[i] = new InputTextMessageContent();
                textcontent[i].DisableWebPagePreview = true;
                textcontent[i].MessageText = product[i].ToString();

                product[i].ProductPrice.Where(p => p.Enabled).FirstOrDefault().Currency 
                    = db.Currency.Where(c => c.Id ==product[i].ProductPrice.Where(p => p.Enabled).FirstOrDefault().CurrencyId).FirstOrDefault();

                article[i] = new InlineQueryResultArticle();
                article[i].HideUrl = true;
                article[i].Id = product[i].Id.ToString();
                article[i].Title = product[i].Name;
                article[i].Description = product[i].Name+" " 
                   +product[i].ProductPrice.Where(p=>p.Enabled).FirstOrDefault().Value.ToString() + " " 
                   + product[i].ProductPrice.Where(p => p.Enabled).FirstOrDefault().Currency.ShortName
                   + "\r\nНажмите сюда";

                article[i].ThumbUrl = product[i].PhotoUrl;
                article[i].Url = product[i].TelegraphUrl;
                article[i].InputMessageContent = textcontent[i];
                article[i].ReplyMarkup  = new InlineKeyboardMarkup(
                    new[]{
                    new[]
                    {
                        OpenBtn=new InlineKeyboardCallbackButton("Открыть",BuildCallData("GetProduct",product[i].Id))
                    }
                    });
                result[i] = new InlineQueryResult();
                result[i] = article[i];
            }
            return result;
        }

        private string BuildCallData(string CommandName, params int[] Argument)
        {
            BotCommand command = new BotCommand
            {
                Cmd = CommandName,
                Arg = new List<int>()
            };

            for (int i = 0; i < Argument.Length; i++)
                command.Arg.Add(Argument[i]);

            return JsonConvert.SerializeObject(command);
        }

    }
}
