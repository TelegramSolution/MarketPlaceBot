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
    public class CurrencySettingsMessage:BotMessage
    {

        private List<Currency> CurrencyList { get; set; }

        private InlineKeyboardCallbackButton[][] Btns { get; set; }

        MarketBotDbContext db { get; set; }

        BotInfo BotInfo { get; set; }
        public CurrencySettingsMessage(BotInfo botInfo)
        {
            BotInfo = botInfo;
        }

        public override BotMessage BuildMsg()
        {

            db = new MarketBotDbContext();
            CurrencyList = db.Currency.ToList();
            db.Dispose();

            BackBtn = BuildInlineBtn("Назад", BuildCallData(MoreSettingsBot.BackToMoreSettingsCmd, MoreSettingsBot.ModuleName), base.Previuos2Emodji, false);
            Btns = new InlineKeyboardCallbackButton[CurrencyList.Count + 1][];
            Btns[Btns.Length - 1] = new InlineKeyboardCallbackButton[1];
            Btns[Btns.Length - 1][0] = BackBtn;

            int counter = 0;

            foreach(var c in CurrencyList)
            {
                Btns[counter] = new InlineKeyboardCallbackButton[1];

                if(BotInfo.Configuration.CurrencyId==c.Id)
                    Btns[counter][0] = BuildInlineBtn(c.Name, BuildCallData(MoreSettingsBot.CurrencyEditorUpdCmd, MoreSettingsBot.ModuleName, c.Id), base.CheckEmodji);

                else
                    Btns[counter][0] = BuildInlineBtn(c.Name, BuildCallData(MoreSettingsBot.CurrencyEditorUpdCmd, MoreSettingsBot.ModuleName, c.Id), base.UnCheckEmodji);

                counter++;
            }

            base.TextMessage = "Валюта";

            base.MessageReplyMarkup = new InlineKeyboardMarkup(Btns);

            return this;

        }
    }
}
