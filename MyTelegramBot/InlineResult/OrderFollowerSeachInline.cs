using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot.Core;
using Telegram.Bot.Types.InlineQueryResults;
using MyTelegramBot.BusinessLayer;
using Telegram.Bot.Types.InputMessageContents;


namespace MyTelegramBot.InlineResult
{
    public class OrderFollowerSeachInline: BotInline
    {

        int FollowerId { get; set; }

        public OrderFollowerSeachInline(string Query) : base(Query)
        {
            try
            {
                FollowerId =Convert.ToInt32(Query);
            }

            catch
            {

            }
        }

        public override InlineQueryResult[] GetResult()
        {
            var Orders = FollowerFunction.FollowerOrder(FollowerId).Take(MaxResult).ToList();

            InputTextMessageContent[] textcontent = new InputTextMessageContent[Orders.Count];
            InlineQueryResultArticle[] article = new InlineQueryResultArticle[Orders.Count];
            InlineQueryResult[] result = new InlineQueryResult[Orders.Count];

            int i = 0;

            foreach (var order in Orders)
            {
                textcontent[i] = new InputTextMessageContent();
                textcontent[i].ParseMode = Telegram.Bot.Types.Enums.ParseMode.Html;
                textcontent[i].DisableWebPagePreview = true;
                textcontent[i].MessageText = "/order" + order.Number.ToString();

                article[i] = new InlineQueryResultArticle();
                article[i].HideUrl = false;
                article[i].Id = order.Id.ToString();
                article[i].Title = "Заказ №" + order.Number.ToString();
                article[i].Description = "Время:" + order.DateAdd.ToString()+ BotMessage.NewLine();

                article[i].ThumbUrl = "https://cdn2.iconfinder.com/data/icons/shop-payment-vol-6/128/shop-19-256.png";
                article[i].InputMessageContent = textcontent[i];

                result[i] = new InlineQueryResult();
                result[i] = article[i];

                i++;
            }

            return result;


        }
    }
}
