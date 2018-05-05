using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.IO;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot.Core;
using MyTelegramBot.BusinessLayer;
using MyTelegramBot.Export;

namespace MyTelegramBot.Bot.AdminModule
{
    public class ReportsBot:BotCore
    {

        public const string ModuleName = "Reports";

        private ExcelExport ExcelExport { get; set; }

        public const string OrderExportCallBack = "OrderExport";

        public const string OrderExportCommand = "/export1";


        public const string FollowerExportCallBack = "FollowerExport";

        public const string FollowerExportCommand = "/export2";


        public const string FeedBackExportCallBack = "FeedBackExport";

        public const string FeedBackExportCommand = "/export3";


        public const string StockHistoryExportCallBack = "StockHistoryExport";

        public const string StockHistoryExportCommand = "/export4";


        public const string HelpDeskExportCallBack = "HelpDeskExport";

        public const string HelpDeskExportCommand = "/export5";


        public const string ProductExportCallBack = "ProductExport";

        public const string ProductQuestionCallBack = "ProductQuestion";


        /// <summary>
        /// один экспорт в  минут. Чаще нельзя
        /// </summary>
        private const int Minute = 1;



        public ReportsBot (Update update) : base(update)
        {

        }

        protected override void Initializer()
        {

        }

        public async override Task<IActionResult> Response()
        {
            if (IsOwner())
            {
                switch (CommandName)
                {
                    case OrderExportCallBack:
                        return await SendOrderExport();

                    case OrderExportCommand:
                        return await SendOrderExport();

                    case FollowerExportCallBack:
                        return await SendFollowerExport();

                    case FollowerExportCommand:
                        return await SendFollowerExport();

                    case FeedBackExportCallBack:
                        return await SendFeedBackExport();

                    case FeedBackExportCommand:
                        return await SendFeedBackExport();

                    case StockHistoryExportCallBack:
                        return await SendStockHistoryExport();

                    case StockHistoryExportCommand:
                        return await SendStockHistoryExport();

                    case HelpDeskExportCallBack:
                        return await SendHelpDeskExport();

                    case HelpDeskExportCommand:
                        return await SendHelpDeskExport();

                    case ProductExportCallBack:
                        return await SendProductExport();

                    case ProductQuestionCallBack:
                        return await SendProductQuestionExport();

                    default:
                        return null;
                }
            }

            return null;
        }

        /// <summary>
        /// экспорт всех данных по заказам
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendOrderExport()
        {
            return await SendExportFile(new OrderExport(), "Заказы.xlsx", "Список всех отзывов");

        }

        private async Task<IActionResult> SendProductExport()
        {
            return await SendExportFile(new ProductExport(), "Товары.xlsx", "Список всех товаров");

        }

        private async Task<IActionResult> SendHelpDeskExport()
        {
            return await SendExportFile(new HelpDeskExport(), "Заявки.xlsx", "Список всех заявок");

        }


        private async Task<IActionResult> SendFeedBackExport()
        {
            return await SendExportFile(new FeedBackExport(), "Отзывы.xlsx", "Список всех отзывов");
        }

        private async Task<IActionResult> SendFollowerExport()
        {
            return await SendExportFile(new FollowerExport(), "Пользователи.xlsx", "Список всех пользователей");

        }

        private async Task<IActionResult> SendStockHistoryExport()
        {

            return await SendExportFile(new StockHistoryExport(), "Остатки.xlsx", "История изменения остатков");

        }

        private async Task<IActionResult> SendProductQuestionExport()
        {

            return await SendExportFile(new ProductQuestionExport(), "Вопросы.xlsx", "Вопросы пользователей о товарах");

        }

        /// <summary>
        /// Сформировать файл с выгрузкой и отправить
        /// </summary>
        /// <param name="excelExport"></param>
        /// <param name="FileName"></param>
        /// <param name="Caption"></param>
        /// <returns></returns>
        private async Task<IActionResult> SendExportFile(ExcelExport excelExport, string FileName, string Caption="")
        {
            
            if (CheckTime())
            {
                ExcelExport = excelExport;

                base.SendAction();

                RequestLogFunction.Insert(FollowerId, DateTime.Now);

                await base.SendDocument(new FileToSend { Content = ExcelExport.BuildReport(), Filename = FileName }, Caption);

                return OkResult;
            }

            else
            {
                if (Update.CallbackQuery == null)
                    await SendMessage(new BotMessage { TextMessage = "Не более одного запроса в минуту" });

                else
                    await AnswerCallback("Не более одного запроса в минуту", true);

                return OkResult;
            }
        }

        /// <summary>
        /// Проверяем время последнего формирования отчета и если последний отчет был сформирован менее 5 минут назад, то Возрващаем False.
        /// Значит отчет сейчас сформировать нельзя.
        /// </summary>
        /// <returns></returns>
        private bool CheckTime()
        {
            var lastRecord = RequestLogFunction.GetLasRecordFromFollower(FollowerId);

            if (lastRecord == null)
                return true;

            if (lastRecord != null && UnixTimeNow() - ConvertToUnixTime(lastRecord.DateAdd.Value) > Minute * 60)
                return true;


            else
                return false;
        }

        private long UnixTimeNow()
        {
            var timeSpan = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }

        private long ConvertToUnixTime(DateTime dateTime)
        {
            var timeSpan = (dateTime - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }
    }
}
