using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementBots.Bot;
using ManagementBots.Bot.Core;
using ManagementBots.Db;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Microsoft.EntityFrameworkCore;


namespace ManagementBots.Messages
{
    public class ServiceInfoMessage:BotMessage
    {
        private Service Service { get; set; }

        private Db.Bot Bot { get; set; }

        private InlineKeyboardCallbackButton ProlongBtn { get; set; }

        private InlineKeyboardCallbackButton InvoiceViewBtn { get; set; }

        private InlineKeyboardCallbackButton BuyPaidVersionBtn { get; set; }

        private int BotId { get; set; }

        private BotMngmntDbContext DbContext { get; set; }

        public ServiceInfoMessage (Db.Bot bot, Service service)
        {
            Bot = bot;
            Service = service;
        }

        public ServiceInfoMessage (int BotId)
        {
            this.BotId = BotId;
        }


        public override BotMessage BuildMsg()
        {
            DbContext = new BotMngmntDbContext();

            if (Bot == null && Service==null && BotId>0)
            {

                Bot = DbContext.Bot.Where(b => b.Id == BotId).Include(b => b.Service.ServiceType).FirstOrDefault();
                Service = Bot.Service;
            
            }

            var ServiceType = DbContext.ServiceType.Where(s => s.Enable && !s.IsDemo).LastOrDefault(); // ищем платный тариф

            BackBtn = BuildInlineBtn("Назад", BuildCallData(MainMenuBot.ToMainMenuCmd,MainMenuBot.ModuleName), base.Previuos2Emodji);

            base.TextMessage = "Услуга №" + Service.Id + NewLine() +
                               Bold("Тариф:")+ Service.ServiceType.Name+NewLine()+
                               Bold("Дата завершения услуги:") + Service.EndTimeStamp.ToString() + NewLine() +
                               Bold("Бот:") + "@" + Bot.BotName;

            ProlongBtn = BuildInlineBtn("Продлить", BuildCallData(ConnectBot.PaidVersionCmd, ConnectBot.ModuleName, Bot.Id,Convert.ToInt32(Bot.Service.ServiceTypeId)),base.CreditCardEmodji);

            InvoiceViewBtn = BuildInlineBtn("Посмотреть счет", BuildCallData("InvoiceView", ConnectBot.ModuleName, Bot.Id, Convert.ToInt32(Service.InvoiceId)));

            BuyPaidVersionBtn = BuildInlineBtn("Приобрести платную версию", BuildCallData(ConnectBot.PaidVersionCmd, ConnectBot.ModuleName, Bot.Id, ServiceType.Id), base.CreditCardEmodji);


            base.MessageReplyMarkup = SetKeyboard();

            DbContext.Dispose();

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
