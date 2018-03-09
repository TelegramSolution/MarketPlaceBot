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
        /// <summary>
        /// поиск по заказам
        /// </summary>
        public const string FindOrder = "Заказы";

        /// <summary>
        /// поиск доп фоток у товара
        /// </summary>
        public const string AdditionalProduct = "Доп.фото";

        /// <summary>
        /// поиск по пользователям
        /// </summary>
        public const string FindUsers = "Пользователи";

        public const string FindOperators = "Операторы";

        /// <summary>
        /// поиск по товарам
        /// </summary>
        public const string PhotoCatalog = "Фотокаталог";

        public const string SearchProduct = "Поиск";

        public const string EditProduct = "Редактор";

        public const string MyOrders = "Мои заказы";

        public const string HelpdDesk = "Desk";

        private InlineQuery inlineQuery { get; set; }

        /// <summary>
        /// имя таблицы в которой будет производить поиск данных
        /// </summary>
        private string From { get; set; }

        /// <summary>
        /// поле -> значение которое нужно найти в этом поле. Например Телефон:890000000181 или же Where Telephone=890000000181
        /// </summary>
        private Dictionary <string,string> FindArgument { get; set; }



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

                if(GetFrom(inlineQuery.Query) == FindUsers)
                    BotInline = new InlineResult.FollowerInlineSearchInline(QueryLine(inlineQuery.Query));

                if (GetFrom(inlineQuery.Query) == PhotoCatalog)
                    BotInline = new InlineResult.PhotoCatalogInline(QueryLine(inlineQuery.Query),BotInfo);

                if (GetFrom(inlineQuery.Query) == SearchProduct)
                    BotInline = new InlineResult.ProductSearchInline(QueryLine(inlineQuery.Query));

                if (GetFrom(inlineQuery.Query) == EditProduct)
                    BotInline = new InlineResult.AdminProductSearchInline(QueryLine(inlineQuery.Query));

                if (GetFrom(inlineQuery.Query) == FindOperators)
                    BotInline = new InlineResult.OperatorSearchInline(QueryLine(inlineQuery.Query));

                if (GetFrom(inlineQuery.Query) == HelpdDesk)
                    BotInline = new InlineResult.HelpDeskSearchInline(QueryLine(inlineQuery.Query));

            }

            else
            {

                if (GetFrom(inlineQuery.Query) == MyOrders)
                    BotInline = new InlineResult.MyOrdersSearchInline(QueryLine(inlineQuery.Query), inlineQuery.From.Id);

                if (GetFrom(inlineQuery.Query) == PhotoCatalog)
                    BotInline = new InlineResult.PhotoCatalogInline(QueryLine(inlineQuery.Query), BotInfo);

                if (GetFrom(inlineQuery.Query) == SearchProduct)
                    BotInline = new InlineResult.ProductSearchInline(QueryLine(inlineQuery.Query));

                if (BotInline != null)
                    await TelegramBot.AnswerInlineQueryAsync(inlineQuery.Id, BotInline.GetResult());
            }

            try
            {
                if (BotInline != null)
                    await TelegramBot.AnswerInlineQueryAsync(inlineQuery.Id, BotInline.GetResult());
            }
            catch
            {

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
                string res = query.Substring(query.IndexOf('|') + 1);

                return res; 

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
