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


        private int ProductId { get; set; }

        public BasketBot(Update _update) : base(_update)
        {
           
        }

        protected override void Initializer()
        {
            try
            {

                if (base.Argumetns.Count > 0)
                    this.ProductId = Argumetns[0];
                
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
        /// <param name="MessageId"></param>
        /// <returns></returns>
        private async Task<IActionResult> ViewBasket(int MessageId=0)
        {
            BotMessage = new ViewBasketMessage(FollowerId, BotInfo.Id);

            var mess= BotMessage.BuildMsg();

            if (mess != null && mess.TextMessage != null)
                await SendMessage(BotMessage, MessageId);

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
            BotMessage = new BasketPositionListMessage(FollowerId);
            await EditMessage(BotMessage.BuildMsg());
            return base.OkResult;

        }

        /// <summary>
        /// Отправить редактор позиции товара
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendEditorPositionMsg()
        {
            BotMessage = new BasketPositionCallBackAnswerMessage(FollowerId, ProductId);

            await EditMessage(BotMessage.BuildMsg());

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
