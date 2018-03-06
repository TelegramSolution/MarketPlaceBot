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

namespace MyTelegramBot.Bot.Core
{
    public abstract class BotInline
    {

        public BotInline(string Query)
        {
            this.Query = Query;
        }
        protected string Query { get; set; }

        protected string SqlQuery { get; set; }

        protected SqlParameter param { get; set; }

        protected const string NotFoundImg = "http://proxyprivat.com/images/noimage.jpeg";

        protected MarketBotDbContext db { get; set; }

        protected InlineKeyboardMarkup keyboardMarkup { get; set; }
        public abstract InlineQueryResult[] GetResult();

        /// <summary>
        /// json объект который будет находится внутри Inline кнопки в поле CallBackData
        /// </summary>
        /// <param name="CommandName">название команды / функции</param>
        /// <param name="ModuleName">модуель к которому относится команда</param>
        /// <param name="Argument">аргументы</param>
        /// <returns></returns>
        public string BuildCallData(string CommandName, string ModuleName, params int[] Argument)
        {
            BotCommand command = new BotCommand
            {
                Cmd = CommandName,
                Arg = new List<int>(),
                M = ModuleName
            };

            for (int i = 0; i < Argument.Length; i++)
                command.Arg.Add(Argument[i]);

            return JsonConvert.SerializeObject(command);
        }

        /// <summary>
        /// Создает Inline кнопку 
        /// </summary>
        /// <param name="Text">текст на кнопке</param>
        /// <param name="CallData">данные внутри кнопки</param>
        /// <param name="Emodji">эмоджи</param>
        /// <param name="TextFirst">флаг указывающий на то что сначала на кнопке рисуется текст, а потом эмоджи. Если False, то сначала рисуется эмоджи а потом текст</param>
        /// <returns></returns>
        protected InlineKeyboardCallbackButton BuildInlineBtn(string Text, string CallData, string Emodji = null, bool TextFirst = true)
        {
            if (Emodji != null && TextFirst)
                return new InlineKeyboardCallbackButton(Text + " " + Emodji, CallData);

            if (Emodji != null && !TextFirst)
                return new InlineKeyboardCallbackButton(Emodji + " " + Text, CallData);

            else
                return new InlineKeyboardCallbackButton(Text, CallData);

        }

    }


}
