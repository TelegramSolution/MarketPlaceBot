using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot.Core;
using MyTelegramBot.Services;
using MyTelegramBot.Services.BitCoinCore;
using MyTelegramBot.Bot;


namespace MyTelegramBot.BusinessLayer
{
    public class InvoiceFunction
    {
        private ICryptoCurrency CryptoCurrency { get; set; }

        MarketBotDbContext db { get; set; }

        public Invoice AddInvoice(Orders order, int PaymentTypeId, double TotalPrice)
        {
            OpenConnection();

            // создаем инвойс для оплаты в через КИВИ
            if (PaymentTypeId == ConstantVariable.PaymentTypeVariable.QIWI)
                return AddQiwiInvoice(order, TotalPrice);

            if (PaymentTypeId != ConstantVariable.PaymentTypeVariable.PaymentOnReceipt
                && PaymentTypeId != ConstantVariable.PaymentTypeVariable.QIWI
                && PaymentTypeId != ConstantVariable.PaymentTypeVariable.DebitCardForYandexKassa)
                return AddCryptoCurrencyInvoice(order, PaymentTypeId, TotalPrice);

            if (PaymentTypeId == ConstantVariable.PaymentTypeVariable.DebitCardForYandexKassa)
                return AddDibitCardInvoice(order, TotalPrice);

            else
                return null;


        }

        private void OpenConnection()
        {
            db = new MarketBotDbContext();
        }

        private void Dispose()
        {
            db.Dispose();
        }

        /// <summary>
        /// Создать счет на оплату в Криптовалюте
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <param name="paymentTypeId">Тип платежа. Лайткоин, БиткоинКэш и т.д</param>
        /// <param name="Total">Сумма в фиате.</param>
        /// <param name="LifeTimeDuration">Время жизни счета в минутах</param>
        /// <returns></returns>
        private Invoice AddCryptoCurrencyInvoice(Orders order, int paymentTypeId, double Total, int LifeTimeDuration = 30)
        {
            double Summa = 0.0;

            string AccountNumber = "";


            var type = db.PaymentType.Where(p => p.Id == paymentTypeId).FirstOrDefault();

            var PaymentConfig = db.PaymentTypeConfig.Where(p => p.PaymentId == paymentTypeId && p.Enable == true).OrderByDescending(p => p.Id).FirstOrDefault();

            if (PaymentConfig != null)
                CryptoCurrency = new Services.BitCoinCore.BitCoin(PaymentConfig.Login, PaymentConfig.Pass, PaymentConfig.Host, PaymentConfig.Port);

            if (type != null) // конвертируем из фиата в крипту
                Summa = MoneyConvert(Total, type.Code, ConfigurationFunction.MainCurrencyInSystem().Code);

            if (CryptoCurrency != null) // Генерируем адрес куда необходимо перевести деньги
                AccountNumber = CryptoCurrency.GetNewAddress(); 

            if (type != null && CryptoCurrency != null && AccountNumber != null && AccountNumber != null && Summa > 0)
                return InsertInvoice(AccountNumber: AccountNumber, PaymentTypeId: paymentTypeId, Value: Summa);


            else
                return null;
        }

        /// <summary>
        /// Создать счет на оплату для Киви
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <param name="PaymentType">Тип оплаты. Киви</param>
        /// <param name="Total">Сумма в рублях</param>
        /// <param name="LifeTimeDuration">Время жизни счета в минутах</param>
        /// <returns></returns>
        private Invoice AddQiwiInvoice(Orders order, double Total, int LifeTimeDuration = 30)
        {
            var ListQiwi = db.PaymentTypeConfig.Where(q => q.PaymentId == ConstantVariable.PaymentTypeVariable.QIWI && q.Enable == true && q.Pass != "").
                OrderByDescending(q => q.Id).ToList();

            Random random = new Random(); // Может быть добавлено много номеров телефон, поэтому выбрираем рандоно

            var qiwi = ListQiwi[random.Next(0, ListQiwi.Count - 1)];


            if (qiwi != null && qiwi.Login != null)
            {
                return InsertInvoice(AccountNumber: qiwi.Login,
                                     PaymentTypeId: ConstantVariable.PaymentTypeVariable.QIWI,
                                     Value: Total,
                                     Comment: GeneralFunction.BuildPaymentComment(GeneralFunction.GetBotName(),
                                                                                  order.Number.ToString())
                                                                                  );
            }

            else
                return null;
        }

        private Invoice AddDibitCardInvoice(Orders order, double Total, int LifeTimeDuration = 30)
        {
            var YandexKassa = db.PaymentTypeConfig.Where(p => p.PaymentId == ConstantVariable.PaymentTypeVariable.DebitCardForYandexKassa).FirstOrDefault();

            if (YandexKassa != null && order != null && Total > 0)
              return InsertInvoice(YandexKassa.Login + " идентификатор магазина в ЯндексКассе", ConstantVariable.PaymentTypeVariable.DebitCardForYandexKassa, Total);
            
            else
                return null;

        }

        /// <summary>
        /// Конвертируем из фиата в крипту
        /// </summary>
        /// <param name="value">Сумма денег в фиате</param>
        /// <param name="bases">В какую валюту конвертируем (LTC например)</param>
        /// <param name="target">Что конвертируем (RUR- рубль например)</param>
        /// <returns></returns>
        private double MoneyConvert(double value, string bases, string target = "RUB")
        {
            var Cryptonator = Services.CryptonatorConvert.GetCryptonator(bases, target);

            try
            {
                if (Cryptonator != null && Cryptonator.ticker != null && value > 0)
                {
                    double price = Convert.ToDouble(Cryptonator.ticker.price.Replace('.', ','));
                    double convert = value / price;
                    return Math.Round(convert, 6);
                }


                else
                    return -1;
            }
            catch (Exception e)
            {
                return -1;
            }

        }

        private Invoice InsertInvoice(string AccountNumber, int PaymentTypeId, double Value, string Comment = "", int LifeTimeDuration = 30)
        {
            try
            {
                Invoice invoice = new Invoice
                {
                    CreateTimestamp = DateTime.Now,
                    AccountNumber = AccountNumber,
                    Comment = Comment,
                    InvoiceNumber = GenerateInvoiceNumber(),
                    LifeTimeDuration = System.TimeSpan.FromMinutes(LifeTimeDuration),
                    PaymentTypeId = PaymentTypeId,
                    Value = Value,
                    Paid = false

                };

                db.Invoice.Add(invoice);
                db.SaveChanges();
                return invoice;
            }

            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Создаем номер счета на опталу. Берем последний номер и прибавляем один
        /// </summary>
        /// <returns></returns>
        private int GenerateInvoiceNumber()
        {
            try
            {
                var last = db.Invoice.OrderByDescending(i => i.Id).FirstOrDefault();

                if (last != null)
                    return Convert.ToInt32(last.InvoiceNumber) + 1;

                else
                    return 1;
            }
            catch
            {
                return 1;
            }

        }
    }
}
