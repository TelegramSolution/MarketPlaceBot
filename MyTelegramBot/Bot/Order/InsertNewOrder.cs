using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.Bot.Order
{
    public class InsertNewOrder
    {
        private int FollowerId {get;set;}

        private BotInfo BotInfo { get; set; }

        private MarketBotDbContext db { get; set; }

        private List<IGrouping<int, Basket>> Basket { get; set; }

        private OrderTemp OrderTmp { get; set; }

        private Services.PaymentTypeEnum PaymentTypeEnum { get; set; }

        private Services.ICryptoCurrency CryptoCurrency { get; set; }

        private Currency Currency { get; set; }

        private Invoice Invoice { get; set; }

        private PaymentTypeConfig PaymentConfig { get; set; }

        public InsertNewOrder(int FollowerId, BotInfo BotInfo)
        {
            this.FollowerId = FollowerId;
            this.BotInfo = BotInfo;
            db = new MarketBotDbContext();



        }
        public Orders AddOrder()
        {
            double total = 0.0;
            double ShipPrice =0;
            int Number = 0;

            Basket = db.Basket.Where(b => b.FollowerId == FollowerId && b.Enable && b.BotInfoId == BotInfo.Id).Include(b=>b.Product).GroupBy(b => b.ProductId).ToList();
            OrderTmp = db.OrderTemp.Where(o => o.FollowerId == FollowerId && o.BotInfoId == BotInfo.Id).FirstOrDefault();
            PaymentTypeEnum = PaymentType.GetPaymentTypeEnum(OrderTmp.PaymentTypeId);
            var LastOrder = db.Orders.OrderByDescending(o => o.Id).FirstOrDefault();

            //Общая строимость корзины
            total = BasketTotalPrice(Basket);

            if (LastOrder != null)  // Узнаем последний номер заказа в БД
                Number =Convert.ToInt32(LastOrder.Number);

            if (OrderTmp != null && OrderTmp.PaymentTypeId != null && Basket.Count>0)
            {


                Orders NewOrder = new Orders
                {
                    DateAdd = DateTime.Now,
                    FollowerId = FollowerId,
                    Text = OrderTmp.Text,
                    Number = Number + 1,
                    Paid = false,
                    BotInfoId = BotInfo.Id,

                };

                if (OrderTmp.PickupPointId != null)// самовывоз
                    NewOrder.PickupPointId = OrderTmp.PickupPointId;

                // Определям стоимость заказа с учетом доставки 
                // Заказ НЕ подходит под условия бесплатной доставки. Доставка платная
                if (OrderTmp.AddressId != null && BotInfo.Configuration.ShipPrice > 0 && total < BotInfo.Configuration.FreeShipPrice)
                {
                    ShipPrice = BotInfo.Configuration.ShipPrice;
                    total += BotInfo.Configuration.ShipPrice;
                }

                // создаем инвойс для оплаты в через КИВИ
                if (PaymentTypeEnum == Services.PaymentTypeEnum.Qiwi)
                    Invoice= AddQiwiInvoice(NewOrder, total);

                // создаем инвойс для оплаты в криптовалюте
                if (PaymentTypeEnum != Services.PaymentTypeEnum.PaymentOnReceipt && PaymentTypeEnum != Services.PaymentTypeEnum.Qiwi) 
                    Invoice= AddCryptoCurrencyInvoice(NewOrder, PaymentTypeEnum, total);

                if(Invoice!=null)
                    NewOrder.InvoiceId = Invoice.Id;

                db.Orders.Add(NewOrder);
                db.SaveChanges();


                // добавляем инф. о доставке в БД
                if (OrderTmp!=null)
                    AddAddressToOrder(NewOrder.Id, OrderTmp.AddressId, ShipPrice);

                // переносим из корзины в Состав заказа
                foreach (var group in Basket)
                    NewOrder.OrderProduct.Concat(FromBasketToOrderPosition(group.ElementAt(0).ProductId, NewOrder.Id, group));


                var CurrentStarus= InsertOrderStatus(NewOrder.Id);
                NewOrder.CurrentStatusNavigation = CurrentStarus;
                db.SaveChanges();
                db.Dispose();
                return NewOrder;
            }

            else
                return null;
            
        }

        /// <summary>
        /// Общая стоимость всех товаров в корзине
        /// </summary>
        /// <param name="Basket"></param>
        /// <returns></returns>
        private double BasketTotalPrice(List<IGrouping<int, Basket>> Basket)
        {
            double total = 0.0;
            foreach (var list in Basket) // Общая стоимость корзины
            {
                foreach (var position in list)
                {
                    var price = db.ProductPrice.Where(p => p.ProductId == position.ProductId && p.Enabled).OrderByDescending(p => p.Id).Include(p => p.Currency).FirstOrDefault();
                    if (price != null)
                    {
                        total += price.Value;
                        Currency = price.Currency;
                    }
                }
            }

            return total;
        }

        /// <summary>
        /// Перенести данные из таблицы Basket в таблицу ORderProduct
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="OrderId"></param>
        /// <param name="group">Группа записей одного товара из таблицы  Basket</param>
        /// <returns></returns>
        private List<OrderProduct> FromBasketToOrderPosition(int ProductId, int OrderId, IGrouping<int, Basket> group)
        {
            ProductPrice price = db.ProductPrice.Where(p => p.ProductId == ProductId && p.Enabled).Include(p=>p.Currency).FirstOrDefault();

            List<OrderProduct> list = new List<OrderProduct>();

            if (price != null)
            {
                foreach (var product in group)
                {
                    OrderProduct orderProduct = new OrderProduct
                    {
                        ProductId = ProductId,
                        OrderId = OrderId,
                        DateAdd = DateTime.Now,
                        Count = 1,
                        PriceId = price.Id,

                    };

                    db.Basket.Remove(product); // удаляем из корзины

                    db.OrderProduct.Add(orderProduct); // вставляем в заказ

                    db.SaveChanges();

                    orderProduct.Price = price;

                    list.Add(orderProduct);
                }

                return list;
            }

            else
                return null;

            
        }

        /// <summary>
        /// Создать счет на оплату для Киви
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <param name="PaymentType">Тип оплаты. Киви</param>
        /// <param name="Total">Сумма в рублях</param>
        /// <param name="LifeTimeDuration">Время жизни счета</param>
        /// <returns></returns>
        private Invoice AddQiwiInvoice(Orders order,double Total, int LifeTimeDuration=60)
        {
            var ListQiwi = db.PaymentTypeConfig.Where(q => q.PaymentId == PaymentType.GetTypeId(Services.PaymentTypeEnum.Qiwi) && q.Enable == true).
                OrderByDescending(q => q.Id).ToList();

            Random random = new Random();

            var qiwi = ListQiwi[random.Next(0, ListQiwi.Count - 1)];

            if (qiwi != null && qiwi.Login!=null)
            {
                Invoice invoice = new Invoice
                {
                    CreateTimestamp = DateTime.Now,
                    AccountNumber = qiwi.Login,
                    Comment = GeneralFunction.BuildPaymentComment(BotInfo.Name, order.Number.ToString()),
                    InvoiceNumber = GenerateInvoiceNumber(),
                    LifeTimeDuration = System.TimeSpan.FromMinutes(LifeTimeDuration),
                    PaymentTypeId = PaymentType.GetTypeId(Services.PaymentTypeEnum.Qiwi),
                    Value = Total,
                    Paid=false

                };

                db.Invoice.Add(invoice);

                if (db.SaveChanges() > 0)
                    return invoice;

                else
                    return null;
            }

            else
                return null;
        }


        /// <summary>
        /// Создать счет на оплату в Криптовалюте
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <param name="paymentTypeEnum">Тип платежа. Лайткоин, БиткоинКэш и т.д</param>
        /// <param name="Total">Сумма в фиате.</param>
        /// <param name="LifeTimeDuration">Время жизни счета в минутах</param>
        /// <returns></returns>
        private Invoice AddCryptoCurrencyInvoice (Orders order, Services.PaymentTypeEnum paymentTypeEnum, double Total, int LifeTimeDuration = 60)
        {
            double Summa = 0.0;

            var type = db.PaymentType.Where(p => p.Id == PaymentType.GetTypeId(paymentTypeEnum)).FirstOrDefault();

            PaymentConfig = db.PaymentTypeConfig.Where(p => p.PaymentId == PaymentType.GetTypeId(paymentTypeEnum) && p.Enable==true).OrderByDescending(p => p.Id).FirstOrDefault();

            if (PaymentConfig != null)
                    CryptoCurrency = new Services.BitCoinCore.BitCoin(PaymentConfig.Login, PaymentConfig.Pass, PaymentConfig.Host, PaymentConfig.Port);                                          

            if (type != null && Currency != null) // конвертируем из фиата в крипту
                Summa = MoneyConvert(Total, type.Code, Currency.Code);

            string AccountNumber = CryptoCurrency.GetNewAddress(); // Генерируем адрес куда необходимо перевести деньги

            if (type!=null && CryptoCurrency != null && AccountNumber!=null && AccountNumber!="" && Summa>0)
            {
                Invoice invoice = new Invoice
                {
                    AccountNumber = AccountNumber,
                    CreateTimestamp = DateTime.Now,
                    InvoiceNumber = GenerateInvoiceNumber(),
                    LifeTimeDuration = System.TimeSpan.FromMinutes(LifeTimeDuration),
                    PaymentTypeId = type.Id,
                    Value = Summa,
                    Comment="-",
                    Paid=false
                };

                db.Invoice.Add(invoice);
                db.SaveChanges();
                return invoice;
          
            }

            else
                return null;
        }

        /// <summary>
        /// Добавить адрес доставки к заказу
        /// </summary>
        /// <param name="OrderId"></param>
        /// <param name="AddressId"></param>
        /// <returns></returns>
        private int AddAddressToOrder(int OrderId, int? AddressId, double ShipPrice=0)
        {
            if (OrderId > 0 && AddressId > 0)
            {
                db.OrderAddress.Add(
                new OrderAddress
                {
                    OrderId = OrderId,
                    AdressId = Convert.ToInt32(AddressId),
                    ShipPriceValue=ShipPrice
                });

                return db.SaveChanges();

            }

            else
                return -1;
        }


        /// <summary>
        /// Удалить заказ из временной таблицы OrderTemp
        /// </summary>
        /// <param name="orderTemp"></param>
        /// <returns></returns>
        private int DeleteTempOrder(OrderTemp orderTemp)
        {
            db.OrderTemp.Remove(orderTemp);
            return db.SaveChanges();
        }

        /// <summary>
        /// Создаем номер счета на опталу
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
        /// <summary>
        /// номер телефона киви для платежа
        /// </summary>
        /// <returns></returns>
        private string GetTelephoneQiwi()
        {
            var qiwi = db.PaymentTypeConfig.Where(q=>q.PaymentId==PaymentType.GetTypeId(Services.PaymentTypeEnum.Qiwi) && q.Enable == true).OrderByDescending(q=>q.Id).FirstOrDefault();

            if (qiwi != null)
                return qiwi.Login;

            else
                return null;
        }

        private OrderStatus InsertOrderStatus(int OrderId,int StatusId = 1)
        {
            OrderStatus orderStatus = new OrderStatus
            {
                OrderId = OrderId,
                StatusId = StatusId,
                Timestamp = DateTime.Now,
                Enable=true,
                FollowerId=FollowerId
            };

            db.OrderStatus.Add(orderStatus);
            db.SaveChanges();
            return orderStatus;
        }

        /// <summary>
        /// Конвертируем из фиата в крипту
        /// </summary>
        /// <param name="value">Сумма денег в фиате</param>
        /// <param name="bases">В какую валюту конвертируем (LTC например)</param>
        /// <param name="target">Что конвертируем (RUR- рубль например)</param>
        /// <returns></returns>
        private double MoneyConvert(double value, string bases, string target="RUR")
        {
            var Cryptonator = Services.CryptonatorConvert.GetCryptonator(bases, target);

            try
            {
                if (Cryptonator != null && Cryptonator.ticker != null && value > 0)
                {
                    double price=Convert.ToDouble(Cryptonator.ticker.price.Replace('.',','));
                    double convert = value / price;
                    return Math.Round(convert,6);
                }


                else
                    return -1;
            }
            catch (Exception e)
            {
                return -1;
            }

        }
    }
}
