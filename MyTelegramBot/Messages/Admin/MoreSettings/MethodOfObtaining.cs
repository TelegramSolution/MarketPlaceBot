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
    /// Управалние способами получения заказа Доставка / самовывоз
    /// </summary>
    public class MethodOfObtaining:BotMessage
    {
        private InlineKeyboardCallbackButton PickupPointBtn { get; set; }

        private InlineKeyboardCallbackButton DeliveryBtn { get; set; }

        private MarketBotDbContext db { get; set; }

        private string BotName { get; set; }

        private BotInfo BotInfo { get; set; }


        public MethodOfObtaining(BotInfo BotInfo)
        {
            this.BotInfo = BotInfo;
        }

        public override BotMessage BuildMsg()
        {

            if (BotInfo != null && BotInfo.Configuration != null)
            {
                base.TextMessage = "Способы получения заказа";

                if (BotInfo.Configuration.Pickup)
                    PickupPointBtn = BuildInlineBtn("Самовывоз", BuildCallData(MoreSettingsBot.UpdPickUpCmd, MoreSettingsBot.ModuleName), base.CheckEmodji);

                else
                    PickupPointBtn = BuildInlineBtn("Самовывоз", BuildCallData(MoreSettingsBot.UpdPickUpCmd, MoreSettingsBot.ModuleName), base.UnCheckEmodji);

                if (BotInfo.Configuration.Delivery)
                    DeliveryBtn = BuildInlineBtn("Доставка", BuildCallData(MoreSettingsBot.UpdDeliveryCmd, MoreSettingsBot.ModuleName), base.CheckEmodji);

                else
                    DeliveryBtn = BuildInlineBtn("Доставка", BuildCallData(MoreSettingsBot.UpdDeliveryCmd, MoreSettingsBot.ModuleName), base.UnCheckEmodji);

                BackBtn = BuildInlineBtn("Назад", BuildCallData(MoreSettingsBot.BackToMoreSettingsCmd, MoreSettingsBot.ModuleName));

                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            DeliveryBtn
                        },
                new[]
                        {
                            PickupPointBtn
                        },

                new[]
                        {
                        BackBtn
                        }
                });

                return this;
            }

            else
                return null;
           


        }
    }
}
