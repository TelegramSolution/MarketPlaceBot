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
using MyTelegramBot.Bot.AdminModule;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Сообщение с категориями товаров в виде кнопок
    /// </summary>
    public class CategoryListMessage:BotMessage
    {
        private string Cmd { get; set; }

        private string ModuleName { get; set; }

        private string BackCmd { get; set; }

        /// <summary>
        /// id товара которому нужно поменять категорию
        /// </summary>
        private int EditProductId { get; set; }

        private InlineKeyboardCallbackButton [][] CategoryListBtn { get; set; }

        private List<Category> Categorys { get; set; }

        private InlineKeyboardCallbackButton ViewAllBtn { get; set; }

        //флаг который показывает отображать ли кнопку "Показать все все товары"
        private bool VisableAllProductBtn { get; set; }

        private InlineKeyboardCallbackButton NextPageBtn { get; set; }

        private InlineKeyboardCallbackButton PreviusPageBtn { get; set; }


        /// <summary>
        /// Сформированные страницы с категориями
        /// </summary>
        private Dictionary<int, List<Category>> Pages { get; set; }

        private int PageCount { get; set; }

        private MarketBotDbContext db;

        public const string NextPageCmd = "NxtCatPage";

        public const string PreviuousPageCmd = "PrvCatPage";

        /// <summary>
        /// Для меню. Показывает кнопки всех категорий и кнопку "Показать весь ассортимент"
        /// </summary>
        public CategoryListMessage(int PageNumber=1)
        {
            this.VisableAllProductBtn = true;
            this.Cmd = "ProductInCategory";
            this.BackCmd = "MainMenu";
            this.ModuleName = Bot.CategoryBot.ModuleName;
            base.BackBtn = BuildInlineBtn("Назад", BuildCallData(this.BackCmd, Bot.MainMenuBot.ModuleName),base.Previuos2Emodji,false);
            this.SelectPageNumber = PageNumber;
            base.PageSize = 4;
        }

        public CategoryListMessage(string ModuleName,string CommandName, int PageNumber = 1)
        {
            base.PageSize = 4;
            this.SelectPageNumber = PageNumber;
            this.VisableAllProductBtn = false;
            this.Cmd = CommandName;
            this.ModuleName = ModuleName;
            this.BackBtn = BuildInlineBtn("Панель администратора", BuildCallData(Bot.AdminModule.AdminBot.BackToAdminPanelCmd, Bot.AdminModule.AdminBot.ModuleName),base.CogwheelEmodji);
        }

        public BotMessage BuildCategoryPage()
        {
            Pages = BuildPages();

            

            if (Pages.Count>0 && Pages.Count>=SelectPageNumber)
            {
                int count = 0;
                var page = Pages[SelectPageNumber];
                ViewAllBtn = BuildInlineBtn("Показать весь ассортимент",
                                                    BuildCallData("ViewAllProduct", Bot.CategoryBot.ModuleName),base.OpenedBookEmodji);

                if (Pages.Keys.Last() != SelectPageNumber && Pages[SelectPageNumber + 1] != null) // Находим следующую страницу 
                    NextPageBtn = BuildInlineBtn("Следующая. стр", BuildCallData(NextPageCmd, Bot.CategoryBot.ModuleName, SelectPageNumber + 1),base.Next2Emodji);

                if (Pages.Keys.Last() == SelectPageNumber && SelectPageNumber != 1 && Pages[1] != null)
                    // Если текущая страница является последней, то делаем кнопку с сылкой на первую,
                    //но при это проверяем не является ли текущая страница первой
                    NextPageBtn = BuildInlineBtn("Следующая. стр", BuildCallData(NextPageCmd, Bot.CategoryBot.ModuleName, 1), base.Next2Emodji);

                //находим предыдующую стр.
                if (SelectPageNumber > 1 && Pages[SelectPageNumber - 1] != null)
                    PreviusPageBtn =BuildInlineBtn("Предыдущая. стр", BuildCallData(PreviuousPageCmd, Bot.CategoryBot.ModuleName, SelectPageNumber - 1),base.Previuos2Emodji,false);

                if (SelectPageNumber == 1 && Pages.Keys.Last() != 1)
                    PreviusPageBtn = BuildInlineBtn("Предыдущая. стр", BuildCallData(PreviuousPageCmd, Bot.CategoryBot.ModuleName, Pages.Keys.Last()),base.Previuos2Emodji,false);


                if (VisableAllProductBtn && NextPageBtn != null && PreviusPageBtn != null)
                {
                    CategoryListBtn = new InlineKeyboardCallbackButton[page.Count + 3][];

                    CategoryListBtn[page.Count + 1] = new InlineKeyboardCallbackButton[1];

                    CategoryListBtn[page.Count + 1][0] = ViewAllBtn;

                    CategoryListBtn[page.Count] = new InlineKeyboardCallbackButton[2];

                    CategoryListBtn[page.Count][0] = PreviusPageBtn;

                    CategoryListBtn[page.Count][1] = NextPageBtn;
                }

                if (VisableAllProductBtn && NextPageBtn == null && PreviusPageBtn == null)
                {
                    CategoryListBtn = new InlineKeyboardCallbackButton[page.Count + 2][];

                    CategoryListBtn[page.Count] = new InlineKeyboardCallbackButton[1];

                    CategoryListBtn[page.Count][0] = ViewAllBtn;

                }

                if (!VisableAllProductBtn && NextPageBtn != null && PreviusPageBtn != null)
                {
                    CategoryListBtn = new InlineKeyboardCallbackButton[page.Count + 2][];

                    CategoryListBtn[page.Count] = new InlineKeyboardCallbackButton[2];

                    CategoryListBtn[page.Count][0] = PreviusPageBtn;

                    CategoryListBtn[page.Count][1] = NextPageBtn;
                }

                CategoryListBtn[CategoryListBtn.Length - 1] = new InlineKeyboardCallbackButton[1];

                CategoryListBtn[CategoryListBtn.Length - 1][0] = BackBtn;


                foreach (Category cat in page)
                {
                    InlineKeyboardCallbackButton button = BuildInlineBtn(cat.Name,
                        base.BuildCallData(Cmd, ModuleName, cat.Id),BlueRhombus,false);
                    CategoryListBtn[count] = new InlineKeyboardCallbackButton[1];
                    CategoryListBtn[count][0] = button;

                    count++;
                }

                base.MessageReplyMarkup = new InlineKeyboardMarkup(CategoryListBtn);

                base.TextMessage = "Выберите категорию:" + NewLine() + "Всего категорий: " + Categorys.Count.ToString()
                    + NewLine() + "стр. " + SelectPageNumber.ToString() + " из " + PageCount.ToString();

                return this;
            }

            else
                return null;
        }

        public BotMessage BuildCategoryAdminPage()
        {
            Pages = BuildPages(false);

            var page = Pages[SelectPageNumber];

            int count = 0;

            if (Pages.Keys.Last() != SelectPageNumber && Pages[SelectPageNumber + 1] != null) // Находим следующую страницу 
                NextPageBtn = BuildInlineBtn("Следующая стр.", BuildCallData(NextPageCmd, ModuleName, SelectPageNumber + 1),base.Next2Emodji);

            if (Pages.Keys.Last() == SelectPageNumber && SelectPageNumber != 1 && Pages[1] != null)
                // Если текущая страница является последней, то делаем кнопку с сылкой на первую,
                //но при это проверяем не является ли текущая страница первой
                NextPageBtn = BuildInlineBtn("Следующая стр.", BuildCallData(NextPageCmd, ModuleName, 1), base.Next2Emodji);

            //находим предыдующую стр.
            if (SelectPageNumber > 1 && Pages[SelectPageNumber - 1] != null)
                PreviusPageBtn = BuildInlineBtn("Предыдущая стр.", BuildCallData(PreviuousPageCmd, ModuleName, SelectPageNumber - 1),base.Previuos2Emodji,false);

            if (SelectPageNumber == 1 && Pages.Keys.Last() != 1)
                PreviusPageBtn = BuildInlineBtn("Предыдущая стр.", BuildCallData(PreviuousPageCmd, ModuleName, Pages.Keys.Last()), base.Previuos2Emodji, false);


            if (NextPageBtn != null && PreviusPageBtn != null)
            {
                CategoryListBtn = new InlineKeyboardCallbackButton[page.Count + 2][];

                CategoryListBtn[page.Count] = new InlineKeyboardCallbackButton[2];

                CategoryListBtn[page.Count][0] = PreviusPageBtn;

                CategoryListBtn[page.Count][1] = NextPageBtn;
            }

            else
            {
                CategoryListBtn = new InlineKeyboardCallbackButton[page.Count+1][];
            }

            CategoryListBtn[CategoryListBtn.Length - 1] = new InlineKeyboardCallbackButton[1];

            CategoryListBtn[CategoryListBtn.Length - 1][0] = BackBtn;


            foreach (Category cat in page)
            {
                InlineKeyboardCallbackButton button = new InlineKeyboardCallbackButton(cat.Name,
                    base.BuildCallData(Cmd, ModuleName, cat.Id));
                CategoryListBtn[count] = new InlineKeyboardCallbackButton[1];
                CategoryListBtn[count][0] = button;

                count++;
            }

            base.MessageReplyMarkup = new InlineKeyboardMarkup(CategoryListBtn);

            base.TextMessage = "Выберите категорию:" + NewLine() + "Всего категорий: " + Categorys.Count.ToString()
                + NewLine() + "стр. " + SelectPageNumber.ToString() + " из " + PageCount.ToString();

            return this;
        }

        /// <summary>
        /// Создаем страницы с категориями
        /// </summary>
        /// <param name="Enable">true- показывать только активные категории, false - все</param>
        /// <returns></returns>
        private Dictionary<int, List<Category>> BuildPages(bool Enable=true)
        {
            db = new MarketBotDbContext();

            if(Enable)
                Categorys = db.Category.Where(c => c.Enable).ToList();

            else
                Categorys = db.Category.ToList();

            db.Dispose();

            if (Categorys.Count % PageSize > 0) // Определяем сколько всего будет страниц
                PageCount = (Categorys.Count / PageSize) + 1;

            else
                PageCount = Categorys.Count / PageSize;


            Pages = new Dictionary<int, List<Category>>();

            //начинаем заполнять

            for (int i = 0; i < PageCount; i++)
            {
                List<Category> list = new List<Category>();

                for (int j = 0; j < PageSize; j++)
                {
                    if ((i * PageSize + j) < Categorys.Count)
                        list.Add(Categorys.ElementAt(i * PageSize + j));

                    else
                        break;
                }
                Pages.Add(i + 1, list);
                    
            }

            return Pages;
        }



    }
}
