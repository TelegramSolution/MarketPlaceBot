using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyTelegramBot.Messages;

namespace MyTelegramBot.Bot
{
    public class CategoryBot:BotCore
    {
        public const string ModuleName = "Cat";

        private int CategoryId { get; set; }

        private Category Category { get; set; }

        ProductViewMessage ProductViewMsg { get; set; }

        ViewAllProductInCategoryMessage ViewAllProductInCategoryMsg { get; set; }

        public CategoryBot(Update _update) : base(_update)
        {
           
        }

        protected override void Constructor()
        {
            try
            {
                if (this.Argumetns.Count > 0)
                {
                    CategoryId = Argumetns[0];
                    using (MarketBotDbContext db = new MarketBotDbContext())
                        Category = db.Category.Where(c => c.Id == this.CategoryId).FirstOrDefault();

                    ProductViewMsg = new ProductViewMessage(Category,BotInfo.Id);
                }

                ViewAllProductInCategoryMsg = new ViewAllProductInCategoryMessage();
            }

            catch
            {

            }
        }
        public async override Task<IActionResult> Response()
        {
        
            switch (base.CommandName)
            {
                case "Menu":
                    return await SendCategoryList();

                case "ReturnToCatalogList":
                    return await SendCategoryList();

                case "ProductInCategory":
                     return await GetProduct();

                case "ViewAllProduct":
                    return await GetAllProduct();

                case "BackCategoryList":
                    return await SendCategoryList(base.MessageId);

                case "GetCategory":
                    return await GetCategory();

                case "NxtCatPage":
                    return await SelectCategoryPage();

                case "PrvCatPage":
                    return await SelectCategoryPage();

                default:
                    return null;
                    
            }
               
               
        }


        private async Task<IActionResult> SelectCategoryPage()
        {
            CategoryListMessage categoryListMessage = new CategoryListMessage(Argumetns[0]);

            var mess= categoryListMessage.BuildCategoryPage();

            if (mess != null)
                await EditMessage(mess);

            else
                await AnswerCallback("Данные отсутствуют");

            return OkResult;
        }

        private async Task<IActionResult> SendCategoryList(int MessageId=0)
        {
            CategoryListMessage categoryListMessage = new CategoryListMessage();

            var mess =categoryListMessage.BuildCategoryPage();

            if (mess != null)
                await SendMessage(mess, MessageId);

            else
                await AnswerCallback("Данные отсутствуют");

            return OkResult;

        }

        private async Task<IActionResult> GetProduct()
        {
            var message = ProductViewMsg.BuildMsg();

            //
            if (message.TextMessage!=null && await SendPhoto(message) != null)
                return base.OkResult;

            if(message.TextMessage==null && await AnswerCallback("Данные отсутсвуют"))
                return base.OkResult;
            

            else
                return base.NotFoundResult;
        }

        private async Task<IActionResult> GetAllProduct()
        {

            if (await EditMessage(ViewAllProductInCategoryMsg.BuildMsg()) != null)
                return base.OkResult;

            else
                return base.NotFoundResult;
        }

        /// <summary>
        /// Покзаать одним сообщение все товары в категории
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> GetCategory()
        {
            ViewAllProductInCategoryMsg = new  ViewAllProductInCategoryMessage(Argumetns[0]);

            if (await EditMessage(ViewAllProductInCategoryMsg.BuildMsg()) != null)
                return base.OkResult;

            else
                return base.NotFoundResult;
        }
    }
}
