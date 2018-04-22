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
using ManagementBots.Bot;
using ManagementBots.Bot.Core;

namespace ManagementBots.Messages
{
    /// <summary>
    /// Главное меню бота
    /// </summary>
    public class MainMenuBotMessage:BotMessage
    {
        private InlineKeyboardCallbackButton MyBotnsBtn { get; set; }

        private InlineKeyboardCallbackButton NewConnectBotBtn { get; set; }

        private InlineKeyboardButton VideoDemoBtn { get; set; }

        private InlineKeyboardCallbackButton ViewAllBotBtn { get; set; }

        private InlineKeyboardCallbackButton HelpBtn { get; set; }

        public override BotMessage BuildMsg()
        {
            MyBotnsBtn = base.BuildInlineBtn("Мои боты", base.BuildCallData("MyBots", "Main"),base.MobileEmodji);

            NewConnectBotBtn = base.BuildInlineBtn("Подключить бота", base.BuildCallData(ConnectBot.RequestBotTokenCmd, ConnectBot.ModuleName),base.SenderEmodji);

            VideoDemoBtn = InlineKeyboardUrlButton.WithUrl("Видеодемонстарция", "https://www.youtube.com/");

            ViewAllBotBtn = base.BuildInlineBtn("Подключенные боты", base.BuildCallData("AllBots", "Main"),base.NoteBookEmodji);

            HelpBtn = base.BuildInlineBtn("Служба поддержки", base.BuildCallData("Help", "Main"));



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
                    MyBotnsBtn,
                },
                new[]
                {
                    NewConnectBotBtn
                },
                new[]
                {
                    VideoDemoBtn
                },
                new[]
                {
                    HelpBtn
                }
                });


        }
    }
}
