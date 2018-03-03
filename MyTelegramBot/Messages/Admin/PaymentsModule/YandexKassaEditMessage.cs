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

namespace MyTelegramBot.Messages.Admin.PaymentsModule
{
    public class YandexKassaEditMessage:BotMessage
    {
        private InlineKeyboardCallbackButton RemoveBtn { get; set; }

        private InlineKeyboardCallbackButton TokenEditBtn { get; set; }

        private InlineKeyboardCallbackButton ShopIdEditBtn { get; set; }

        private InlineKeyboardCallbackButton AddBtn { get; set; }

        private PaymentTypeConfig YandexKassaConfig { get; set; }

        private MarketBotDbContext db { get; set; }

        public override BotMessage BuildMsg()
        {

            db = new MarketBotDbContext();

            YandexKassaConfig = db.PaymentTypeConfig.Where(p => p.PaymentId == ConstantVariable.PaymentTypeVariable.DebitCardForYandexKassa).FirstOrDefault();

            BackBtn= BuildInlineBtn("Назад", BuildCallData(MoreSettingsBot.SettingsPaymentMethodCmd, MoreSettingsBot.ModuleName), base.Previuos2Emodji,false);

            if (YandexKassaConfig != null)
            {
                base.TextMessage = "Яндекс Касса" + NewLine() + NewLine() +
                    Bold("Идентификатор магазина: ") + YandexKassaConfig.Login + NewLine() +
                    Bold("Платежный токен: ") + YandexKassaConfig.Pass + NewLine();

                ShopIdEditBtn = BuildInlineBtn("Изм. Идентификатор магазина", BuildCallData(MoreSettingsBot.YandexKassaShopIdEditCmd, MoreSettingsBot.ModuleName),base.PenEmodji);
                TokenEditBtn = BuildInlineBtn("Изм. токен", BuildCallData(MoreSettingsBot.YandexKassaTokenEditCmd, MoreSettingsBot.ModuleName),base.PenEmodji);
                RemoveBtn = BuildInlineBtn("Удалить", BuildCallData(MoreSettingsBot.YandexRemoveCmd, MoreSettingsBot.ModuleName),base.CrossEmodji);

                base.MessageReplyMarkup = new InlineKeyboardMarkup(new[]
                {
                new[]
                {
                    TokenEditBtn, ShopIdEditBtn
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

            }

            else
            {
                AddBtn = BuildInlineBtn("Добавить", BuildCallData(MoreSettingsBot.YandexAddCmd, MoreSettingsBot.ModuleName), base.PenEmodji);

                base.MessageReplyMarkup = new InlineKeyboardMarkup(new[]
                {
                        new[]
                        {
                            AddBtn
                        },               
                        new[]
                        {
                            BackBtn
                        }
                });

            }

            return this;

        }
    }
}
