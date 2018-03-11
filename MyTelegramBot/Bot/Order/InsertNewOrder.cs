using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.BusinessLayer;

namespace MyTelegramBot.Bot.Order
{
    public class InsertNewOrder
    {
        private int FollowerId {get;set;}

        private BotInfo BotInfo { get; set; }

        private MarketBotDbContext db { get; set; }

        private List<Basket> Basket { get; set; }

        private OrderTemp OrderTmp { get; set; }


        private Services.ICryptoCurrency CryptoCurrency { get; set; }

        private Currency Currency { get; set; }

        private Invoice Invoice { get; set; }

        private PaymentTypeConfig PaymentConfig { get; set; }

        private OrderFunction OrderFunction { get; set; }

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

            OrderFunction = new OrderFunction();

            Basket = db.Basket.Where(b => b.FollowerId == FollowerId && b.Enable && b.BotInfoId == BotInfo.Id).Include(b=>b.Product.CurrentPrice).ToList();
            OrderTmp = db.OrderTemp.Where(o => o.FollowerId == FollowerId && o.BotInfoId == BotInfo.Id).FirstOrDefault();
            var LastOrder = db.Orders.OrderByDescending(o => o.Id).FirstOrDefault();

            //Общая строимость корзины
            total = BusinessLayer.BasketFunction.BasketTotalPrice(Basket);

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



                if (Invoice!=null) // проверяем создался ли инвойс. Если нет то у пользователя будет способ оплаты при получении
                    NewOrder.InvoiceId = Invoice.Id;

                db.Orders.Add(NewOrder);
                db.SaveChanges();


                // добавляем инф. о доставке в БД
                if (OrderTmp!=null)
                    AddAddressToOrder(NewOrder.Id, OrderTmp.AddressId, ShipPrice);

                // переносим из корзины в Состав заказа
                NewOrder.OrderProduct= BusinessLayer.BasketFunction.FromBasketToOrderPosition(NewOrder.Id, Basket);

                var CurrentStarus= OrderFunction.InsertOrderStatus(NewOrder.Id,FollowerId);

                NewOrder =OrderFunction.UpdCurrentStatus(NewOrder,CurrentStarus);

                OrderFunction.RemoveOrderTmp(FollowerId,BotInfo.Id);

                db.SaveChanges();

                db.Dispose();

                return NewOrder;
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


    }
}
