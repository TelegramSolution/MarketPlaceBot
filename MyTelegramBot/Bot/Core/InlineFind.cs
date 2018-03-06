using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Telegram.Bot.Types.Payments;
using System.Text;
using System.Collections.Specialized;
using System.Data.SqlClient;
using MyTelegramBot.InlineResult;

namespace MyTelegramBot.Bot.Core
{
    public class InlineFind
    {
        public const string FindOrder = "Заказы";

        public const string AdditionalProduct = "Доп.фото";


        private InlineQuery inlineQuery { get; set; }

        /// <summary>
        /// имя таблицы в которой будет производить поиск данных
        /// </summary>
        private string From { get; set; }

        /// <summary>
        /// поле -> значение которое нужно найти в этом поле. Например Телефон:890000000181 или же Where Telephone=890000000181
        /// </summary>
        private Dictionary <string,string> FindArgument { get; set; }


        private InlineResult.OrderSearchInline OrderSearchInline { get; set; }

        private InlineResult.ProductSearchInline ProductSearchInline { get; set; }

        private Telegram.Bot.TelegramBotClient  TelegramBot { get; set; }

        private BotInfo BotInfo { get; set; }

        BotInline BotInline { get; set; }

       

        public InlineFind(InlineQuery query)
        {
            this.inlineQuery = query;
            this.From = this.inlineQuery.Query;

        }

        public async Task<bool> Response()
        {
            this.BotInfo = GeneralFunction.GetBotInfo();

            if (IsOwner(inlineQuery.From.Id) || IsOperator(inlineQuery.From.Id))
            {
                TelegramBot = new TelegramBotClient(BotInfo.Token);

                if (GetFrom(inlineQuery.Query) == FindOrder)
                    BotInline = new InlineResult.OrderSearchInline(QueryLine(inlineQuery.Query));


                else
                    BotInline = new InlineResult.ProductSearchInline(QueryLine(inlineQuery.Query),BotInfo);

                if (BotInline != null)
                    await TelegramBot.AnswerInlineQueryAsync(inlineQuery.Id, BotInline.GetResult());
            }

            return true;
        }

        public string GetFrom(string query)
        {
            try
            {
                if (query.IndexOf('|') > 0)
                    return query.Substring(0, query.IndexOf('|')); // определяем где проходит поиск

                else
                    return query;

                //return ArgumentsTableName[parse];
            }

            catch
            {
                return "";
            }
        }

        public string QueryLine (string query)
        {
            try
            {
                return query.Substring(query.IndexOf('|')+1); // определяем где проходит поиск
                //return ArgumentsTableName[parse];
            }

            catch
            {
                return "";
            }
        }
        private bool IsOwner (int ChatId)
        {
            return true;
        }

        private bool IsOperator(int ChatId)
        {
            return true;
        }


    }
}
