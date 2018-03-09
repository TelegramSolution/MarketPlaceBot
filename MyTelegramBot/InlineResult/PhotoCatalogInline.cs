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
    public class PhotoCatalogInline:BotInline
    {
        private InlineKeyboardCallbackButton OpenBtn { get; set; }

        private BotInfo BotInfo { get; set; }

        public PhotoCatalogInline(string Query, BotInfo botInfo):base(Query)
        {
            this.Query = Query;

            this.BotInfo = botInfo;

            SqlQuery = "SELECT Product.* FROM Product Inner Join Category On Category.Id=Product.CategoryId "
                + "WHERE Product.Name LIKE @name and Product.Enable=1 OR Category.Name LIKE @name and Product.Enable=1 OR "
                + "Product.Text LIKE @name and Product.Enable=1";
        }

        private List<Product> GetProductList()
        {
            try
            {
                List<Product> product = new List<MyTelegramBot.Product>();
                SqlParameter param = new SqlParameter("@name", "%" + Query + "%");
                return db.Product.FromSql(SqlQuery, param).Include(p => p.CurrentPrice).
                        Include(p => p.CurrentPrice).Include(p => p.Stock).Include(p => p.Unit).ToList();
            }

            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Фотокаталог
        /// </summary>
        /// <returns></returns>
        public override InlineQueryResult[] GetResult()
        {
            db = new MarketBotDbContext();

            var ProductList = GetProductList();

            InlineQueryResult[] result;

            List<InlineQueryResultCachedPhoto> cachedPhotoList = new List<InlineQueryResultCachedPhoto>();

            for (int i = 0; i < ProductList.Count; i++)
            {
                var attach_telegram = db.AttachmentTelegram.Where(a => a.AttachmentFsId == ProductList[i].MainPhoto && a.BotInfoId == BotInfo.Id)
                    .FirstOrDefault();
                if (attach_telegram != null)
                {
                    Messages.ProductViewMessage productView = new Messages.ProductViewMessage(ProductList[i]);
                    InlineQueryResultCachedPhoto cachedPhoto = new InlineQueryResultCachedPhoto();
                    
                    string caption= ProductList[i].ToString();

                    if (caption.Length > 200)
                        caption=caption.Substring(0, 199);

                    cachedPhoto.Caption = caption;
                    cachedPhoto.Id = ProductList[i].Id.ToString();
                    cachedPhoto.FileId = attach_telegram.FileId;
                    cachedPhoto.ReplyMarkup = productView.SetInlineKeyBoard();
                    cachedPhotoList.Add(cachedPhoto);
                }
            }
            result = new InlineQueryResult[cachedPhotoList.Count];
            result = cachedPhotoList.ToArray();
            db.Dispose();
            return result;
        }



    }
}
