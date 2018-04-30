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

        private InlineKeyboardCallbackButton HelpDeskExportBtn { get; set; }

        private InlineKeyboardCallbackButton ProductExportBtn { get; set; }

        private InlineKeyboardCallbackButton ProductQuestionExportBtn { get; set; }

        public override BotMessage BuildMsg()
        {
            base.TextMessage = "Экспорт данных в формате .xlsx";

            OrderExportBtn = BuildInlineBtn("Заказы", BuildCallData(ReportsBot.OrderExportCallBack, ReportsBot.ModuleName),base.OpenedBookEmodji);

            FollowerExportBtn = BuildInlineBtn("Пользователи", BuildCallData(ReportsBot.FollowerExportCallBack, ReportsBot.ModuleName), base.OpenedBookEmodji);

            FeedBackExportBtn = BuildInlineBtn("Отзывы", BuildCallData(ReportsBot.FeedBackExportCallBack, ReportsBot.ModuleName), base.OpenedBookEmodji);

            StockHistoryExportBtn = BuildInlineBtn("Остатки", BuildCallData(ReportsBot.StockHistoryExportCallBack, ReportsBot.ModuleName), base.OpenedBookEmodji);

            HelpDeskExportBtn= BuildInlineBtn("Заявки", BuildCallData(ReportsBot.HelpDeskExportCallBack, ReportsBot.ModuleName), base.OpenedBookEmodji);

            ProductExportBtn= BuildInlineBtn("Товары", BuildCallData(ReportsBot.ProductExportCallBack, ReportsBot.ModuleName), base.OpenedBookEmodji);

            ProductQuestionExportBtn = BuildInlineBtn("Вопросы по товарам", BuildCallData(ReportsBot.ProductQuestionCallBack, ReportsBot.ModuleName), base.OpenedBookEmodji);

            BackBtn = BackToAdminPanelBtn();

            base.MessageReplyMarkup = new InlineKeyboardMarkup(
            new[]{
                new[]
                        {
                            OrderExportBtn,HelpDeskExportBtn
                        },
                new[]
                        {
                            FollowerExportBtn,ProductExportBtn
                        },
                new[]
                        {
                            FeedBackExportBtn,StockHistoryExportBtn
                        },
                new[]
                        {
                            ProductQuestionExportBtn
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
