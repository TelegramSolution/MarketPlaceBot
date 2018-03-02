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

namespace MyTelegramBot.Bot.AdminModule
{
    public class MoreSettingsBot : BotCore
    {
        public const string ModuleName = "MoreSet";

        public const string VkEditCmd = "VkEdit";

        public const string InstagramEditCmd = "InstagramEdit";

        public const string ChannelEditCmd = "ChannelEdit";

        public const string ChatEditCmd = "ChatEdit";

        public const string VkEditForceReply = "Vk.com";

        public const string InstagramForceReply = "Instagram.com";

        public const string ChatEditForceReply = "TelegramChat";

        public const string ChannelEditForceReply = "TelegramChannel";

        public const string WorkTimeEditorCmd = "WorkTimeEditor";

        public const string AboutEditCmd = "AboutEdit";

        public const string AboutEditorForceReply = "О нас";

        public const string EnablePaymentMethodCmd = "EnablePaymentMethod";

        public const string SettingsPaymentMethodCmd = "SettingsPaymentMethod";

        public const string MethodOfObtaitingCmd = "MethodOfObtaiting";

        public const string QiwiSettingsCmd = "QiwiSettings";

        public const string BackToMoreSettingsCmd = "BackToMoreSettings";

        public const string MoreSettingsCmd = "MoreSettings";

        public const string UpdDeliveryCmd = "UpdDelivery";

        public const string UpdPickUpCmd = "UpdPickUp";

        public const string DeliveryPriceCmd = "DeliveryPrice";

        public const string StartWorkTimeCmd = "StartWorkTime";

        public const string EndWorkTimeCmd = "EndWorkTime";

        public const string RemoveWorkTimeCmd = "RemoveWorkTime";

        public const string StartWorkTimeForceReplyCmd = "Время начала работы";

        public const string EndWorkTimeForceReplyCmd = "Время окончания работы";

        MoreSettingsMessage MoreSettingsMsg { get; set; }

        MethodOfObtaining MethodOfObtainingMsg { get; set; }

        WorkTimeMessage WorkTimeMsg { get; set; }

        public MoreSettingsBot(Update _update) : base(_update)
        {

        }

        protected override void Constructor()
        {
            MoreSettingsMsg = new MoreSettingsMessage();
        }

        public async override Task<IActionResult> Response()
        {
            if (IsOwner())
            {
                if (base.OriginalMessage == StartWorkTimeForceReplyCmd)
                    return await SaveTime(StartWorkTimeForceReplyCmd);

                if (base.OriginalMessage == EndWorkTimeForceReplyCmd)
                    return await SaveTime(EndWorkTimeForceReplyCmd);

                switch (base.CommandName)
                {
                    case MoreSettingsCmd:
                        return await SendMoreSettings(base.MessageId);

                    case MethodOfObtaitingCmd:
                        return await SendMethodOfObtaining();

                    case UpdDeliveryCmd:
                        return await UpdDelivery();

                    case UpdPickUpCmd:
                        return await UpdPickUp();

                    case BackToMoreSettingsCmd:
                        return await SendMoreSettings(base.MessageId);

                    case WorkTimeEditorCmd:
                        return await SendWorkTime(base.MessageId);

                    case StartWorkTimeCmd:
                        return await TimeInputSendForceReply(StartWorkTimeForceReplyCmd);

                    case EndWorkTimeCmd:
                        return await TimeInputSendForceReply(EndWorkTimeForceReplyCmd);

                    case RemoveWorkTimeCmd:
                        return await RemoveWorkTime();

                    default:
                        return null;
                }


            }



            else
                return null;
        }

        private async Task<IActionResult> SendMoreSettings(int MessageId = 0)
        {
            await SendMessage(MoreSettingsMsg.BuildMsg(), MessageId);

            return OkResult;
        }

        /// <summary>
        /// отпрваить сообщение со способами получения заказа
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendMethodOfObtaining()
        {
            MethodOfObtainingMsg = new MethodOfObtaining(base.BotInfo);

            await EditMessage(MethodOfObtainingMsg.BuildMsg());

            return OkResult;
        }

        private async Task<IActionResult> UpdDelivery()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            ///Доставка активна, пользователь пытается ее деактивировать, тогда не останется доступных способов оплаты
            if (BotInfo.Configuration.Pickup == false && BotInfo.Configuration.Delivery)
            {
                await AnswerCallback("Должен быть доступен хотя бы один способ получения заказа", true);

            }

            // Пользователь 
            if (BotInfo.Configuration.Pickup == true)
            {
                if (BotInfo.Configuration.Delivery)
                    BotInfo.Configuration.Delivery = false;

                else
                    BotInfo.Configuration.Delivery = true;

                db.Update<Configuration>(BotInfo.Configuration);

                int sabe = db.SaveChanges();

                db.Dispose();

                return await SendMethodOfObtaining();

            }

            else
                return OkResult;
        }

        private async Task<IActionResult> UpdPickUp()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            ///Доставка активна, пользователь пытается ее деактивировать, тогда не останется доступных способов оплаты
            if (BotInfo.Configuration.Delivery == false && BotInfo.Configuration.Pickup)
            {
                await AnswerCallback("Должен быть доступен хотя бы один способ получения заказа", true);

            }

            // Пользователь 
            if (BotInfo.Configuration.Delivery == true)
            {
                if (BotInfo.Configuration.Pickup)
                    BotInfo.Configuration.Pickup = false;

                else
                    BotInfo.Configuration.Pickup = true;

                db.Update<Configuration>(BotInfo.Configuration);

                int sabe = db.SaveChanges();

                db.Dispose();

                return await SendMethodOfObtaining();

            }

            else
                return OkResult;
        }

        private async Task<IActionResult> SendWorkTime(int MessageId = 0)
        {
            WorkTimeMsg = new WorkTimeMessage(base.BotInfo);

            await SendMessage(WorkTimeMsg.BuildMsg(), MessageId);

            return OkResult;
        }

        private async Task<IActionResult> TimeInputSendForceReply(string ForceRelplyMsg)
        {

            await SendMessage(new BotMessage { TextMessage = "Введите время. Например 9:00" });
            return await ForceReplyBuilder(ForceRelplyMsg);
            
        }

        private async Task<IActionResult> SaveTime(string OriginalMessage)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {

                TimeSpan timeSpan = Convert.ToDateTime(ReplyToMessageText).TimeOfDay;


                if (OriginalMessage == StartWorkTimeForceReplyCmd)
                {
                    BotInfo.Configuration.StartTime = timeSpan;

                    db.Update<Configuration>(BotInfo.Configuration);

                    db.SaveChanges();

                    return await SendWorkTime();

                }

                if (OriginalMessage == EndWorkTimeForceReplyCmd)
                {
                    BotInfo.Configuration.EndTime = timeSpan;

                    db.Update<Configuration>(BotInfo.Configuration);

                    db.SaveChanges();

                    return await SendWorkTime();

                }

                else
                    return OkResult;
            }

            catch
            {
                await SendMessage(new BotMessage { TextMessage = "Ошибка! Неверный формат данных" });
                return await TimeInputSendForceReply(OriginalMessage);
            }

            finally
            {
                db.Dispose();
            }
        }

        private async Task<IActionResult> RemoveWorkTime()
        {
            using(MarketBotDbContext db=new MarketBotDbContext())
            {
                BotInfo.Configuration.StartTime = null;
                BotInfo.Configuration.EndTime = null;
                db.Update<Configuration>(BotInfo.Configuration);
                return await SendWorkTime(base.MessageId);
            }
        }
    }
}
