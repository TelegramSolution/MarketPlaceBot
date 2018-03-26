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
    public class MyOrdersSearchInline : BotInline
    {
        List<Orders> OrderList { get; set; }

        private int ChatId { get; set; }

        private int FollowerId { get; set; }

        public MyOrdersSearchInline (string Query, int ChatId) : base(Query)
        {
            this.Query = Query;
            this.ChatId = ChatId;

        }

        public override InlineQueryResult[] GetResult()
        {
            db = new MarketBotDbContext();

            var Follower= db.Follower.Where(f => f.ChatId == ChatId).FirstOrDefault();

            if (Follower != null)
            {

                OrderList = db.Orders.Where(o => o.FollowerId == Follower.Id).OrderByDescending(o => o.Id).Take(MaxResult).ToList();

                int i = 0;

                InputTextMessageContent[] textcontent = new InputTextMessageContent[OrderList.Count];
                InlineQueryResultArticle[] article = new InlineQueryResultArticle[OrderList.Count];
                InlineQueryResult[] result = new InlineQueryResult[OrderList.Count];

                foreach (Orders order in OrderList)
                {
                    textcontent[i] = new InputTextMessageContent();
                    textcontent[i].ParseMode = Telegram.Bot.Types.Enums.ParseMode.Html;
                    textcontent[i].DisableWebPagePreview = true;
                    textcontent[i].MessageText = "/myorder" + order.Number.ToString();

                    article[i] = new InlineQueryResultArticle();
                    article[i].HideUrl = true;
                    article[i].Id = order.Id.ToString();
                    article[i].Title = order.Number.ToString();
                    article[i].Description = "№:" + order.Number.ToString() + BotMessage.NewLine() + "Дата:" + order.DateAdd.ToString();

                    article[i].ThumbUrl = "https://cdn2.iconfinder.com/data/icons/shop-payment-vol-6/128/shop-04-256.png";
                    article[i].InputMessageContent = textcontent[i];
                    result[i] = article[i];
                    i++;
                }
                db.Dispose();
                return result;
            }

            return null;
        }
    }
}
