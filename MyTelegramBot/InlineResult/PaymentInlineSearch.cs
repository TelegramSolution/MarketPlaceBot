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
using MyTelegramBot.BusinessLayer;

namespace MyTelegramBot.InlineResult
{
    public class PaymentInlineSearchInline:BotInline
    {
        //https://cdn2.iconfinder.com/data/icons/shop-payment-vol-6/128/shop-01-256.png

        Messages.Admin.PaymentViewMessage PaymentViewMsg { get; set; }

        public PaymentInlineSearchInline(string Query):base(Query)
        {

        }

        public override InlineQueryResult[] GetResult()
        {
           var list = PaymentsFunction.GetPaymentsList();


            InputTextMessageContent[] textcontent = new InputTextMessageContent[list.Count];
            InlineQueryResultArticle[] article = new InlineQueryResultArticle[list.Count];
            InlineQueryResult[] result = new InlineQueryResult[list.Count];

            int i = 0;

            foreach (var payment in list)
            {
                //PaymentViewMsg = new Messages.Admin.PaymentViewMessage(payment);
                //BotMessage mess = PaymentViewMsg.BuildMsg();

                textcontent[i] = new InputTextMessageContent();
                textcontent[i].ParseMode = Telegram.Bot.Types.Enums.ParseMode.Html;
                textcontent[i].DisableWebPagePreview = true;
                textcontent[i].MessageText = "/payment"+payment.Id.ToString();

                article[i] = new InlineQueryResultArticle();
                article[i].HideUrl = false;
                article[i].Id = payment.Id.ToString();
                article[i].Title = "Платеж №" + payment.Id.ToString();
                article[i].Description = payment.TimestampDataAdd.ToString();

                article[i].ThumbUrl = "https://cdn2.iconfinder.com/data/icons/shop-payment-vol-6/128/shop-01-256.png";
                //article[i].ReplyMarkup = mess.MessageReplyMarkup;
                article[i].InputMessageContent = textcontent[i];


                result[i] = new InlineQueryResult();
                result[i] = article[i];

                i++;
            }

            return result;
        }
    }
}
