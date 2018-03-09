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
    public class HelpDeskSearchInline:BotInline
    {
        List<HelpDesk> list { get; set; }

        int Number { get; set; }

        public HelpDeskSearchInline(string Query) : base(Query)
        {
            try
            {
                Number = Convert.ToInt32(Query);
            }

            catch
            {

            }
        }

        public override InlineQueryResult[] GetResult()
        {
             list = HelpDeskFunction.GetHelpDeskList(Number);


            InputTextMessageContent[] textcontent = new InputTextMessageContent[list.Count];
            InlineQueryResultArticle[] article = new InlineQueryResultArticle[list.Count];
            InlineQueryResult[] result = new InlineQueryResult[list.Count];

            int i = 0;

            foreach (var help in list)
            {

                textcontent[i] = new InputTextMessageContent();
                textcontent[i].ParseMode = Telegram.Bot.Types.Enums.ParseMode.Html;
                textcontent[i].DisableWebPagePreview = true;
                textcontent[i].MessageText ="/ticket"+ help.Number.ToString();

                article[i] = new InlineQueryResultArticle();
                article[i].HideUrl = false;
                article[i].Id = help.Id.ToString();
                article[i].Title ="Заявка №"+ help.Number.ToString();
                article[i].Description = help.Number + BotMessage.NewLine() +
                    "Время:" + help.Timestamp;

                article[i].ThumbUrl = "https://cdn2.iconfinder.com/data/icons/shop-payment-vol-6/128/shop-10-256.png";
                article[i].InputMessageContent = textcontent[i];

                result[i] = new InlineQueryResult();
                result[i] = article[i];

                i++;
            }

            return result;
        }
    }
}
