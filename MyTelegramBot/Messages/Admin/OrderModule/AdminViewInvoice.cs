using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyTelegramBot.BusinessLayer;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using MyTelegramBot.Bot.AdminModule;

namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// Счет на оплату
    /// </summary>
    public class AdminViewInvoice:BotMessage
    {
        private Invoice Invoice { get; set; }

        private int OrderId { get; set; }

        private InlineKeyboardCallbackButton BackToOrderBtn { get; set; }

        private InlineKeyboardCallbackButton ViewPaymentBtn { get; set; }

        Orders Orders { get; set; }

        public AdminViewInvoice(int OrderId)
        {
            this.OrderId = OrderId;

        }

        public AdminViewInvoice (Invoice invoice)
        {
            Invoice = invoice;
        }

        public override BotMessage BuildMsg()
        {
            string IsPaid = "Нет";

            if(Invoice==null && OrderId>0)
                Invoice = InvoiceFunction.GetInvoiceByOrderId(OrderId);

            if (Invoice != null)
            {

                if (Invoice.Paid && Invoice.Payment.Count > 0)
                    IsPaid = "Да (" + Invoice.Payment.LastOrDefault().TimestampDataAdd.ToString() + ")";

                base.TextMessage = Bold("Счет на оплату №") + Invoice.InvoiceNumber.ToString() + NewLine() +
                                 Bold("Адрес счета получателя:") + Invoice.AccountNumber + NewLine() +
                                 Bold("Комментарий к платежу:") + Invoice.Comment + NewLine() +
                                 Bold("Сумма: ") + Invoice.Value.ToString() + " " + Invoice.PaymentType.Code + NewLine() +
                                 Bold("Время создания: ") + Invoice.CreateTimestamp.ToString() + NewLine() +
                                 Bold("Способ оплаты: ") + Invoice.PaymentType.Name + NewLine() +
                                 Bold("Оплачено:") + IsPaid;

                BackToOrderBtn = BuildInlineBtn("Детали заказа", BuildCallData(OrderProccesingBot.CmdBackToOrder, OrderProccesingBot.ModuleName, Invoice.Orders.LastOrDefault().Id));

                ViewPaymentBtn = BuildInlineBtn("Платеж", BuildCallData(OrderProccesingBot.ViewPaymentCmd, OrderProccesingBot.ModuleName, Invoice.Orders.LastOrDefault().Id, Invoice.Payment.LastOrDefault().Id));

                if (Invoice.PaymentType != null && Invoice.PaymentType.Id == ConstantVariable.PaymentTypeVariable.Litecoin ||
                    Invoice.PaymentType != null && Invoice.PaymentType.Id == ConstantVariable.PaymentTypeVariable.Bitcoin ||
                    Invoice.PaymentType != null && Invoice.PaymentType.Id == ConstantVariable.PaymentTypeVariable.Doge)
                    base.TextMessage += NewLine() + NewLine() +
                        HrefUrl("https://live.blockcypher.com/" + Invoice.PaymentType.Code + "/address/" + Invoice.AccountNumber, "Посмотреть счет");


                if (Invoice.PaymentType != null && Invoice.PaymentType.Id == ConstantVariable.PaymentTypeVariable.BitcoinCash)
                    base.TextMessage += NewLine() + NewLine() +
                        HrefUrl("https://blockchair.com/bitcoin-cash/address/" + Invoice.AccountNumber, "Посмотреть счет");

                if (Invoice.PaymentType != null && Invoice.PaymentType.Id == ConstantVariable.PaymentTypeVariable.Dash)
                    base.TextMessage += NewLine() + NewLine() +
                         HrefUrl("https://explorer.dash.org/address/" + Invoice.AccountNumber, "Посмотреть счет");

                if (Invoice.PaymentType != null && Invoice.PaymentType.Id == ConstantVariable.PaymentTypeVariable.Zcash)
                    base.TextMessage += NewLine() + NewLine() +
                         HrefUrl("https://explorer.zcha.in/accounts/" + Invoice.AccountNumber, "Посмотреть счет");


                SetButtons();

                return this;

            }

            else
                return null;
        }

        private void SetButtons()
        {
            base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                    {
                        BackToOrderBtn,ViewPaymentBtn

                    },
                    new[]
                    {
                        BackToAdminPanelBtn()
                    }
                });

        }


    }
}
