using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot.AdminModule;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages.Admin
{
    public class AdminQiwiSettingsMessage:BotMessage
    {
        private InlineKeyboardCallbackButton RemoveBtn { get; set; }

        private InlineKeyboardCallbackButton TokenEditBtn { get; set; }

        private InlineKeyboardCallbackButton EnableBtn { get; set; }

        private InlineKeyboardCallbackButton TestConnectionBtn { get; set; }

        private PaymentTypeConfig QiwiConfig { get; set; }



        public AdminQiwiSettingsMessage(PaymentTypeConfig config)
        {
            this.QiwiConfig = config;
        }

        public override BotMessage BuildMsg()
        {
            if (this.QiwiConfig != null)
            {
                base.TextMessage = Bold("Номер телефона: ") + QiwiConfig.Login + NewLine() +
                    Bold("Токен: ") + QiwiConfig.Pass + NewLine();

                this.RemoveBtn = base.BuildInlineBtn("Удалить", BuildCallData(MoreSettingsBot.QiwiRemoveCmd, MoreSettingsBot.ModuleName, QiwiConfig.Id), base.CrossEmodji);

                this.TokenEditBtn = base.BuildInlineBtn("Изменить токен", BuildCallData(MoreSettingsBot.QiwiEditTokenCmd, MoreSettingsBot.ModuleName, QiwiConfig.Id), base.PenEmodji);

                this.BackBtn = BuildInlineBtn("Назад", BuildCallData(MoreSettingsBot.SettingsPaymentMethodCmd, MoreSettingsBot.ModuleName), base.Previuos2Emodji, false);

                this.TestConnectionBtn = BuildInlineBtn("Проверить соединение", BuildCallData(MoreSettingsBot.QiwiTestConectionCmd, MoreSettingsBot.ModuleName,QiwiConfig.Id));

                base.MessageReplyMarkup = new InlineKeyboardMarkup(new[]
                {
                new[]
                {
                    TokenEditBtn, RemoveBtn
                },
                    new[]
                    {
                        TestConnectionBtn
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
