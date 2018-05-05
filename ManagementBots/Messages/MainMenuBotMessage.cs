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

       // private InlineKeyboardCallbackButton ViewAllBotBtn { get; set; }

        private InlineKeyboardButton ExampleBotBtn { get; set; }

        private InlineKeyboardButton HelpBtn { get; set; }

        private InlineKeyboardButton AboutBtn { get; set; }

        public override BotMessage BuildMsg()
        {
            MyBotnsBtn = base.BuildInlineBtn("Мои боты", base.BuildCallData(ConnectBot.MyBotsCmd, ConnectBot.ModuleName),base.MobileEmodji);

            NewConnectBotBtn = base.BuildInlineBtn("Подключить бота", base.BuildCallData(ConnectBot.RequestBotTokenCmd, ConnectBot.ModuleName),base.SenderEmodji);

            VideoDemoBtn = InlineKeyboardUrlButton.WithUrl("Видеодемонстарция", "https://www.youtube.com/watch?v=fYtglYPh-wM");

            //ViewAllBotBtn = base.BuildInlineBtn("Подключенные боты", base.BuildCallData("AllBots", "Main"),base.NoteBookEmodji);

            HelpBtn = InlineKeyboardUrlButton.WithUrl("Служба поддержки", "https://t.me/tgsolution");

            ExampleBotBtn = InlineKeyboardButton.WithUrl("Пример бота", "https://t.me/testmcdonaldsbot");

            AboutBtn= base.BuildInlineBtn("Что это ?", base.BuildCallData(ConnectBot.AboutCmd, ConnectBot.ModuleName), base.SenderEmodji);

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
                    AboutBtn
                },
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
                    ExampleBotBtn
                },
                new[]
                {
                    HelpBtn
                }
                });


        }
    }
}
