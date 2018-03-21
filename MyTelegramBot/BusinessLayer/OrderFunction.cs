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

        /// <summary>
        /// кто обрабатывает заявку в данные момент
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public static OrdersInWork WhoItWorkNow(int OrderId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return db.OrdersInWork.Where(o => o.OrderId == OrderId).Include(o=>o.Follower).LastOrDefault();
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


        public static OrdersInWork InsertOrderInWork (int OrderId,int FollowerId,bool InWork=true)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                if (OrderId > 0 && FollowerId > 0)
                {
                    OrdersInWork ordersInWork = new OrdersInWork
                    {
                        FollowerId = FollowerId,
                        InWork = InWork,
                        OrderId = OrderId,
                        Timestamp = DateTime.Now
                    };

                    db.OrdersInWork.Add(ordersInWork);
                    db.SaveChanges();
                    return ordersInWork;
                }

                else
                    return null;
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

        public static OrderAddress GetAddress(int OrderId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return  db.OrderAddress.Where(o => o.OrderId == OrderId).Include(o => o.Adress.House).FirstOrDefault();
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


        public static Orders GetOrder(int OrderId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return db.Orders.Where(o => o.Id == OrderId).
                            Include(o => o.OrderProduct).
                            Include(o => o.Follower).
                            Include(o => o.FeedBack).
                            Include(o => o.OrderAddress).
                            Include(o => o.CurrentStatusNavigation.Status).
                            Include(o=>o.PickupPoint).
                            Include(o => o.Invoice).
                            Include(o => o.OrdersInWork).FirstOrDefault();

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


        /// <summary>
        /// удалить статус заказа
        /// </summary>
        /// <param name="OrderStatusId"></param>
        /// <returns></returns>
        public static int RemoveStatus(int OrderStatusId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                db.OrderStatus.Remove(db.OrderStatus.Find(OrderStatusId));
                return db.SaveChanges();
            }

            catch
            {
                return -1;
            }

            finally
            {
                db.Dispose();
            }
        }


        public static List<Orders> GetAllOrders()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                List<Orders> list = db.Orders.OrderByDescending(o => o.Id).Include(o => o.Follower).Include(o => o.CurrentStatusNavigation.Status).
                                    Include(o => o.FeedBack).Include(o => o.Invoice.PaymentType).Include(o => o.OrderAddress).Include(o => o.PickupPoint).
                                    Include(o => o.OrderProduct).ToList();

                foreach (var order in list)
                {
                    order.OrderProduct = db.OrderProduct.Where(o => o.OrderId == order.Id).Include(o => o.Product).Include(o => o.Price).ToList();

                    if (order.OrderAddress != null)
                        order.OrderAddress.Adress = db.Address.Where(a => a.Id == order.OrderAddress.AdressId).Include(o => o.House.Street.City).FirstOrDefault();
                }

                return list;

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

        public static List<OrderStatus> GetAllHistoryStatus()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var list= db.OrderStatus.Where(o=>o.Enable).Include(o=>o.Status)
                    .Include(o => o.Follower).OrderByDescending(o=>o.Id).ToList();

                foreach (var status in list)
                    status.Orders.Add(db.Orders.Find(status.OrderId));

                return list;
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

        /// <summary>
        /// Подтвердить добавленный статус. 
        /// </summary>
        /// <param name="OrderStatusId"></param>
        /// <returns></returns>
        public static OrderStatus ConfirmOrderStatus(int OrderStatusId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var status = db.OrderStatus.Find(OrderStatusId);
                var order = db.Orders.Find(status.OrderId);
                if (status != null && order != null)
                {
                    status.Enable = true;
                    status.Timestamp = DateTime.Now;
                    db.Update<OrderStatus>(status);
                    db.SaveChanges();

                    order.CurrentStatus = status.Id;
                    db.Update<Orders>(order);
                    db.SaveChanges();

                    return status;
                }

                else
                    return null;
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


        public static OrderStatus InsertOrderStarus(int OrderId,int StatusId, int FollowerId, bool Enable=true, string Comment="")
        {
            MarketBotDbContext db = new MarketBotDbContext();
            try
            {
                OrderStatus orderStatus = new OrderStatus
                {
                    Enable = Enable,
                    FollowerId = FollowerId,
                    OrderId = OrderId,
                    StatusId = StatusId,
                    Timestamp = DateTime.Now,
                    Text = Comment
                };

                db.OrderStatus.Add(orderStatus);
                db.SaveChanges();
                return orderStatus;
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


        public static OrderStatus AddCommentToStatus(int OrderStatus, string Comment)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var status = db.OrderStatus.Find(OrderStatus);

                if (status != null)
                {
                    status.Text = Comment;
                    db.Update<OrderStatus>(status);
                    db.SaveChanges();
                    
                }

                return status;
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


        public Orders CreateOrder(int FollowerId, BotInfo botInfo)
        {
            InvoiceFunction = new InvoiceFunction();

            //содержимое корзины
            var Basket = db.Basket.Where(b => b.FollowerId == FollowerId && b.Enable && b.BotInfoId == botInfo.Id).Include(b => b.Product.CurrentPrice).ToList();

            var OrderTmp = db.OrderTemp.Where(o => o.FollowerId == FollowerId && o.BotInfoId == botInfo.Id).Include(o => o.PickupPoint).LastOrDefault();

            int LastOrderNumber = 0;

            if(db.Orders.LastOrDefault()!=null)
                LastOrderNumber=Convert.ToInt32(db.Orders.LastOrDefault().Number);

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
                    var Order = InsertOrder(OrderTmp, LastOrderNumber+1);

                    var CurrentStatus= InsertOrderStatus(Order.Id, FollowerId);

                    var Invoice = InvoiceFunction.AddInvoice(Order, Convert.ToInt32(OrderTmp.PaymentTypeId), TotalPrice);

                    InvoiceFunction.Dispose();

                    if (Invoice!=null)
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



        private Orders InsertOrder(OrderTemp orderTemp,int OrderNumber)
        {
            Orders Order = new Orders
            {
                DateAdd = DateTime.Now,
                FollowerId = Convert.ToInt32(orderTemp.FollowerId),
                Text = orderTemp.Text,
                Number = OrderNumber,
                Paid = false,
                BotInfoId = orderTemp.BotInfoId,
                PickupPoint=orderTemp.PickupPoint,
                StockUpdate=false
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

            if (PaymentTypeCfg != null && PaymentTypeCfg.PaymentId == ConstantVariable.PaymentTypeVariable.QIWI ||
               PaymentTypeCfg != null && PaymentTypeCfg.PaymentId == ConstantVariable.PaymentTypeVariable.DebitCardForYandexKassa)
                return true;

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
                        .Include(o => o.FeedBack)
                        .Include(o => o.OrderProduct)
                        .Include(o => o.OrderAddress)
                        .Include(o => o.PickupPoint)
                        .Include(o => o.BotInfo)
                        .Include(o => o.Invoice)
                        .Include(o=>o.CurrentStatusNavigation)
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
            Payment payment = null;

            try
            {
                 if(Order!=null && Order.Paid==false) // заказ еще не оплачен, значит добавляем платеж
                    payment = InsertPayment(Convert.ToInt32(Order.InvoiceId), Summ, TxId, Comment);

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
                    OrderTmp = InsertOrderTmp(FollowerId, BotId, PickupPointId:PickUpPointId);
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
                    OrderTmp = InsertOrderTmp(FollowerId, BotId, AddressId:AddressId);
                else
                {
                    OrderTmp.PickupPointId = null;
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
                    OrderTmp = InsertOrderTmp(FollowerId, BotId, PaymentTypeId:PaymentTypeId);

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



        public OrderTemp InsertOrderTmp(int FollowerId, int BotId,int? PickupPointId=null,int? AddressId=null,int? PaymentTypeId=null)
        {
            try
            {
                OrderTemp orderTemp = new OrderTemp
                {
                    FollowerId = FollowerId,
                    BotInfoId = BotId,
                    PickupPointId= PickupPointId,
                    AddressId= AddressId,
                    PaymentTypeId= PaymentTypeId

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
