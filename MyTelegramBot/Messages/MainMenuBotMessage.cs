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
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Главное меню бота
    /// </summary>
    public class MainMenuBotMessage:BotMessage
    {
        private InlineKeyboardCallbackButton MenuBtn { get; set; }

        private InlineKeyboardCallbackButton ContactBtn { get; set; }

        private InlineKeyboardCallbackButton ViewBasketBtn { get; set; }

        private InlineKeyboardButton MyOrdersBtn { get; set; }

        private InlineKeyboardCallbackButton HelpBtn { get; set; }

        private InlineKeyboardButton OpenSourceBtn { get; set; }

        private InlineKeyboardCallbackButton MethodOfObtainingInfoBtn { get; set; }

        private InlineKeyboardButton PhotoCatalogBtn { get; set; }

        private InlineKeyboardButton SearchProductBtn { get; set; }

        public override BotMessage BuildMsg()
        {
            MenuBtn = new InlineKeyboardCallbackButton("Каталог (текстовая версия)"+ " \ud83d\udcc3", BuildCallData("Menu",Bot.CategoryBot.ModuleName));
            ContactBtn = new InlineKeyboardCallbackButton("О нас" + " \u260e\ufe0f", BuildCallData("Contact", Bot.MainMenuBot.ModuleName));
            ViewBasketBtn = new InlineKeyboardCallbackButton("Корзина" + " \ud83d\uded2", BuildCallData(Bot.BasketBot.ViewBasketCmd,Bot.BasketBot.ModuleName));
            MyOrdersBtn = InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Мои заказы", InlineFind.MyOrders + "|");
            HelpBtn = new InlineKeyboardCallbackButton("Служба поддержки", BuildCallData("Help", Bot.HelpDeskBot.ModuleName));
            OpenSourceBtn =  InlineKeyboardButton.WithUrl("Исходный код", "https://github.com/TelegramSolution/MarketPlaceBot");
            PhotoCatalogBtn = InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Фотокаталог"+base.PictureEmodji, InlineFind.PhotoCatalog + "|");
            SearchProductBtn = InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Поиск"+base.SearchEmodji, InlineFind.SearchProduct + "|");

            SetInlineKeyBoard();
            base.TextMessage = "Выберите действие";
            return this;
        }

        private void SetInlineKeyBoard()
        {

            base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            PhotoCatalogBtn
                        },
                new[]
                        {
                            SearchProductBtn
                        },
                new[]
                        {
                            ContactBtn,HelpBtn
                        },

                new[]
                        {
                            ViewBasketBtn,MyOrdersBtn
                        },

                new[]
                        {
                                MenuBtn,
                        },
                    //new [] 
                    //    {
                    //            OpenSourceBtn
                    //    },

                 });


        }
    }
}
