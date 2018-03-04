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
    /// вкл / выкл способов оплаты
    /// </summary>
    public class EnablePaymentsMethodMessage : BotMessage
    {
        private List<PaymentType> PaymentTypeList { get; set; }

        private InlineKeyboardCallbackButton[][] Btns {get;set;}

        MarketBotDbContext db { get; set; }

        public override BotMessage BuildMsg()
        {
            db = new MarketBotDbContext();

            PaymentTypeList = db.PaymentType.ToList();

            BackBtn = BuildInlineBtn("Назад", BuildCallData(MoreSettingsBot.BackToMoreSettingsCmd, MoreSettingsBot.ModuleName), base.Previuos2Emodji, false);

            Btns = new InlineKeyboardCallbackButton[PaymentTypeList.Count + 1][];
            Btns[Btns.Length - 1] = new InlineKeyboardCallbackButton[1];
            Btns[Btns.Length - 1][0] = BackBtn;

            int counter = 0;

            foreach(var p in PaymentTypeList)
            {
                Btns[counter] = new InlineKeyboardCallbackButton[1];

                if (p.Enable)
                    Btns[counter][0] = BuildInlineBtn(p.Name, BuildCallData(MoreSettingsBot.EnablePaymentMethodCmd, MoreSettingsBot.ModuleName, p.Id), base.CheckEmodji);

                else
                    Btns[counter][0] = BuildInlineBtn(p.Name, BuildCallData(MoreSettingsBot.EnablePaymentMethodCmd, MoreSettingsBot.ModuleName, p.Id), base.UnCheckEmodji);

                counter++;
            }

            db.Dispose();

            base.TextMessage = "Вкл/выкл способ оплаты";

            base.MessageReplyMarkup = new InlineKeyboardMarkup(Btns);

            return this;
        }


    }
}
