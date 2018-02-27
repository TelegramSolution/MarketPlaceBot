using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using MyTelegramBot.Bot;

namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// Сообщение со списком товаров в виде кнопок
    /// </summary>
    public class AdminProductListMessage:BotMessage
    {
        /// <summary>
        /// товары какой категории мы отображаем
        /// </summary>
        int CategoryId { get; set; }

        InlineKeyboardCallbackButton[][] ProductListBtn { get; set; }

        private List<Product> ProductList { get; set; }

        private InlineKeyboardCallbackButton NextPageBtn { get; set; }

        private InlineKeyboardCallbackButton PreviusPageBtn { get; set; }

        private const int PageSize = 4;

        /// <summary>
        /// Сформированные страницы 
        /// </summary>
        private Dictionary<int, List<Product>> Pages { get; set; }

        private int PageCount { get; set; }

        private MarketBotDbContext db;

        private int PageNumber { get; set; }

        public const string NextPageCmd = "NxtProdPage";

        public const string PreviuousPageCmd = "PrvProdPage";

        public AdminProductListMessage(int CategoryId,int PageNumber=1)
        {
            this.PageNumber = PageNumber;
            this.CategoryId = CategoryId;
            base.BackBtn = BuildInlineBtn("Панель администратора", BuildCallData(Bot.AdminModule.AdminBot.BackToAdminPanelCmd, Bot.AdminModule.AdminBot.ModuleName));
        }
        public override BotMessage BuildMsg()
        {     

            Pages = BuildPages();

            if (Pages.Count > 0)
            {
                base.TextMessage = "Страница " + PageNumber.ToString() + " из " + Pages.Count.ToString() + NewLine()
                    + "Выберите товар который хотите изменить:";

                var page = Pages[PageNumber];

                int count = 0;

                if (Pages.Keys.Last() != PageNumber && Pages[PageNumber + 1] != null) // Находим следующую страницу 
                    NextPageBtn = BuildInlineBtn("Следующая. стр", BuildCallData(NextPageCmd, Bot.ProductEditBot.ModuleName, CategoryId, PageNumber + 1), base.Next2Emodji);

                if (Pages.Keys.Last() == PageNumber && PageNumber != 1 && Pages[1] != null)
                    // Если текущая страница является последней, то делаем кнопку с сылкой на первую,
                    //но при это проверяем не является ли текущая страница первой
                    NextPageBtn = BuildInlineBtn("Следующая. стр", BuildCallData(NextPageCmd, Bot.ProductEditBot.ModuleName, CategoryId, 1), base.Next2Emodji);

                //находим предыдующую стр.
                if (PageNumber > 1 && Pages[PageNumber - 1] != null)
                    PreviusPageBtn = BuildInlineBtn("Предыдущая. стр", BuildCallData(PreviuousPageCmd, Bot.ProductEditBot.ModuleName, CategoryId, PageNumber - 1), base.Previuos2Emodji, false);

                if (PageNumber == 1 && Pages.Keys.Last() != 1)
                    PreviusPageBtn = BuildInlineBtn("Предыдущая. стр", BuildCallData(PreviuousPageCmd, Bot.ProductEditBot.ModuleName, CategoryId, Pages.Keys.Last()), base.Previuos2Emodji, false);

                if (NextPageBtn != null && PreviusPageBtn != null)
                {
                    ProductListBtn = new InlineKeyboardCallbackButton[page.Count + 2][];

                    ProductListBtn[page.Count] = new InlineKeyboardCallbackButton[2];

                    ProductListBtn[page.Count][0] = PreviusPageBtn;

                    ProductListBtn[page.Count][1] = NextPageBtn;

                }

                else
                    ProductListBtn = new InlineKeyboardCallbackButton[page.Count+1][];

                ProductListBtn[ProductListBtn.Length - 1] = new InlineKeyboardCallbackButton[1];

                ProductListBtn[ProductListBtn.Length - 1][0] = BackBtn;

                foreach (Product prod in page)
                {
                    InlineKeyboardCallbackButton button = BuildInlineBtn(prod.Name,
                        base.BuildCallData(Bot.ProductEditBot.SelectProductCmd, Bot.ProductEditBot.ModuleName, prod.Id), base.PenEmodji);
                    ProductListBtn[count] = new InlineKeyboardCallbackButton[1];
                    ProductListBtn[count][0] = button;

                    count++;
                }

                base.MessageReplyMarkup = new InlineKeyboardMarkup(ProductListBtn);

                return this;
            }

            else
                return null;
           
        }


        /// <summary>
        /// Создаем страницы с категориями
        /// </summary>
        /// <param name="Enable">true- показывать только активные категории, false - все</param>
        /// <returns></returns>
        private Dictionary<int, List<Product>> BuildPages()
        {
            db = new MarketBotDbContext();

            ProductList = db.Product.Where(p => p.CategoryId == CategoryId).ToList();



            db.Dispose();

            if (ProductList.Count % PageSize > 0) // Определяем сколько всего будет страниц
                PageCount = (ProductList.Count / PageSize) + 1;

            else
                PageCount = ProductList.Count / PageSize;


            Pages = new Dictionary<int, List<Product>>();

            //начинаем заполнять

            for (int i = 0; i < PageCount; i++)
            {
                List<Product> list = new List<Product>();

                for (int j = 0; j < PageSize; j++)
                {
                    if ((i * PageSize + j) < ProductList.Count)
                        list.Add(ProductList.ElementAt(i * PageSize + j));

                    else
                        break;
                }
                Pages.Add(i + 1, list);

            }

            return Pages;
        }
    }
}
