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
    public class OperatorSearchInline:BotInline
    {
        private List<Follower> FollowerList { get; set; }


        public OperatorSearchInline(string Query):base(Query)
        {
            base.SqlQuery = "select  Follower.* from Follower inner Join Admin ON Follower.Id=Admin.FollowerId WHERE Telephone Like @param OR FIrstName Like @param  OR LastName Like @param  OR UserName Like @param";

        }

        private List<Follower> GetOperators()
        {
            try
            {

                db = new MarketBotDbContext();
                base.param = new SqlParameter("@param", "%" + Query.Trim() + "%");

                var follower = db.Follower.FromSql(base.SqlQuery, param).Include(f=>f.Admin).Take(MaxResult).ToList();

                return follower;
            }

            catch (Exception e)
            {
                return null;
            }

            finally
            {
                db.Dispose();
            }
        }
        public override InlineQueryResult[] GetResult()
        {
            db = new MarketBotDbContext();

            this.FollowerList = GetOperators();

            InputTextMessageContent[] textcontent = new InputTextMessageContent[FollowerList.Count];
            InlineQueryResultArticle[] article = new InlineQueryResultArticle[FollowerList.Count];
            InlineQueryResult[] result = new InlineQueryResult[FollowerList.Count];

            int i = 0;

            foreach (var follower in FollowerList)
            {
                string telephoneLine = "";


                if (follower.Telephone != null && follower.Telephone != "")
                    telephoneLine = BotMessage.NewLine() + "Телефон: " + follower.Telephone;


                textcontent[i] = new InputTextMessageContent();
                textcontent[i].ParseMode = Telegram.Bot.Types.Enums.ParseMode.Html;
                textcontent[i].DisableWebPagePreview = true;
                textcontent[i].MessageText = follower.FirstName + " " + follower.LastName + BotMessage.NewLine() +
                    "Профиль:" + "https://t.me/" + follower.UserName + telephoneLine;

                article[i] = new InlineQueryResultArticle();
                article[i].HideUrl = false;
                article[i].Id = follower.Id.ToString();
                article[i].Title = follower.FirstName + " " + follower.LastName;
                article[i].Description = telephoneLine;

                article[i].ThumbUrl = "https://cdn3.iconfinder.com/data/icons/user-avatars-1/512/users-10-3-256.png";
                article[i].Url = "https://t.me/" + follower.UserName;
                article[i].InputMessageContent = textcontent[i];

                result[i] = new InlineQueryResult();
                result[i] = article[i];

                i++;
            }

            return result;
        }
    }
}
