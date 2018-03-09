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
    public class OrderFunction
    {
        private MarketBotDbContext db { get; set; }

        private InvoiceFunction InvoiceFunction { get; set; }

        public OrderFunction()
        {
            OpenConnection();
        }

        private void OpenConnection()
        {
            db = new MarketBotDbContext();
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public Orders CreateOrder(int FollowerId, BotInfo botInfo)
        {
            InvoiceFunction = new InvoiceFunction();

            //содержимое корзины
            var Basket = db.Basket.Where(b => b.FollowerId == FollowerId && b.Enable && b.BotInfoId == botInfo.Id).Include(b => b.Product.CurrentPrice).ToList();

            var OrderTmp = db.OrderTemp.Where(o => o.FollowerId == FollowerId && o.BotInfoId == botInfo.Id).Include(o => o.PickupPoint).LastOrDefault();

            int LastOrderNumber =Convert.ToInt32(db.Orders.LastOrDefault().Number);

            double ShipPrice = 0.0;

            //Общая строимость корзины
            double TotalPrice = BasketFunction.BasketTotalPrice(Basket);

            if (OrderTmp != null && OrderTmp.AddressId > 0 && botInfo!=null &&botInfo.Configuration!=null) // Высчитываем стоимость доставки и пересчитываем общую стоиость
            {
                ShipPrice = CalcShipPrice(TotalPrice, botInfo.Configuration.ShipPrice, botInfo.Configuration.FreeShipPrice);
                TotalPrice += ShipPrice;
            }

            if (TotalPrice > 0 && Basket.Count > 0 && OrderTmp != null)
            {
                try
                {
                    var Order = InsertOrder(OrderTmp, LastOrderNumber);

                    var CurrentStatus= InsertOrderStatus(Order.Id, FollowerId);

                    var Invoice = InvoiceFunction.AddInvoice(Order, Convert.ToInt32(OrderTmp.PaymentTypeId), TotalPrice);

                    if(Invoice!=null)
                        Order.InvoiceId = Invoice.Id;

                    if(CurrentStatus!=null)
                        Order.CurrentStatus = CurrentStatus.Id;

                    db.Update<Orders>(Order);

                    db.SaveChanges();

                    if (OrderTmp.AddressId > 0)
                        AddAddressToOrder(Order.Id, OrderTmp.AddressId, ShipPrice);

                    RemoveOrderTmp(FollowerId, botInfo.Id);
                    var Position = BasketFunction.FromBasketToOrderPosition(Order.Id, Basket);
                    Order.OrderProduct = Position;
                    Order.Invoice = Invoice;
                    Order.CurrentStatusNavigation = CurrentStatus;

                    return Order;
                }
                catch (Exception e)
                {
                    return null;
                }
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
        private int AddAddressToOrder(int OrderId, int? AddressId, double ShipPrice = 0)
        {
            if (OrderId > 0 && AddressId > 0)
            {
                db.OrderAddress.Add(
                new OrderAddress
                {
                    OrderId = OrderId,
                    AdressId = Convert.ToInt32(AddressId),
                    ShipPriceValue = ShipPrice
                });

                return db.SaveChanges();
                 

            }

            else
                return -1;
        }

        /// <summary>
        /// Стоимость доставки
        /// </summary>
        /// <param name="BasketTotalPrice">стоимость всего заказа</param>
        /// <param name="ShipPrice">стоимость доставки</param>
        /// <param name="FreeShipPrice">стоимость заказа при которой доставка бесплатная</param>
        /// <returns></returns>
        private double CalcShipPrice(double BasketTotalPrice, double ShipPrice, double FreeShipPrice)
        {
            if (FreeShipPrice!=0 && BasketTotalPrice >= FreeShipPrice)
                return 0;

            if (FreeShipPrice == 0 && ShipPrice == 0)
                return 0;

            if (FreeShipPrice != 0 && BasketTotalPrice < FreeShipPrice)
                return ShipPrice;

            if (FreeShipPrice == 0 && ShipPrice > 0)
                return ShipPrice;

            else
                return 0;
        }

        private Orders InsertOrder(OrderTemp orderTemp,int LastOrderNumber)
        {
            Orders Order = new Orders
            {
                DateAdd = DateTime.Now,
                FollowerId = Convert.ToInt32(orderTemp.FollowerId),
                Text = orderTemp.Text,
                Number = LastOrderNumber + 1,
                Paid = false,
                BotInfoId = orderTemp.BotInfoId,
                PickupPoint=orderTemp.PickupPoint
            };

            db.Orders.Add(Order);
            db.SaveChanges();
            return Order;
        }

        /// <summary>
        /// Обновить текущий статус заказ
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderStatus"></param>
        /// <returns></returns>
        public Orders UpdCurrentStatus(Orders order, OrderStatus orderStatus)
        {
            try
            {
                if (order != null && orderStatus != null)
                {
                    order.CurrentStatusNavigation = orderStatus;
                    db.Update<Orders>(order);
                    db.SaveChanges();
                    return order;
                }

                else
                    return null;
            }

            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Добавить запись в таблицу OrderStatus
        /// </summary>
        /// <param name="OrderId"></param>
        /// <param name="FollowerId"></param>
        /// <param name="StatusId"></param>
        /// <param name="Enable"></param>
        /// <param name="Comment"></param>
        /// <returns></returns>
        public OrderStatus InsertOrderStatus(int OrderId,int FollowerId ,int StatusId=ConstantVariable.OrderStatusVariable.PendingProcessing,bool Enable=true ,string Comment="")
        {
            try
            {
                OrderStatus orderStatus = new OrderStatus
                {
                    OrderId = OrderId,
                    StatusId = StatusId,
                    Timestamp = DateTime.Now,
                    Enable = Enable,
                    FollowerId = FollowerId
                };

                db.OrderStatus.Add(orderStatus);
                db.SaveChanges();
                return orderStatus;
            }

            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Удалить временные данные
        /// </summary>
        /// <param name="FollowerId"></param>
        /// <param name="BotId"></param>
        /// <returns></returns>
        public int RemoveOrderTmp(int FollowerId, int BotId)
        {
            try
            {
                var list = db.OrderTemp.Where(o => o.FollowerId == FollowerId && o.BotInfoId == BotId).ToList();

                foreach (var ordertmp in list)
                    db.OrderTemp.Remove(ordertmp);

                return db.SaveChanges();
            }

            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Доступен ли способ оплаты
        /// </summary>
        /// <param name="PaymentTypeId"></param>
        /// <returns></returns>
        public bool PaymentTypeTestConnection(int PaymentTypeId)
        {
            ICryptoCurrency cryptoCurrency;

            //способ оплаты при получении. не трубует проверки
            if (ConstantVariable.PaymentTypeVariable.PaymentOnReceipt == PaymentTypeId)
                return true;

            //есть ли настройки для выбранного метода оплаты
            var PaymentTypeCfg = db.PaymentTypeConfig.Where(p => p.PaymentId == PaymentTypeId).LastOrDefault();

            if (PaymentTypeCfg == null)
                return false;

            if (ConstantVariable.PaymentTypeVariable.Bitcoin == PaymentTypeId ||
               ConstantVariable.PaymentTypeVariable.BitcoinCash == PaymentTypeId ||
               ConstantVariable.PaymentTypeVariable.Litecoin == PaymentTypeId ||
               ConstantVariable.PaymentTypeVariable.Doge == PaymentTypeId)
            {
                if (PaymentTypeCfg != null)
                {
                    cryptoCurrency = new BitCoin(PaymentTypeCfg.Login, PaymentTypeCfg.Pass, PaymentTypeCfg.Host, PaymentTypeCfg.Port);

                    //подключаемся к ноде и вытаскиваем инфу
                    var block = cryptoCurrency.GetInfo<Services.BitCoinCore.GetInfo>();

                    if (block != null && block.result != null && block.result.blocks > 0)
                        return true;

                    else
                        return false;
                }

                else
                    return false;
            }

            else
                return false;
        }

        /// <summary>
        /// Приндалежит ли заказ пользователю если нет вернут null
        /// </summary>
        /// <param name="OrderId"></param>
        /// <param name="FollowerId"></param>
        /// <returns></returns>
        public Orders GetFollowerOrder(int OrderNumber, int FollowerId)
        {
            try
            {
                return db.Orders.Where(o => o.Number == OrderNumber && o.FollowerId == FollowerId)
                        .Include(o => o.Confirm)
                        .Include(o => o.Delete).Include(o => o.Done)
                        .Include(o => o.FeedBack)
                        .Include(o => o.OrderProduct)
                        .Include(o => o.OrderAddress)
                        .Include(o => o.PickupPoint)
                        .Include(o => o.BotInfo)
                        .Include(o => o.Invoice)
                        .FirstOrDefault();

            }

            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Добавить платеж кридитной картой и поменить заказ и счет как оплаченные
        /// </summary>
        /// <param name="OrderId"></param>
        /// <param name="Summ"></param>
        /// <param name="TxId"></param>
        /// <param name="Comment"></param>
        /// <returns></returns>
        public Payment AddCreditCardPayment(int OrderId, double Summ, string TxId = "", string Comment = "")
        {
            var Order = db.Orders.Find(OrderId);
            try
            {
                var payment = InsertPayment(1, Summ, TxId, Comment);

                if (payment != null)
                {
                    InvoicePaid(Convert.ToInt32(payment.InvoiceId));
                    Order.Paid = true;
                    db.Update<Orders>(Order);
                    db.SaveChanges();
                    return payment;
                }

                else
                    return null;
            }

            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Добавить пункт самовывоза  к еще не офрмленному закаку
        /// </summary>
        /// <param name="FollowerId"></param>
        /// <param name="BotId"></param>
        /// <param name="AddressId"></param>
        /// <returns></returns>
        public OrderTemp AddPickUpPointToOrderTmp(int FollowerId, int BotId, int PickUpPointId)
        {
            try
            {
                var OrderTmp = db.OrderTemp.Where(o => o.FollowerId == FollowerId && o.BotInfoId == BotId).LastOrDefault();

                if (OrderTmp == null)
                    OrderTmp = InsertOrderTmp(FollowerId, BotId);
                else
                {
                    OrderTmp.PickupPointId = PickUpPointId;
                    OrderTmp.AddressId = null;
                    db.Update<OrderTemp>(OrderTmp);
                    db.SaveChanges();
                }

                return OrderTmp;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Добавить адрес к заказу
        /// </summary>
        /// <param name="FollowerId"></param>
        /// <param name="BotId"></param>
        /// <param name="AddressId"></param>
        /// <returns></returns>
        public OrderTemp AddAddressToOrderTmp(int FollowerId, int BotId, int AddressId)
        {
            try
            {
                var OrderTmp = db.OrderTemp.Where(o => o.FollowerId == FollowerId && o.BotInfoId == BotId).LastOrDefault();

                if (OrderTmp == null)
                    OrderTmp = InsertOrderTmp(FollowerId, BotId);
                else
                {
                    OrderTmp.PickupPoint = null;
                    OrderTmp.AddressId = AddressId;
                    db.Update<OrderTemp>(OrderTmp);
                    db.SaveChanges();
                }

                return OrderTmp;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Добавить способо оплаты к заказу
        /// </summary>
        /// <param name="FollowerId"></param>
        /// <param name="BotId"></param>
        /// <param name="PaymentTypeId"></param>
        /// <returns></returns>
        public OrderTemp AddPaymentMethodToOrderTmp(int FollowerId, int BotId, int PaymentTypeId)
        {
            try
            {
                var OrderTmp = db.OrderTemp.Where(o => o.FollowerId == FollowerId && o.BotInfoId == BotId).LastOrDefault();

                if (OrderTmp == null)
                    OrderTmp = InsertOrderTmp(FollowerId, BotId);

                else
                {
                    OrderTmp.PaymentTypeId = PaymentTypeId;
                    db.Update<OrderTemp>(OrderTmp);
                    db.SaveChanges();
                }

                return OrderTmp;
            }

            catch
            {
                return null;
            }
        } 

        /// <summary>
        /// Добавить комментарий к еще не оформленому заказу
        /// </summary>
        /// <param name="FollowerId"></param>
        /// <param name="BotId"></param>
        /// <param name="Text"></param>
        /// <returns></returns>
        public OrderTemp AddCommentToOrderTmp(int FollowerId, int BotId, string Text)
        {
            try
            {
                var OrderTmp = db.OrderTemp.Where(o => o.FollowerId == FollowerId && o.BotInfoId == BotId).LastOrDefault();

                if (OrderTmp == null)
                    OrderTmp = InsertOrderTmp(FollowerId, BotId);

                else
                {
                    OrderTmp.Text = Text;
                    db.Update<OrderTemp>(OrderTmp);
                    db.SaveChanges();
                }
                return OrderTmp;
            }

            catch
            {
                return null;
            }
        }

        public OrderTemp InsertOrderTmp(int FollowerId, int BotId)
        {
            try
            {
                OrderTemp orderTemp = new OrderTemp
                {
                    FollowerId = FollowerId,
                    BotInfoId = BotId,

                };

                db.OrderTemp.Add(orderTemp);
                db.SaveChanges();
                return orderTemp;
            }
            catch
            {
                return null;
            }
        }

        private Payment InsertPayment(int InvoiceId,double Summ, string TxId="", string Comment="")
        {
            try
            {
                Payment payment = new Payment
                {
                    InvoiceId = InvoiceId,
                    Comment = Comment,
                    TimestampDataAdd = DateTime.Now,
                    Summ = Summ,
                    TxId = TxId
                };

                db.Payment.Add(payment);
                db.SaveChanges();
                return payment;
            }

            catch
            {
                return null;
            }

            finally
            {

            }
        }

        private Invoice InvoicePaid(int InvoiceId)
        {
            try
            {
                var invoice = db.Invoice.Find(InvoiceId);
                invoice.Paid = true;
                db.Update<Invoice>(invoice);
                db.SaveChanges();
                return invoice;
            }

            catch
            {
                return null;
            }
        }
    }
}
