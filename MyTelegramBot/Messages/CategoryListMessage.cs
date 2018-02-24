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

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Сообщение с категориями товаров в виде кнопок
    /// </summary>
    public class CategoryListMessage:Bot.BotMessage
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

        private const int PageSize = 4;

        /// <summary>
        /// Сформированные страницы с категориями
        /// </summary>
        private Dictionary<int, List<Category>> Pages { get; set; }

        private int PageCount { get; set; }

        private MarketBotDbContext db;

        private int PageNumber { get; set; }

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
            base.BackBtn = BuildInlineBtn("Назад", BuildCallData(this.BackCmd, Bot.MainMenuBot.ModuleName));
            this.PageNumber = PageNumber;
        }

        public CategoryListMessage(string ModuleName,string CommandName, int PageNumber = 1)
        {
            this.PageNumber = PageNumber;
            this.VisableAllProductBtn = false;
            this.Cmd = CommandName;
            this.ModuleName = ModuleName;
            this.BackBtn = BuildInlineBtn("Панель администратора", BuildCallData(Bot.AdminModule.AdminBot.BackToAdminPanelCmd, Bot.AdminModule.AdminBot.ModuleName),base.CogwheelEmodji);
        }

        public Bot.BotMessage BuildCategoryPage()
        {
            Pages = BuildPages();

            

            if (Pages.Count>0 && Pages.Count>=PageNumber)
            {
                int count = 0;
                var page = Pages[PageNumber];
                ViewAllBtn = new InlineKeyboardCallbackButton("Показать весь ассортимент",
                                                    BuildCallData("ViewAllProduct", Bot.CategoryBot.ModuleName));

                if (Pages.Keys.Last() != PageNumber && Pages[PageNumber + 1] != null) // Находим следующую страницу 
                    NextPageBtn = BuildInlineBtn("Следующая. стр", BuildCallData(NextPageCmd, Bot.CategoryBot.ModuleName, PageNumber + 1),base.Next2Emodji);

                if (Pages.Keys.Last() == PageNumber && PageNumber != 1 && Pages[1] != null)
                    // Если текущая страница является последней, то делаем кнопку с сылкой на первую,
                    //но при это проверяем не является ли текущая страница первой
                    NextPageBtn = BuildInlineBtn("Следующая. стр", BuildCallData(NextPageCmd, Bot.CategoryBot.ModuleName, 1), base.Next2Emodji);

                //находим предыдующую стр.
                if (PageNumber > 1 && Pages[PageNumber - 1] != null)
                    PreviusPageBtn =BuildInlineBtn("Предыдущая. стр", BuildCallData(PreviuousPageCmd, Bot.CategoryBot.ModuleName, PageNumber - 1),base.Previuos2Emodji,false);

                if (PageNumber == 1 && Pages.Keys.Last() != 1)
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
                    InlineKeyboardCallbackButton button = new InlineKeyboardCallbackButton(cat.Name,
                        base.BuildCallData(Cmd, ModuleName, cat.Id));
                    CategoryListBtn[count] = new InlineKeyboardCallbackButton[1];
                    CategoryListBtn[count][0] = button;

                    count++;
                }

                base.MessageReplyMarkup = new InlineKeyboardMarkup(CategoryListBtn);

                base.TextMessage = "Выберите категорию:" + NewLine() + "Всего категорий: " + Categorys.Count.ToString()
                    + NewLine() + "стр. " + PageNumber.ToString() + " из " + PageCount.ToString();

                return this;
            }

            else
                return null;
        }

        public Bot.BotMessage BuildCategoryAdminPage()
        {
            Pages = BuildPages(false);

            var page = Pages[PageNumber];

            int count = 0;

            if (Pages.Keys.Last() != PageNumber && Pages[PageNumber + 1] != null) // Находим следующую страницу 
                NextPageBtn = BuildInlineBtn("Следующая стр.", BuildCallData(NextPageCmd, ModuleName, PageNumber + 1),base.Next2Emodji);

            if (Pages.Keys.Last() == PageNumber && PageNumber != 1 && Pages[1] != null)
                // Если текущая страница является последней, то делаем кнопку с сылкой на первую,
                //но при это проверяем не является ли текущая страница первой
                NextPageBtn = BuildInlineBtn("Следующая стр.", BuildCallData(NextPageCmd, ModuleName, 1), base.Next2Emodji);

            //находим предыдующую стр.
            if (PageNumber > 1 && Pages[PageNumber - 1] != null)
                PreviusPageBtn = BuildInlineBtn("Предыдущая стр.", BuildCallData(PreviuousPageCmd, ModuleName, PageNumber - 1),base.Previuos2Emodji,false);

            if (PageNumber == 1 && Pages.Keys.Last() != 1)
                PreviusPageBtn = BuildInlineBtn("Предыдущая стр.", BuildCallData(PreviuousPageCmd, ModuleName, Pages.Keys.Last()), base.Previuos2Emodji, false);


            if (NextPageBtn != null && PreviusPageBtn != null)
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
                InlineKeyboardCallbackButton button = new InlineKeyboardCallbackButton(cat.Name,
                    base.BuildCallData(Cmd, ModuleName, cat.Id));
                CategoryListBtn[count] = new InlineKeyboardCallbackButton[1];
                CategoryListBtn[count][0] = button;

                count++;
            }

            base.MessageReplyMarkup = new InlineKeyboardMarkup(CategoryListBtn);

            base.TextMessage = "Выберите категорию:" + NewLine() + "Всего категорий: " + Categorys.Count.ToString()
                + NewLine() + "стр. " + PageNumber.ToString() + " из " + PageCount.ToString();

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
