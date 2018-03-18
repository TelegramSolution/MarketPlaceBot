using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;
using MyTelegramBot.Bot.AdminModule;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// сообщение с кнопками экспорта данных
    /// </summary>
    public class ExportViewerMessage:BotMessage
    {
        private InlineKeyboardCallbackButton OrderExportBtn { get; set; }

        private InlineKeyboardCallbackButton FollowerExportBtn { get; set; }

        private InlineKeyboardCallbackButton FeedBackExportBtn { get; set; }

        private InlineKeyboardCallbackButton StockHistoryExportBtn { get; set; }

        public override BotMessage BuildMsg()
        {
            base.TextMessage = "Экспорт данных в формате .xlsx";

            OrderExportBtn = BuildInlineBtn("Список заказов", BuildCallData(ReportsBot.OrderExportCallBack, ReportsBot.ModuleName));

            FollowerExportBtn = BuildInlineBtn("Список пользователей", BuildCallData(ReportsBot.FollowerExportCallBack, ReportsBot.ModuleName));

            FeedBackExportBtn = BuildInlineBtn("Отзывы", BuildCallData(ReportsBot.FeedBackExportCallBack, ReportsBot.ModuleName));

            StockHistoryExportBtn = BuildInlineBtn("Остатки", BuildCallData(ReportsBot.StockHistoryExportCallBack, ReportsBot.ModuleName));

            BackBtn = BackToAdminPanelBtn();

            base.MessageReplyMarkup = new InlineKeyboardMarkup(
            new[]{
                new[]
                        {
                            OrderExportBtn
                        },
                new[]
                        {
                            FollowerExportBtn
                        },
                new[]
                        {
                            FeedBackExportBtn
                        },
                new[]
                        {
                            StockHistoryExportBtn
                        },
                new[]
                        {
                            BackBtn
                        },

            });

            return this;
        }

    }
}
