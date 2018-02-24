using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;
using System.Web;
using Telegram.Bot.Types.InlineKeyboardButtons;



namespace MyTelegramBot.Bot
{
    public class BotMessage
    {
        /// <summary>
        /// ✔️
        /// </summary>
        protected readonly string CheckEmodji = "\u2714\ufe0f";

        protected readonly string UnCheckEmodji = "\ud83d\udd32";

        /// <summary>
        /// 🔹 - синий ромб
        /// </summary>
        protected readonly string BlueRhombus = "\ud83d\udd39";

        /// <summary>
        /// 🔸 - золотой ромб
        /// </summary>
        protected readonly string GoldRhobmus = "\ud83d\udd38";

        /// <summary>
        /// ⚠️ - Воскл. знак
        /// </summary>
        protected readonly string WarningEmodji = "\u26a0\ufe0f";

        /// <summary>
        /// 🛒 - Корзина
        /// </summary>
        protected readonly string BasketEmodji = "\ud83d\uded2";

        /// <summary>
        /// ⚙️ - Шестеренка
        /// </summary>
        protected readonly string CogwheelEmodji = "\u2699\ufe0f";

        /// <summary>
        /// 🖊 - Ручка
        /// </summary>
        protected readonly string PenEmodji = "\ud83d\udd8a";

        /// <summary>
        /// 🏠 - Домик
        /// </summary>
        protected readonly string HouseEmodji = "\ud83c\udfe0";

        /// <summary>
        /// 🚚 - Машина
        /// </summary>
        protected readonly string CarEmodji = "\ud83d\ude9a";

        /// <summary>
        /// 🙋🏻‍♂️ - Человек
        /// </summary>
        protected readonly string ManEmodji = "\ud83d\ude4b\ud83c\udffb\u200d\u2642\ufe0f";

        /// <summary>
        /// ⭐️- Звезда
        /// </summary>
        protected readonly string StartEmodji = "\u2b50\ufe0f";

        /// <summary>
        /// ➡️
        /// </summary>
        protected readonly string NextEmodji = "\u27a1\ufe0f";

        /// <summary>
        /// ⬅️
        /// </summary>
        protected readonly string PreviuosEmodji = "\u2b05\ufe0f";

        /// <summary>
        /// ◀️
        /// </summary>
        protected readonly string Previuos2Emodji = "\u25c0\ufe0f";

        /// <summary>
        /// ▶️
        /// </summary>
        protected readonly string Next2Emodji = "\u25b6\ufe0f";

        /// <summary>
        /// 💰 - мешочек с деньгами
        /// </summary>
        protected readonly string CashEmodji = "\ud83d\udcb0";

        /// <summary>
        /// ⚖️ весы
        /// </summary>
        protected readonly string WeigherEmodji = "\u2696\ufe0f";

        /// <summary>
        /// 🖼 - картина
        /// </summary>
        protected readonly string PictureEmodji = "\ud83d\uddbc";

        /// <summary>
        /// 📝 - тетрадь с ручкой
        /// </summary>
        protected readonly string NoteBookEmodji = "\ud83d\udcdd";

        /// <summary>
        /// 📉 - график
        /// </summary>
        protected readonly string DepthEmodji = "\ud83d\udcc9";

        /// <summary>
        /// 📤 - отправить
        /// </summary>
        protected readonly string SenderEmodji = "\ud83d\udce4";

        /// <summary>
        /// 📜 - лист
        /// </summary>
        protected readonly string PaperEmodji = "\ud83d\udcdc";


        /// <summary>
        /// ❌ - красный крест
        /// </summary>
        protected readonly string CrossEmodji = "\u274c";

        /// <summary>
        /// ✅ 
        /// </summary>
        protected readonly string DoneEmodji = "\u2705";

        public BotMessage()
        {
          
        }

        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string TextMessage { get; set; }

        /// <summary>
        /// Клавиатула из Inline кнопок
        /// </summary>
        public IReplyMarkup MessageReplyMarkup { get; set; }

        /// <summary>
        /// текст для AnswerCallbackQueryAsync
        /// </summary>
        public string CallBackTitleText { get; set; }

        public string Url { get; set; }

        /// <summary>
        /// Кнопка назад. Для некоторых случаев
        /// </summary>
        protected InlineKeyboardCallbackButton BackBtn { get; set; }


        public MediaFile MediaFile { get; set; }

        public virtual BotMessage BuildMsg()
        {
            return this;
        }

        /// <summary>
        /// Создает Inline кнопку 
        /// </summary>
        /// <param name="Text">текст на кнопке</param>
        /// <param name="CallData">данные внутри кнопки</param>
        /// <param name="Emodji">эмоджи</param>
        /// <param name="TextFirst">флаг указывающий на то что сначала на кнопке рисуется текст, а потом эмоджи. Если False, то сначала рисуется эмоджи а потом текст</param>
        /// <returns></returns>
        protected InlineKeyboardCallbackButton BuildInlineBtn(string Text, string CallData, string Emodji=null, bool TextFirst=true)
        {
            if(Emodji!=null && TextFirst)
                return new InlineKeyboardCallbackButton(Text + " " + Emodji, CallData);

            if (Emodji != null && !TextFirst)
                return new InlineKeyboardCallbackButton(Emodji+" " + Text, CallData);

            else
                return new InlineKeyboardCallbackButton(Text, CallData);

        }

        /// <summary>
        /// json объект который будет находится внутри Inline кнопки в поле CallBackData
        /// </summary>
        /// <param name="CommandName">название команды / функции</param>
        /// <param name="ModuleName">модуель к которому относится команда</param>
        /// <param name="Argument">аргументы</param>
        /// <returns></returns>
        public string BuildCallData (string CommandName,string ModuleName , params int [] Argument)
        {
            BotCommand command = new BotCommand
            {
                Cmd = CommandName,
                Arg = new List<int>(),
                M= ModuleName
            };

            for (int i = 0; i < Argument.Length; i++)
                command.Arg.Add(Argument[i]);

            return JsonConvert.SerializeObject(command);
        }

        public static string Bold(string value)
        {
            return "<b>" + value + "</b>";
        }

        public static string Italic(string value)
        {
            return "<i>" + value + "</i>";
        }

        public static string NewLine()
        {
            return "\r\n";
        }

        public static string HrefUrl(string url, string text)
        {
            const string quote = "\"";
            return "<a href=" + quote+ url + quote+ ">" + text + "</a>";
        }
    }

}
