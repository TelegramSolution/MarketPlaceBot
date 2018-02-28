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
        /// 🛍 - пакеты
        /// </summary>
        protected readonly string PackageEmodji = "\ud83d\udecd";

        /// <summary>
        /// 🙍🏻‍♂️ чувак
        /// </summary>
        protected readonly string ManEmodji2 = "\ud83d\ude4d\ud83c\udffb\u200d\u2642\ufe0f";

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
        /// 👨🏻‍💻
        /// </summary>
        protected readonly string ManAndComputerEmodji = "\ud83d\udc68\ud83c\udffb\u200d\ud83d\udcbb";

        /// <summary>
        /// 🏘
        /// </summary>
        protected readonly string Build2Emodji = "\ud83c\udfd8";

        /// <summary>
        /// 📊
        /// </summary>
        protected readonly string Depth2Emodji = "\ud83d\udcca";

        /// <summary>
        /// 💳
        /// </summary>
        protected readonly string CreditCardEmodji = "\ud83d\udcb3";

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

        /// <summary>
        /// кнопка "предыдущая страница"
        /// </summary>
        protected InlineKeyboardCallbackButton PreviousPageBtn { get; set; }

        /// <summary>
        /// кнопка "следующая страница"
        /// </summary>
        protected InlineKeyboardCallbackButton NextPageBtn { get; set; }

        /// <summary>
        /// номер страницы, из ассоциативного массива страниц (номер страницы-> массив с данными)
        /// который нужно отобразить при отправке сообщения 
        /// </summary>
        protected int SelectPageNumber { get; set; }

        /// <summary>
        /// данная переменная определяет количество записией на странице 
        /// </summary>
        protected int PageSize { get; set; }

        /// <summary>
        /// медиа файл. фото/видео/аудио/документ и тд
        /// </summary>
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
        /// Разбить информацию на страницы. Например у нас есть выборка данных из бд (20 записей из табл. Orders)
        /// и мы хотим отобразить ее в сообщении.Делаем это Для того что бы сообщение не было очень большим и его не нужно было
        /// проматывать. Будет одно небольшое сообщение и кнопки Вперед Назад, Нажимая на них будет отображаться информация
        /// уже из другой страницы (в то же сообщении без отправления нового)
        /// </summary>
        /// <typeparam name="T">тип данных в массиве List</typeparam>
        /// <param name="PageSize">Кол-во записей на стр</param>
        /// <param name="List">Массив объектов. Например массив записей из таблицы Orders</param>
        /// <returns></returns>
        protected Dictionary<int,List<T>> BuildDataPage<T>(List<T> List,int PageSize=4)
        {
            int PageCount=0;
            Dictionary<int, List<T>> Pages = new Dictionary<int, List<T>>();

            if (List.Count % PageSize > 0) // Определяем сколько всего будет страниц
                PageCount = (List.Count / PageSize) + 1;

            else
                PageCount = List.Count / PageSize;


            for (int i = 0; i < PageCount; i++)
            {
                List<T> list = new List<T>();

                for (int j = 0; j < PageSize; j++)
                {
                    if ((i * PageSize + j) < List.Count)
                        list.Add(List.ElementAt(i * PageSize + j));

                    else
                        break;
                }
                Pages.Add(i + 1, list);

            }

            return Pages;

        }

        /// <summary>
        /// Создать кнопку "Предыдущая страница" для навигации по страницам
        /// </summary>
        /// <typeparam name="T">модель данных из бд</typeparam>
        /// <param name="Pages">ассоциативный массив с страницами и записями на этих страницах</param>
        /// <param name="SelectPageNumber">Выбранная пользователем стр. которая должна быть показана ему</param>
        /// <param name="CmdName">название команды для кнопки</param>
        /// <param name="CmdModuleName">к какому модулю относится эта команда</param>
        /// <param name="BtnText">Текст на кнопке</param>
        /// <param name="Arg">Аргументы</param>
        /// <returns></returns>
        protected InlineKeyboardCallbackButton BuildPreviousPageBtn<T>(Dictionary<int,List<T>> Pages,int SelectPageNumber,string CmdName,string CmdModuleName,string BtnText= "", params int [] Argument)
        {
            //проверяемя не является ли выбранная пользователем стр. первой по счету из всех страниц
            if (SelectPageNumber > 1 && Pages[SelectPageNumber - 1] != null) //
                return BuildInlineBtn(BtnText, BuildCallData(CmdName, CmdModuleName,InsertFirstItemToArray(SelectPageNumber-1, Argument)), Previuos2Emodji, false);

            //если пользователь выбрал первую стр. то предыдущей стриницей станет послденяя страница из всех существующих
            if (SelectPageNumber == 1 && Pages.Keys.Last() != 1)
                return BuildInlineBtn(BtnText, BuildCallData(CmdName, CmdModuleName, InsertFirstItemToArray(Pages.Keys.Last(), Argument)), Previuos2Emodji, false);

            else
                return null;

        }


        /// <summary>
        /// Создать кнопку "Следующая страница" для навигации по страницам
        /// </summary>
        /// <typeparam name="T">модель данных из бд</typeparam>
        /// <param name="Pages">ассоциативный массив с страницами и записями на этих страницах</param>
        /// <param name="SelectPageNumber">Выбранная пользователем стр. которая должна быть показана ему</param>
        /// <param name="CmdName">название команды для кнопки</param>
        /// <param name="CmdModuleName">к какому модулю относится эта команда</param>
        /// <param name="BtnText">Текст на кнопке</param>
        /// <param name="Argument">Аргументы</param>
        /// <returns></returns>
        protected InlineKeyboardCallbackButton BuildNextPageBtn<T>(Dictionary<int, List<T>> Pages, int SelectPageNumber, string CmdName, string CmdModuleName, string BtnText = "", params int[] Argument)
        {
            //Проверяем не является ли выбранная пользователем стр. последеней по счету 
            if (Pages.Keys.Last() != SelectPageNumber && Pages[SelectPageNumber + 1] != null)
                return BuildInlineBtn(BtnText, BuildCallData(CmdName, CmdModuleName, InsertFirstItemToArray(SelectPageNumber + 1,Argument)), Next2Emodji);

            if (Pages.Keys.Last() == SelectPageNumber && SelectPageNumber != 1 && Pages[1] != null)
                // Если выбранная пользователем страница является последней, то делаем кнопку с сылкой на первую,
                //но при это проверяем не является ли выбранная пользователем  страница первой
                return BuildInlineBtn(BtnText, BuildCallData(CmdName, CmdModuleName, InsertFirstItemToArray(1, Argument)), Next2Emodji);

            else
                return null;

        }

        /// <summary>
        /// Создать клавиатуру для навигации по страницам
        /// </summary>
        /// <param name="NextBtn">кнопка "след. запись"</param>
        /// <param name="PrevBtn">кнопка "пред. запись"</param>
        /// <param name="BackBtn">кнопка "вернуться назад"</param>
        /// <returns></returns>
        protected IReplyMarkup PageNavigatorKeyboard(InlineKeyboardCallbackButton NextBtn, InlineKeyboardCallbackButton PrevBtn, InlineKeyboardCallbackButton BackBtn)
        {
            if (NextBtn !=null && PrevBtn !=null && BackBtn !=null )
            {
                return new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            PrevBtn,
                            NextBtn
                        },
                new[]
                        {
                            BackBtn
                        }



                });
            }

            if (NextBtn == null && PrevBtn == null && BackBtn != null)
            {
                return new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            BackBtn
                        },

                });
            }

            else
                return null;
        }

        private int [] InsertFirstItemToArray (int FirstItem, params int [] Argument)
        {
            if(Argument!=null && Argument.Length > 0)
            {
                int[] res = new int[Argument.Length + 1];

                res[0] = FirstItem;

                for (int i=0;i< Argument.Length + 1; i++)
                {
                    res[i + 1] = Argument[i];
                }

                return res;
            }

            else
            {
                int[] res = new int[1];
                res[0] = FirstItem;
                return res;
            }
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
