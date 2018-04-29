using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementBots.Bot;
using ManagementBots.Bot.Core;
using ManagementBots.Db;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InlineKeyboardButtons;

namespace ManagementBots.Messages
{
    public class ServiceInfoMessage:BotMessage
    {
        private Service Service { get; set; }

        private Db.Bot Bot { get; set; }

        private InlineKeyboardCallbackButton ProlongBtn { get; set; }

        private InlineKeyboardCallbackButton InvoiceViewBtn { get; set; }

        private InlineKeyboardCallbackButton BuyPaidVersionBtn { get; set; }

        public ServiceInfoMessage (Db.Bot bot, Service service)
        {
            Bot = bot;
            Service = service;
        }


        public override BotMessage BuildMsg()
        {
            BackBtn = BuildInlineBtn("Назад", BuildCallData(MainMenuBot.ToMainMenuCmd,MainMenuBot.ModuleName), base.Previuos2Emodji);

            base.TextMessage = "Услуга №" + Service.Id + NewLine() +
                               Bold("Тариф:")+ Service.ServiceType.Name+NewLine()+
                               Bold("Дата начала:") + Service.StartTimeStamp.ToString() + NewLine() +
                               Bold("Продолжительность:") + Service.DayDuration + NewLine() +
                               Bold("Бот:") + "@" + Bot.BotName;

            ProlongBtn = BuildInlineBtn("Продлить", BuildCallData("", "", Bot.Id),base.CreditCardEmodji);

            InvoiceViewBtn = BuildInlineBtn("Посмотреть счет", BuildCallData("InvoiceView", "", Bot.Id, Convert.ToInt32(Service.InvoiceId)));

            BuyPaidVersionBtn = BuildInlineBtn("Приобрести платную версию", BuildCallData("", "", Bot.Id), base.CreditCardEmodji);

            base.MessageReplyMarkup = SetKeyboard();

            return this;
        }

        private InlineKeyboardMarkup SetKeyboard()
        {
            if(!Service.ServiceType.IsDemo)
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
