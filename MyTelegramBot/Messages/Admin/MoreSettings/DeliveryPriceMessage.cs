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


namespace MyTelegramBot.Messages.Admin
{
    public class DeliveryPriceMessage:BotMessage
    {
        private InlineKeyboardCallbackButton PriceEditBtn { get; set; }

        private InlineKeyboardCallbackButton FreeRulesEditBtn { get; set; }

        private InlineKeyboardCallbackButton RemoveBtn { get; set; }

        private BotInfo BotInfo { get; set; }

        public DeliveryPriceMessage(BotInfo bot)
        {
            this.BotInfo = bot;
        }

        public override BotMessage BuildMsg()
        {

            if (BotInfo != null && BotInfo.Configuration != null)
            {
                base.TextMessage = "Стоимость доставки: " + BotInfo.Configuration.ShipPrice.ToString() + NewLine() +
                    "Бесплатная доставка от:" + BotInfo.Configuration.FreeShipPrice.ToString();

                PriceEditBtn = BuildInlineBtn("Стоимость доставки", BuildCallData(MoreSettingsBot.DeliveryPriceEditCmd, MoreSettingsBot.ModuleName), base.CashEmodji);

                FreeRulesEditBtn = BuildInlineBtn("Бесплатная доставка от", BuildCallData(MoreSettingsBot.FreeDeliveryRulesEditCmd, MoreSettingsBot.ModuleName), base.PenEmodji);

                RemoveBtn = BuildInlineBtn("Очистить", BuildCallData(MoreSettingsBot.RemoveDeliveryPriceCmd, MoreSettingsBot.ModuleName), base.CrossEmodji);

                BackBtn = BuildInlineBtn("Назад", BuildCallData(MoreSettingsBot.BackToMoreSettingsCmd, MoreSettingsBot.ModuleName), base.Previuos2Emodji, false);

                base.MessageReplyMarkup = new InlineKeyboardMarkup(new[] {
                    new[]
                    {
                        PriceEditBtn
                    },
                    new[]
                    {
                        FreeRulesEditBtn
                    },
                    new[]
                    {
                        RemoveBtn
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
