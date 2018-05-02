using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;
using System.Web;
using Telegram.Bot.Types.InlineKeyboardButtons;
using ManagementBots.Bot;
using ManagementBots.Bot.Core;
using ManagementBots.Db;
using Microsoft.EntityFrameworkCore;


namespace ManagementBots.Messages
{
    public class BotInfoMessage:BotMessage
    {
        Db.Bot Bot { get; set; }

        private int BotId { get; set; }

        BotMngmntDbContext DbContext { get; set; }

        private InlineKeyboardCallbackButton ProlongBtn { get; set; }

        private InlineKeyboardCallbackButton InvoiceViewBtn { get; set; }

        private InlineKeyboardCallbackButton BuyPaidVersionBtn { get; set; }


        public BotInfoMessage(int Botid)
        {
            this.BotId = Botid;
        }

        public BotInfoMessage(Db.Bot bot)
        {
            this.Bot = bot;
        }

        public override BotMessage BuildMsg()
        {
            DbContext = new BotMngmntDbContext();

            var ServiceType = DbContext.ServiceType.Where(s => s.Enable && !s.IsDemo).LastOrDefault();

            if(Bot==null)
                Bot = DbContext.Bot.Where(b => b.Id == BotId).Include(b => b.Service.ServiceType).FirstOrDefault();

            DbContext.Dispose();

            base.TextMessage = "Услуга №" + Bot.Service.Id + NewLine() +
                   Bold("Тариф:") + Bot.Service.ServiceType.Name + NewLine() +
                   Bold("Дата завершения услуги:") +Bot. Service.EndTimeStamp.ToString() + NewLine() +
                   Bold("Бот:") + "@" + Bot.BotName;


            if (Bot.Service.InvoiceId > 0)
            {
                ProlongBtn = BuildInlineBtn("Продлить", BuildCallData(ConnectBot.PaidVersionCmd, ConnectBot.ModuleName, Bot.Id, Convert.ToInt32(Bot.Service.ServiceTypeId)), base.CreditCardEmodji);

                InvoiceViewBtn = BuildInlineBtn("Посмотреть счет", BuildCallData("InvoiceView", ConnectBot.ModuleName, Bot.Id, Convert.ToInt32(Bot.Service.InvoiceId)));
            }
            BuyPaidVersionBtn = BuildInlineBtn("Приобрести платную версию", BuildCallData(ConnectBot.PaidVersionCmd, ConnectBot.ModuleName, Bot.Id,ServiceType.Id), base.CreditCardEmodji);

            BackBtn = BuildInlineBtn("На главную", BuildCallData(MainMenuBot.ToMainMenuCmd, MainMenuBot.ModuleName));

            base.MessageReplyMarkup = SetKeyboard();

            return base.BuildMsg();
        }

        private InlineKeyboardMarkup SetKeyboard()
        {
            if (!Bot.Service.ServiceType.IsDemo)
                return new InlineKeyboardMarkup(
                new[]{
                        new[]
                        {
                            ProlongBtn
                        },

                        new[]
                        {
                            InvoiceViewBtn
                        },
                        new[]
                        {
                            BackBtn
                        }

                });

            else
                return new InlineKeyboardMarkup(
                new[]{
                        new[]
                        {
                            BuyPaidVersionBtn
                        },

                        new[]
                        {
                            BackBtn
                        }

                });
        }
    }
}
