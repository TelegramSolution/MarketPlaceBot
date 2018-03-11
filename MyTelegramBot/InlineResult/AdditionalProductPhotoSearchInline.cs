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
    public class AdditionalProductPhotoSearchInline:BotInline
    {
        List<AttachmentFs> PhotosList { get; set; }

        //public AdditionalProductPhotoSearchInline(string Query):base(Query)
        //{

        //}

        private BotInfo BotInfo { get; set; }

        public AdditionalProductPhotoSearchInline(string Query, BotInfo botInfo): base(Query)
        {
            this.BotInfo = botInfo;
            
        }
        public override InlineQueryResult[] GetResult()        {
            db = new MarketBotDbContext();

            var product = db.Product.Where(p => p.Name == Query.Trim()).Include(p=>p.ProductPhoto).FirstOrDefault();

            InputTextMessageContent[] textcontent = new InputTextMessageContent[product.ProductPhoto.Count];
            InlineQueryResultPhoto[] ResultPhoto = new InlineQueryResultPhoto[product.ProductPhoto.Count];
            InlineQueryResult[] result = new InlineQueryResult[product.ProductPhoto.Count];

            if (product != null)
            {
                int i = 0;
                foreach (var photo in product.ProductPhoto)
                {
                    var attach = db.AttachmentFs.Find(photo.AttachmentFsId);

                    
                    ResultPhoto[i] = new InlineQueryResultPhoto();
                    ResultPhoto[i].Id = (i + 1).ToString();
                    ResultPhoto[i].Caption = attach.Caption;
                    // ResultPhoto[i].Url = db.AttachmentTelegram.Where(a => a.AttachmentFsId == attach.Id && a.BotInfoId == this.BotInfo.Id).FirstOrDefault().FileId;
                    ResultPhoto[i].ThumbUrl = "https://groosha.gitbooks.io/telegram-bot-lessons/l9_2.png";
                    ResultPhoto[i].Url= "https://groosha.gitbooks.io/telegram-bot-lessons/l9_2.png";
                    result[i] = new InlineQueryResult();

                    result[i] = ResultPhoto[i];
                    i++;
                }
            }

            return result;
        }
    }
}
