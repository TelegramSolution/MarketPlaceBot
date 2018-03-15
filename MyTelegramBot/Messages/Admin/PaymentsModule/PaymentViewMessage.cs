using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.InlineQueryResults;
using MyTelegramBot.Bot;
using Newtonsoft.Json;
using MyTelegramBot.Bot.Core;
using MyTelegramBot.BusinessLayer;
using MyTelegramBot.Bot.AdminModule;

namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// Детали платежа
    /// </summary>
    public class PaymentViewMessage:BotMessage
    {

        private InlineKeyboardCallbackButton ViewOrderBtn { get; set; }

        private InlineKeyboardCallbackButton ViewInvoiceBtn { get; set; }

        private InlineKeyboardButton SearchPaymentBtn { get; set; }

        private int PaymentId { get; set; }

        private Payment Payment { get; set; }

        public PaymentViewMessage (Payment payment)
        {
            Payment = payment;
        }

        public PaymentViewMessage(int PaymentId)
        {
            this.PaymentId = PaymentId;
        }

        public override BotMessage BuildMsg()
        {
            if (Payment == null)
                Payment=PaymentsFunction.GetPayment(PaymentId);

            if (Payment!=null && Payment.Invoice != null && Payment.Invoice.Orders.Count > 0)
            {
                ViewOrderBtn = BuildInlineBtn("Детали заказа", BuildCallData(OrderProccesingBot.CmdGetOrderAdmin, OrderProccesingBot.ModuleName, Payment.Invoice.Orders.FirstOrDefault().Id),base.PackageEmodji);

                ViewInvoiceBtn = BuildInlineBtn("Счет", BuildCallData(OrderProccesingBot.ViewInvoiceCmd, OrderProccesingBot.ModuleName, Payment.Invoice.Orders.FirstOrDefault().Id),base.PaperEmodji);

                SearchPaymentBtn = InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Платежи"+base.CreditCardEmodji, InlineFind.Payment + "|");

                base.TextMessage = "Платеж №" + Payment.Id.ToString() + " для Заказа №" + Payment.Invoice.Orders.FirstOrDefault().Number.ToString()
                                   + NewLine() + Bold("Дата добавления платежа:") + Payment.TimestampDataAdd.ToString() + NewLine() +
                                   Bold("Сумма:") + Payment.Summ.ToString() + NewLine() +
                                   Bold("ID Транзакции:") + Payment.TxId.ToString();

                base.MessageReplyMarkup = new InlineKeyboardMarkup(new[]
                {
                        new[]
                        {
                            ViewInvoiceBtn
                        },
                        new[]
                        {
                            ViewOrderBtn
                        },
                        new[]
                        {
                            SearchPaymentBtn
                        }
                });

            }
            return this;

        }
    }
}
