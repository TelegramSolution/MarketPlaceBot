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
    public class OrderSearchInline:BotInline
    {

        List<Orders> Orders { get; set; }

        private InlineKeyboardCallbackButton OpenOrderBtn { get; set; }

        public OrderSearchInline(string Query):base(Query)
        {
            base.SqlQuery = "Select Orders.*  from Orders inner join Follower ON Follower.Id=Orders.FollowerId " +
                "inner join (Select Status.Name, OrderStatus.Id From OrderStatus inner join Status ON OrderStatus.StatusId=Status.Id) AS OrSt " +
                " ON OrSt.Id=Orders.CurrentStatus WHERE Follower.FIrstName Like @param OR Follower.LastName Like @param" +
                " OR Follower.Telephone Like @param  OR OrSt.Name Like @param";

          
        }

        private List<Orders> GetOrder()
        {
            try
            {

                db = new MarketBotDbContext();
                base.param = new SqlParameter("@param", "%" + Query.Trim() + "%");
                
                var orders = db.Orders.FromSql(base.SqlQuery, param)
                    .Include(o=>o.CurrentStatusNavigation).Include(o => o.Follower).OrderByDescending(o=>o.Id).Take(MaxResult).ToList();
                return orders;
            }

            catch (Exception e)
            {
                return null;
            }

            finally
            {
                
            }
        }

        public override InlineQueryResult[] GetResult()
        {
            this.Orders = GetOrder();
            InputTextMessageContent[] textcontent = new InputTextMessageContent[Orders.Count];
            InlineQueryResultArticle[] article = new InlineQueryResultArticle[Orders.Count];
            InlineQueryResult[] result = new InlineQueryResult[Orders.Count];


            for (int i = 0; i < Orders.Count; i++)
            {
                string StatusOrderLine = "";

                if (Orders[i].CurrentStatusNavigation !=null)
                    StatusOrderLine = "\r\nСтатус: "+ db.Status.Find(Orders[i].CurrentStatusNavigation.StatusId).Name;

                textcontent[i] = new InputTextMessageContent();
                textcontent[i].DisableWebPagePreview = true;
                textcontent[i].MessageText = "/order" + Orders[i].Number.ToString();

                article[i] = new InlineQueryResultArticle();
                article[i].HideUrl = true;
                article[i].Id = Orders[i].Number.ToString();
                article[i].Title = Orders[i].Number.ToString();
                article[i].Description = "№" + Orders[i].Number.ToString() + "\r\nДата:" + Orders[i].DateAdd.ToString()
                    + StatusOrderLine;

                article[i].ThumbUrl = "https://cdn2.iconfinder.com/data/icons/shop-payment-vol-6/128/shop-19-256.png";
                article[i].InputMessageContent = textcontent[i];


                result[i] = new InlineQueryResult();
                result[i] = article[i];
                
            }
            db.Dispose();
            return result;
        }
    }
}
