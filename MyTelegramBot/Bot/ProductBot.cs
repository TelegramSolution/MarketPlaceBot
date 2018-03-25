using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyTelegramBot.Messages;
using MyTelegramBot.Bot.Core;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;
using System.Web;
using Telegram.Bot.Types.InlineKeyboardButtons;

namespace MyTelegramBot.Bot
{
    public class ProductBot : BotCore
    {
        public const string ModuleName = "Prod";

        private int ProductId { get; set; }

        private ProductViewMessage ProductViewMsg { get; set; }


        /// <summary>
        /// Сообщение со всеми фотографиями товара
        /// </summary>
        private ProductAllPhotoMessage ProductAllPhotoMsg { get; set; }

        private ViewAllProductInCategoryMessage ViewAllProductInCategoryMsg { get; set; }


        public const string GetProductCmd = "GetProduct";

        public const string AddToBasketCmd = "AddToBasket";

        public const string RemoveFromBasketCmd = "RemoveFromBasket";

        public const string MoreInfoProductCmd = "MoreInfoProduct";

        public const string ProductCmd = "/item";

        public const string ViewAllPhotoProductCmd = "ViewAllPhotoProduct";

        public const string CmdViewFeedBack = "ViewFeedBack";

        public const string CmdAddFeedBackProduct = "AddFbToPrdct";

        public const string CmdProductPage = "ProductPage";


        public ProductBot(Update _update) : base(_update)
        {

        }

        protected override void Initializer()
        {
            try
            {
                if (base.Argumetns.Count > 0)
                {
                    ProductId = Argumetns[0];
                    ProductViewMsg = new ProductViewMessage(this.ProductId, BotInfo.Id);
                }

            }

            catch
            {

            }
        }

        public async override Task<IActionResult> Response()
        {

            switch (base.CommandName)
            {
                //Пользователь нажал на кнопку вперед или назад при листнинге товаров
                case GetProductCmd:
                    return await GetProduct();

                //Пользователь нажал на +, добавил товар в корзину в кол-во 1 шт.
                case AddToBasketCmd:
                    return await AddToBasket();

                //Пользователь нажалн на-, удалил товар из корзины в кол-во 1 шт.
                case RemoveFromBasketCmd:
                    return await RemoveFromBasket();

                //Пользователь нажал "Подробнее" 
                case MoreInfoProductCmd:
                    return await MoreInfoProduct();

                case ViewAllPhotoProductCmd:
                    return await SendAllProductPhoto();

                case CmdProductPage:
                    return await SendProductPage(Argumetns[0], Argumetns[1]);

                case CmdViewFeedBack:
                    return await SendFeedBack();
            }



            //команда /item
            if (base.CommandName.Contains(ProductCmd))
                return await GetProductCommand();

            else
                return null;
        }

        /// <summary>
        /// Отправить стр. с товарами
        /// </summary>
        /// <param name="CategoryId">id категории</param>
        /// <param name="PageNumber">номер стр.</param>
        /// <returns></returns>
        private async Task<IActionResult> SendProductPage(int CategoryId, int PageNumber = 1)
        {
            BotMessage = new ViewAllProductInCategoryMessage(CategoryId, PageNumber);

            await EditMessage(BotMessage.BuildMsg());

            return OkResult;
        }

        private async Task<IActionResult> SendFeedBack()
        {
            if (Argumetns.Count == 1)
            {
                BotMessage = new ViewProductFeedBackMessage(Argumetns[0]);
                var mess = BotMessage.BuildMsg();

                if (mess != null)
                    await SendMessage(mess);

            }
            if (Argumetns.Count == 2) // перелистывание отзывов в одном сообщении.
            {
                BotMessage = new ViewProductFeedBackMessage(Argumetns[0], Argumetns[1]);
                var mess = BotMessage.BuildMsg();

                if (mess != null)
                    await EditMessage(mess);
            }


            else
                await AnswerCallback("Отзывы отсутствуют", true);

            return OkResult;
        }

        private async Task<IActionResult> SendAllProductPhoto()
        {
            try
            {
                ProductAllPhotoMsg = new ProductAllPhotoMessage(this.ProductId, BotInfo.Id);

                ProductAllPhotoMsg.BuildMessage();

                //отправляем альбом с фотографиями

                if (ProductAllPhotoMsg.MediaGroupPhoto != null && ProductAllPhotoMsg.MediaGroupPhoto.ListMedia != null && ProductAllPhotoMsg.MediaGroupPhoto.ListMedia.Count > 0)
                {
                    await base.SendMediaPhotoGroup(ProductAllPhotoMsg.MediaGroupPhoto);

                    //следом отправляем кнопку назад
                    await base.SendMessage(ProductAllPhotoMsg);
                }

                else
                {
                    await base.AnswerCallback("Фотографии отсутствуют");
                }
                return OkResult;
            }
            catch
            {
                return OkResult;
            }

        }


        /// <summary>
        /// Добавить позицию в корзину после нажатия кнопки с плюсом (+)
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> AddToBasket()
        {
           var basket_total_amount= BusinessLayer.BasketFunction.ProductBasketCount(FollowerId, ProductId, BotInfo.Id);

            var balance= BusinessLayer.StockFunction.CurrentBalance(ProductId);

            if (basket_total_amount + 1 <= balance)
            {
                BusinessLayer.BasketFunction.AddPositionToBasker(FollowerId, ProductId, BotInfo.Id);
                await AnswerCallback("Итого:"+(basket_total_amount+1).ToString(), false);
            }

           else
            {
                await AnswerCallback("В наличии только:" + balance.ToString(), true);
            }

            return OkResult;
        }

        /// <summary>
        /// Срабатывает по команде /product[id] напримиер /product12
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> GetProductCommand()
        {
            try
            {
                int id = Convert.ToInt32(base.CommandName.Substring(ProductCmd.Length));

                ProductViewMsg = new ProductViewMessage(id, BotInfo.Id);
                return await GetProduct();
            }

            catch
            {
                return base.OkResult;
            }
        }

        private async Task<IActionResult> GetProduct()
        {
            if (ProductViewMsg != null)
            {
                var mess = ProductViewMsg.BuildMsg();
                if (mess != null && await SendPhoto(mess) != null)
                    return base.OkResult;
                else
                    return base.OkResult;
            }
            else
                return base.OkResult;

        }

        private async Task<IActionResult> RemoveFromBasket()
        {
          int basket_total= BusinessLayer.BasketFunction.RemovePositionFromBasket(FollowerId, ProductId, BotInfo.Id);

           await AnswerCallback("Итого:" + basket_total.ToString(), false);

          return base.OkResult;

        }

        private async Task<IActionResult> MoreInfoProduct()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {

                var product = db.Product.Find(ProductId);
                await SendUrl("Вернуться к товару /product" + product.Id + BotMessage.NewLine() + BotMessage.NewLine() + product.TelegraphUrl);
                return OkResult;

            }
        }
    }
}
