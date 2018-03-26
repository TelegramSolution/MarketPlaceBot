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
    /// <summary>
    /// Настройка яндекс кассы
    /// </summary>
    public class YandexKassaEditMessage:BotMessage
    {
        private InlineKeyboardCallbackButton RemoveBtn { get; set; }

        private InlineKeyboardCallbackButton TokenEditBtn { get; set; }

        private InlineKeyboardCallbackButton YandexFaqBtn { get; set; }

        private InlineKeyboardCallbackButton AddBtn { get; set; }

        private PaymentTypeConfig YandexKassaConfig { get; set; }

        private MarketBotDbContext db { get; set; }

        public override BotMessage BuildMsg()
        {

            db = new MarketBotDbContext();

            YandexKassaConfig = db.PaymentTypeConfig.Where(p => p.PaymentId == ConstantVariable.PaymentTypeVariable.DebitCardForYandexKassa).FirstOrDefault();

            BackBtn= BuildInlineBtn("Назад", BuildCallData(MoreSettingsBot.SettingsPaymentMethodCmd, MoreSettingsBot.ModuleName), base.Previuos2Emodji,false);

            YandexFaqBtn= BuildInlineBtn("Инструкция", BuildCallData(MoreSettingsBot.SettingsPaymentMethodCmd, MoreSettingsBot.ModuleName), base.Previuos2Emodji, false);

            if (YandexKassaConfig != null)
            {
                base.TextMessage = "Яндекс Касса (бот не передает в Кассу данные для фискализации. Будет реализовано позднее)"+NewLine()+
                    HrefUrl("Как подключить яндекс кассу к боту ?", "https://kassa.yandex.ru/manuals/telegram")
                    + NewLine() + NewLine() +
                    Bold("Идентификатор магазина: ") + YandexKassaConfig.Login + NewLine() +
                    Bold("Платежный токен: ") + YandexKassaConfig.Pass + NewLine();

                TokenEditBtn = BuildInlineBtn("Изм. токен", BuildCallData(MoreSettingsBot.YandexKassaTokenEditCmd, MoreSettingsBot.ModuleName),base.PenEmodji);
                RemoveBtn = BuildInlineBtn("Удалить", BuildCallData(MoreSettingsBot.YandexRemoveCmd, MoreSettingsBot.ModuleName),base.CrossEmodji);

                base.MessageReplyMarkup = new InlineKeyboardMarkup(new[]
                {
                new[]
                {
                    TokenEditBtn
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
                base.TextMessage = "Яндекс Касса (бот не передает в Кассу данные для фискализации. Будет реализовано позднее)" + NewLine()
                     +HrefUrl("Как подключить яндекс кассу к боту ?", "https://kassa.yandex.ru/manuals/telegram");
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
