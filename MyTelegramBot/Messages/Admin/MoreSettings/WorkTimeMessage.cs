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
    /// Режим работы магазина
    /// </summary>
    public class WorkTimeMessage:BotMessage
    {
        private InlineKeyboardCallbackButton StartTimeBtn { get; set; }

        private InlineKeyboardCallbackButton EndTimeBtn { get; set; }

        private InlineKeyboardCallbackButton RemoveBtn { get; set; }

        private BotInfo BotInfo { get; set; }

        public WorkTimeMessage(BotInfo botInfo)
        {
            BotInfo = botInfo;
        }

        public override BotMessage BuildMsg()
        {
            if (BotInfo != null && BotInfo.Configuration != null)
            {
                if (BotInfo.Configuration.StartTime == null && BotInfo.Configuration.EndTime == null)
                {
                    base.TextMessage = "Режим работы магазина: " + NewLine() +Italic("Клиенты могут оформлять заказы круглосуточно");
                }

                if (BotInfo.Configuration.StartTime != null && BotInfo.Configuration.EndTime == null)
                {
                    base.TextMessage = "Режим работы магазина:" + NewLine() + "Начало: " + BotInfo.Configuration.StartTime.ToString();

                }

                if (BotInfo.Configuration.StartTime == null && BotInfo.Configuration.EndTime != null)
                {
                    base.TextMessage = "Режим работы магазина:" + NewLine() + "Конец: " + BotInfo.Configuration.EndTime.ToString();

                }

                if (BotInfo.Configuration.StartTime != null && BotInfo.Configuration.EndTime != null)
                {
                    base.TextMessage = "Режим работы магазина:" + NewLine() + "Начало: " + BotInfo.Configuration.StartTime.ToString()
                        + NewLine() + "Конец: " + BotInfo.Configuration.EndTime.ToString();

                }

                StartTimeBtn = BuildInlineBtn("Начало", BuildCallData(MoreSettingsBot.StartWorkTimeCmd, MoreSettingsBot.ModuleName),base.ClockEmodji);

                EndTimeBtn = BuildInlineBtn("Конец", BuildCallData(MoreSettingsBot.EndWorkTimeCmd, MoreSettingsBot.ModuleName),base.ClockEmodji);

                RemoveBtn = BuildInlineBtn("Очистить", BuildCallData(MoreSettingsBot.RemoveWorkTimeCmd, MoreSettingsBot.ModuleName),base.CrossEmodji);

                BackBtn = BuildInlineBtn("Назад", BuildCallData(MoreSettingsBot.BackToMoreSettingsCmd, MoreSettingsBot.ModuleName),base.Previuos2Emodji,false);

                base.MessageReplyMarkup = new InlineKeyboardMarkup(new[] {
                    new[]
                    {
                        StartTimeBtn,EndTimeBtn
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

            return null;
        }
    }
}
