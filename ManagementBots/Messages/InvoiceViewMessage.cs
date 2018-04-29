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
    public class InvoiceViewMessage:BotMessage
    {
        private Invoice Invoice { get; set; }

        private InlineKeyboardCallbackButton CheckPayBtn { get; set; }

        private int BotId { get; set; }

        private int ServiceTypeId { get; set; }

        private const int DurationMinute = 30; 

        public InvoiceViewMessage(Invoice invoice, int BotId, int ServiceTypeId)
        {
            Invoice = invoice;
            this.ServiceTypeId = ServiceTypeId;
            this.BotId = BotId;
        }

        public override BotMessage BuildMsg()
        {
            BackBtn = BuildInlineBtn("Назад", BuildCallData(ConnectBot.BackToDurationEnterCmd, ConnectBot.ModuleName, BotId, ServiceTypeId), base.Previuos2Emodji);

            CheckPayBtn = BuildInlineBtn("Я оплатил", BuildCallData(ConnectBot.CheckPayCmd, ConnectBot.ModuleName, BotId, Invoice.Id), base.CreditCardEmodji);

            base.TextMessage = "Счет № " + Invoice.Id.ToString() + NewLine() +
                              Bold("Счет получателя:") + Invoice.AccountNumber + NewLine() +
                              Bold("Сумма:") + Invoice.Summ + NewLine() +
                              Bold("Комментарий к платежу:") + Invoice.Id + NewLine() +
                              Bold("Счет оплачен:") + "Нет."+ NewLine()+ NewLine()+
                              base.WarningEmodji+ Italic("Обязательно указывайте комментарий к платежу!")+NewLine()+ NewLine()+
                              base.WarningEmodji + Italic("Вы должны оплатить счет не позднее " + Invoice.CreateTimeStamp.Value.Add(new TimeSpan(0,DurationMinute,0)).ToString())  + 
                              NewLine() + NewLine() +
                              base.CreditCardEmodji+ HrefUrl(QiwiForm(Invoice.AccountNumber,Convert.ToInt32(Invoice.Summ),Invoice.Id.ToString()),"Открыть платежную форму");

            base.MessageReplyMarkup = SetKeyboard();

            return this;
        }

        private InlineKeyboardMarkup SetKeyboard()
        {
            return new InlineKeyboardMarkup(
            new[]{
                        new[]
                        {
                            BackBtn
                        },

                        new[]
                        {
                            CheckPayBtn
                        }

            });
        }

        private string QiwiForm(string tel, int summ, string comm)
        {
            //https://qiwi.com/payment/form/99?extra%5B%27account%27%5D=79991112233&amountInteger=1&extra%5B%27comment%27%5D=test123&currency=643


            return "https://qiwi.com/payment/form/99?" + "extra%5B%27account%27%5D=" + tel + "&amountInteger=" + summ.ToString() + "&extra%5B%27comment%27%5D=" + comm + "&currency=643";
        }
    }
}
