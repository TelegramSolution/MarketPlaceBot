using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyTelegramBot.Services;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Services.BitCoinCore;
using MyTelegramBot.Services.Qiwi;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.BusinessLayer
{
    public class PaymentsFunction
    {
        private ICryptoCurrency CryptoCurrency { get; set; }

        private MarketBotDbContext db { get; set; }

        private QiwiFunction QiwiFunction { get; set; }

        /// <summary>
        /// Количество подвержедний для криптовалютных транзакций
        /// </summary>
        private const int ConfirmAmount = 2;

        public PaymentsFunction()
        {
            db = new MarketBotDbContext();
        }

        public void Dispose()
        {
            if (db != null)
                db.Dispose();
        }

        public static List<Payment> GetPaymentsList()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return db.Payment.Include(p => p.Invoice.Orders).OrderByDescending(p=>p.Id).ToList();
            }

            catch (Exception e)
            {
                return null;
            }

            finally
            {
                db.Dispose();
            }
        }

        public static Payment GetPayment(int PaymentId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return db.Payment.Where(p => p.Id == PaymentId).Include(p => p.Invoice.Orders).LastOrDefault();
            }

            catch
            {
                return null;
            }

            finally
            {
                db.Dispose();
            }
        }

        public static Payment GetPaymentByInvoiceId(int InvoiceId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return db.Payment.Where(p => p.InvoiceId == InvoiceId).Include(p => p.Invoice.Orders).LastOrDefault();
            }

            catch
            {
                return null;
            }

            finally
            {
                db.Dispose();
            }
        }

        public List<Invoice> FindNoPaidInvoice()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return db.Invoice.Where(i=>i.Paid==false).Include(p => p.Orders).ToList();
            }

            catch
            {
                return null;
            }

            finally
            {
                db.Dispose();
            }
        }

        public async Task<Invoice> CheckPaidInvoice(int OrderId)
        {
          var order= db.Orders.Where(p=>p.Id== OrderId).Include(p=>p.Invoice.Payment).FirstOrDefault();

            if (order != null)
                return await CheckPaidInvoice(order.Invoice);

            else
                return null;
        }

        /// <summary>
        /// Проверить оплачен ли инвойс. Если оплачен вернет инвойс со стасутом Оплачен и платежом
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        public async Task<Invoice> CheckPaidInvoice(Invoice invoice)
        {
            Payment payment = null;

            if(invoice!=null && invoice.PaymentTypeId==ConstantVariable.PaymentTypeVariable.QIWI)
            {
                payment =await CheckQiwiInvoce(invoice);
            }

            if(invoice!=null && !invoice.Paid && invoice.PaymentTypeId==ConstantVariable.PaymentTypeVariable.Bitcoin ||
                invoice.PaymentTypeId == ConstantVariable.PaymentTypeVariable.BitcoinCash ||
                invoice.PaymentTypeId == ConstantVariable.PaymentTypeVariable.Litecoin ||
                invoice.PaymentTypeId == ConstantVariable.PaymentTypeVariable.Doge)
            {
                payment = CheckCryptoCurrency(invoice);
            }

            //Найден платеж, смотрим зачисленная сумма больше или равна сумме которая была указана в инвойсе
            if (invoice!=null && !invoice.Paid &&payment != null && invoice != null && payment.Summ >= invoice.Value)
            {
               invoice=InvoiceAndOrderIsPaid(invoice.Id).Invoice;
               invoice.Payment.Add(payment);
            }

            return invoice;
        }

        private async Task<Payment> CheckQiwiInvoce(Invoice invoice)
        {
            var token = db.PaymentTypeConfig.Where(p => p.Login == invoice.AccountNumber).LastOrDefault();

            if (token != null)
            {
                var transaction = await QiwiFunction.SearchPayment(invoice.Comment, token.Pass, invoice.AccountNumber);

                if (transaction != null)
                    return AddPayment(invoice.Id, transaction.txnId.ToString(), transaction.sum.amount, DateTime.Now);
                

                else
                    return null;
            }

            else
                return null;
        }

        private Payment CheckCryptoCurrency (Invoice invoice)
        {
            var config = db.PaymentTypeConfig.Where(p => p.PaymentId == invoice.PaymentTypeId).OrderByDescending(p => p.Id).FirstOrDefault();

            double balance = 0;

            if (config != null)
            {
                CryptoCurrency = new BitCoin(config.Login, config.Pass, config.Host, config.Port);
                balance = CryptoCurrency.GetBalance(invoice.AccountNumber);

                if (balance >= invoice.Value)
                {
                    var transaction = CryptoCurrency.GetListTransactions<TransactionInfoList>();
                    var txinfo =transaction.result.Where(t => t.address == invoice.AccountNumber).LastOrDefault();

                    if(txinfo!=null && txinfo.confirmations>=ConfirmAmount) // нашли транзакцию. Проверям кол-во подтверждений
                        return AddPayment(invoice.Id, txinfo.txid, balance, UnixTimeStampToDateTime(txinfo.timereceived));

                    if (txinfo == null)
                        return AddPayment(invoice.Id, "", balance, DateTime.Now);

                    return null;
                }

                else
                    return null;
            }

            else
                return null;
        }

        private  DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        private Payment AddPayment(int InvoiceId, string TxnId, double value, DateTime TxdateTime)
        {
            var ReapetPayment = db.Payment.Where(p => p.InvoiceId == InvoiceId && p.TxId == TxnId).FirstOrDefault();

            if (ReapetPayment == null)
            {

                Payment payment = new Payment
                {
                    TimestampDataAdd = DateTime.Now,
                    Summ = value,
                    TxId = TxnId,
                    InvoiceId = InvoiceId,
                    TimestampTx= TxdateTime
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

        /// <summary>
        /// Поставить статус "Оплачено у заказа и инвойса"
        /// </summary>
        /// <param name="OrderId"></param>
        /// <param name="paid"></param>
        /// <returns></returns>
        private Orders InvoiceAndOrderIsPaid(int InvoiceId)
        {

            var order = db.Orders.Where(o => o.InvoiceId == InvoiceId).Include(o => o.Invoice).FirstOrDefault();

            if (order != null)
            {
                var Invoice = order.Invoice;
                order.Paid = true;
                Invoice.Paid = true;
                db.Update<Orders>(order);
                db.Update<Invoice>(order.Invoice);
                db.SaveChanges();
                return order;

            }

            else
                return null;
        }
    }
}
