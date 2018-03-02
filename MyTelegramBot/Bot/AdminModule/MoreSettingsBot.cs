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
    public class MoreSettingsBot:BotCore
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



        MoreSettingsMessage MoreSettingsMsg { get; set; }

        MethodOfObtaining  MethodOfObtainingMsg { get; set; }

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
            if(BotInfo!=null && BotInfo.Configuration!=null && BotInfo.Configuration.Pickup==false && BotInfo.Configuration.Delivery)
            {
                await AnswerCallback("Должен быть доступен хотя бы один способ получения заказа",true);

            }

            // Пользователь 
            if (BotInfo != null && BotInfo.Configuration != null && BotInfo.Configuration.Pickup == true)
            {
                if (BotInfo.Configuration.Delivery)
                    BotInfo.Configuration.Delivery = false;

                else
                    BotInfo.Configuration.Delivery = true;

                db.Update<Configuration>(BotInfo.Configuration);

                int sabe = db.SaveChanges();

                return await SendMethodOfObtaining();

            }

            else
                return OkResult;
        }

        private async Task<IActionResult> UpdPickUp()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            ///Доставка активна, пользователь пытается ее деактивировать, тогда не останется доступных способов оплаты
            if (BotInfo != null && BotInfo.Configuration != null && BotInfo.Configuration.Delivery == false && BotInfo.Configuration.Pickup)
            {
                await AnswerCallback("Должен быть доступен хотя бы один способ получения заказа", true);

            }

            // Пользователь 
            if (BotInfo != null && BotInfo.Configuration != null && BotInfo.Configuration.Delivery == true)
            {
                if (BotInfo.Configuration.Pickup)
                    BotInfo.Configuration.Pickup = false;

                else
                    BotInfo.Configuration.Pickup = true;

                db.Update<Configuration>(BotInfo.Configuration);

                int sabe = db.SaveChanges();

                return await SendMethodOfObtaining();

            }

            else
                return OkResult;
        }
    }
}
