using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// сообщение со статусом платежа найден / не найден 
    /// </summary>
    public class CheckPayMessage:BotMessage
    {
        public Orders Order { get; set; }

        private int OrderId { get; set; }

        MarketBotDbContext db;

        Services.ICryptoCurrency CryptoCurrency { get; set; }

        Services.BitCoinCore.BitCoin BitCoinCore { get; set; }

        public CheckPayMessage(int OrderID)
        {
            this.OrderId = OrderID;

            db = new MarketBotDbContext();
        }


        public CheckPayMessage(Orders Order)
        {
            this.Order = Order;

            db = new MarketBotDbContext();
        }

        private Orders GetOrder(int OrderId)
        {
             return db.Orders.Where(o => o.Id == OrderId).Include(o=>o.Invoice).FirstOrDefault();
        }

        public async Task<CheckPayMessage> BuildMessage()
        {
            if(this.Order==null)
                this.Order = GetOrder(this.OrderId);

            if (Order.Paid==false && Order.Invoice.PaymentTypeId == Bot.Core.ConstantVariable.PaymentTypeVariable.QIWI)
               base.TextMessage= await CheckQiwi(Order.Invoice);

            if (Order.Paid == false && Order.Invoice.PaymentTypeId != Bot.Core.ConstantVariable.PaymentTypeVariable.QIWI)
                base.TextMessage = CheckCryptoCurrency(Order.Invoice);

            if (Order.Paid == true)
                base.TextMessage = "Ваш заказ оплачен";


            return this;
        }

        private async Task<string> CheckQiwi(Invoice invoice)
        {
            var token = db.PaymentTypeConfig.Where(p => p.Login == invoice.AccountNumber).FirstOrDefault();

            if (token != null) // токен найден
            {
              var transaction= await Services.Qiwi.QiwiFunction.SearchPayment(invoice.Comment, token.Pass, invoice.AccountNumber);

                Payment payment = new Payment();

                if (transaction != null && (Convert.ToDateTime(transaction.date) - invoice.CreateTimestamp).Value.TotalMinutes <= invoice.LifeTimeDuration.Value.TotalMinutes)
                {
                    payment = AddPayment(invoice.Id, transaction.txnId.ToString(), transaction.sum.amount, Convert.ToDateTime(transaction.date));


                    if (payment != null && invoice.Paid == false && payment.Summ >= invoice.Value)
                    {
                        InvoicePaid(this.Order.Id);

                        invoice.Payment.Add(payment);

                        this.Order.Invoice = invoice;

                        return "Платеж найден. Заказ отправлен на обработку!";
                    }

                    
                }

                if (transaction == null)
                    return "Платеж не найден!";

                if((Convert.ToDateTime(transaction.date) - invoice.CreateTimestamp).Value.Minutes <= invoice.LifeTimeDuration.Value.Minutes)
                    return "Вы оплатили позже положеного времени. Свяжитесь с технической поддержкой!";
                
            }

            return "Ошибка при подключении к QIWI.Свяжитесь с технической поддержкой!";
        }

        private string CheckCryptoCurrency (Invoice invoice)
        {
            var config = db.PaymentTypeConfig.Where(p => p.PaymentId == invoice.PaymentTypeId).OrderByDescending(p => p.Id).FirstOrDefault();

            if (config != null)
            {
                BitCoinCore = new Services.BitCoinCore.BitCoin(config.Login, config.Pass, config.Host, config.Port);
                var TxList = BitCoinCore.GetListTransactions(invoice.AccountNumber);

                //Найден платеж с нужным кол-во монет
                if (TxList != null && TxList.Count > 0 && TxList[TxList.Count - 1].amount >= invoice.Value)
                {
                    DateTime pDate = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(TxList[TxList.Count - 1].timereceived);

                    if (pDate > invoice.CreateTimestamp.Value.AddMinutes(invoice.LifeTimeDuration.Value.Minutes))
                        return "Вы оплатили позже положеного времени. Свяжитесь с технической поддержкой!";

                    else
                    {
                        if (invoice.Paid == false)
                            InvoicePaid(this.Order.Id);

                        invoice.Payment.Add(AddPayment(invoice.Id, TxList[TxList.Count - 1].txid, TxList[TxList.Count - 1].amount, pDate));

                        return "Платеж найден. Заказ отправлен на обработку!";
                    }
                }

                else
                    return "Платеж не найден.";


            }

            else
                return "Ошибка при подключении к платежной сети .Свяжитесь с технической поддержкой!";
        }

        private Payment AddPayment(int InvoiceId,string TxnId, double value, DateTime dateTime)
        {
            var ReapetPayment = db.Payment.Where(p => p.InvoiceId == InvoiceId && p.TxId == TxnId).FirstOrDefault();

            if (ReapetPayment == null)
            {

                Payment payment = new Payment
                {
                    DataAdd = dateTime,
                    Summ = value,
                    TxId = TxnId,
                    InvoiceId = InvoiceId
                };

                db.Payment.Add(payment);

                if (db.SaveChanges() > 0)
                    return payment;

                else
                    return null;
            }

            else
                return ReapetPayment;
        }

        private int InvoicePaid (int OrderId , bool paid=true)
        {


            var order = db.Orders.Where(o => o.Id == OrderId).Include(o=>o.Invoice).FirstOrDefault();

            if (order != null)
            {
                var Invoice = order.Invoice;
                order.Paid = paid;
                Invoice.Paid = paid;
                int save = db.SaveChanges();
                this.Order.Paid = paid;
                this.Order.Invoice.Paid = paid;
                return save;
            }

            else
                return 0;
        }
    }
}
