using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyTelegramBot.Messages;
using MyTelegramBot.Bot.Core;
using MyTelegramBot.BusinessLayer;

namespace MyTelegramBot.Bot
{
    public class BasketBot:BotCore
    {
        public const string ModuleName = "Bskt";

        public const string ViewBasketCmd = "ViewBasket";

        public const string BasketEditCmd = "EditBasket";

        public const string BackToBasketCmd = "BackToBasket";

        public const string AddProductToBasketCmd = "AddProductToBasket";

        public const string RemoveProductFromBasketCmd = "RemoveProductFromBasket";

        public const string BackToBasketPositionCmd = "BackToBasketPosition";

        public const string EditBasketProductCmd = "EditBasketProduct";

        public const string ClearBasketCmd = "ClearBasket";

        ViewBasketMessage ViewBasketMsg { get; set; }

        BasketPositionListMessage  BasketPositionListMsg { get; set; }

        BasketPositionCallBackAnswerMessage BasketPositionEditMsg { get; set; }

        private int ProductId { get; set; }

        public BasketBot(Update _update) : base(_update)
        {
           
        }

        protected override void Initializer()
        {
            try
            {
                ViewBasketMsg = new ViewBasketMessage(base.FollowerId,BotInfo.Id);
                BasketPositionListMsg = new BasketPositionListMessage(base.FollowerId);


                if (base.Argumetns.Count > 0)
                {
                    this.ProductId = Argumetns[0];
                    BasketPositionEditMsg = new BasketPositionCallBackAnswerMessage(base.FollowerId, this.ProductId);
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
                case ViewBasketCmd:
                    return await ViewBasket();

                case ClearBasketCmd:
                    return await ClearBasket();

                case BasketEditCmd:
                    return await SendPositionList();

                case BackToBasketCmd:
                    return await ViewBasket(base.MessageId);

                case EditBasketProductCmd:
                    return await SendEditorPositionMsg();

                case AddProductToBasketCmd:
                    return await AddToBasket();

                case RemoveProductFromBasketCmd:
                    return await RemoveFromBasket();

                case BackToBasketPositionCmd:
                    return await SendPositionList();

                default:
                    return null;
            }             

               
        }

        /// <summary>
        /// отправить сообщение с содержанием корзины
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        private async Task<IActionResult> ViewBasket(int Message=0)
        {
            ViewBasketMsg.BuildMsg();
            var mess= ViewBasketMsg.BuildMsg();

            if (mess != null && mess.TextMessage != null)
                await SendMessage(ViewBasketMsg, Message);

            else
                await AnswerCallback("Корзина пуста!", true);

            return OkResult;
        }

        /// <summary>
        /// очистить корзину
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> ClearBasket()
        {
            if (BasketFunction.ClearBasket(FollowerId, BotInfo.Id) > 0)
                await EditMessage(new BotMessage { TextMessage = "Корзина пуста" });

            return base.OkResult;
        }

        /// <summary>
        /// отправить сообщение с позициями в корзине в виде кнопок
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendPositionList()
        {
            if (await EditMessage(BasketPositionListMsg.BuildMsg()) != null)
                return base.OkResult;

            else
                return base.OkResult;

        }

        /// <summary>
        /// Отправить редактор позиции товара
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendEditorPositionMsg()
        {
            if (await EditMessage(BasketPositionEditMsg.BuildMsg()) != null)
                return base.OkResult;

            else
                return base.OkResult;

        }

        /// <summary>
        /// Добавить одну позицию товара в корзину
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> AddToBasket()
        {

                var CurrentStock = StockFunction.CurrentBalance(ProductId);

                int CountInBasket = BasketFunction.ProductBasketCount(FollowerId, ProductId, BotInfo.Id);

                if (CurrentStock >= CountInBasket + 1)
                {

                    BasketFunction.AddPositionToBasker(FollowerId, ProductId, BotInfo.Id);
                    return await SendEditorPositionMsg();
                }

                if (CurrentStock < CountInBasket + 1)
                {
                    await AnswerCallback("В наличии только " + CurrentStock.ToString(), true);
                    return OkResult;
                }

                    return OkResult;
        }

        /// <summary>
        /// Удалить одну позицию товара из корзины
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> RemoveFromBasket()
        {
             BasketFunction.RemovePositionFromBasket(FollowerId, ProductId, BotInfo.Id);
             return await SendEditorPositionMsg();    

        }
    }
}
