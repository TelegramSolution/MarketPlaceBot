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

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Главное меню бота
    /// </summary>
    public class MainMenuBotMessage:Bot.BotMessage
    {
        private InlineKeyboardCallbackButton MenuBtn { get; set; }

        private InlineKeyboardCallbackButton ContactBtn { get; set; }

        private InlineKeyboardCallbackButton ViewBasketBtn { get; set; }

        private InlineKeyboardCallbackButton MyOrdersBtn { get; set; }

        private InlineKeyboardCallbackButton HelpBtn { get; set; }

        private InlineKeyboardCallbackButton OpenSourceBtn { get; set; }

        private InlineKeyboardCallbackButton MethodOfObtainingInfoBtn { get; set; }

        public override BotMessage BuildMsg()
        {
            MenuBtn = new InlineKeyboardCallbackButton("Каталог товаров"+ " \ud83d\udcc3", BuildCallData("Menu",Bot.CategoryBot.ModuleName));
            ContactBtn = new InlineKeyboardCallbackButton("О нас" + " \u260e\ufe0f", BuildCallData("Contact", Bot.MainMenuBot.ModuleName));
            ViewBasketBtn = new InlineKeyboardCallbackButton("Корзина" + " \ud83d\uded2", BuildCallData(Bot.BasketBot.ViewBasketCmd,Bot.BasketBot.ModuleName));
            MyOrdersBtn = new InlineKeyboardCallbackButton("Мои заказы"+ " \ud83d\udce6", BuildCallData(Bot.OrderBot.MyOrdersListCmd,Bot.OrderBot.ModuleName));
            HelpBtn = new InlineKeyboardCallbackButton("Техническая поддержка", BuildCallData("Help", Bot.HelpDeskBot.ModuleName));
            OpenSourceBtn = new InlineKeyboardCallbackButton("Исходный код", BuildCallData("OpenSource", Bot.MainMenuBot.ModuleName));
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
                            MenuBtn
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
                        OpenSourceBtn
                        }
                 });


        }
    }
}
