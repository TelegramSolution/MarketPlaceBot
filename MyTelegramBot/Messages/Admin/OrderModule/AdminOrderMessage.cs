using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;
using MyTelegramBot.Bot.AdminModule;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// Сообщение с описанием заказа. Админка
    /// </summary>
    public class AdminOrderMessage:BotMessage
    {
        private InlineKeyboardCallbackButton EditOrderPositionBtn { get; set; }

        private InlineKeyboardCallbackButton ViewTelephoneNumberBtn { get; set; }

        /// <summary>
        /// Кнопка взять заказ в обработку
        /// </summary>
        private InlineKeyboardCallbackButton TakeOrderBtn { get; set; }

        private InlineKeyboardCallbackButton ViewAddressOnMapBtn { get; set; }

        private InlineKeyboardCallbackButton ViewInvoiceBtn { get; set; }

        /// <summary>
        /// кнопка изменить статус заказа
        /// </summary>
        private InlineKeyboardCallbackButton EditStatusBtn { get; set; }

        /// <summary>
        /// кнопка посмотреть историю изменения статусов
        /// </summary>
        private InlineKeyboardCallbackButton StatusHistoryBtn { get; set; }

        /// <summary>
        /// Освободить заявку
        /// </summary>
        private InlineKeyboardCallbackButton FreeOrderBtn { get; set; }
      
        private List<Payment> Payments { get; set; }

        private Orders Order { get; set; }

        private int OrderId { get; set; }

        private Follower Follower { get; set; }

        private string PaymentMethodName { get; set; }

        /// <summary>
        /// Какому пользователю будет отсылать сообщение
        /// </summary>
        private int FollowerId { get; set; }

        /// <summary>
        /// У какого пользователя заявка сейчас находится в обработке
        /// </summary>
        private int InWorkFollowerId { get; set; }


        private Address Address { get; set; }

        public AdminOrderMessage(int OrderId, int FollowerId = 0)
        {
            this.OrderId = OrderId;
            this.FollowerId = FollowerId;
        }

        public AdminOrderMessage (Orders order, int FollowerId=0)
        {
            this.Order = order;
            this.FollowerId = FollowerId;
        }

        public override BotMessage BuildMsg()
        {
            double total = 0.0;
            double ShipPrice = 0;
            string Position = "";
            string Paid = "";

            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                if(this.Order==null && this.OrderId>0)
                Order = db.Orders.Where(o => o.Id == OrderId).
                    Include(o=>o.FeedBack).
                    Include(o => o.OrderProduct).
                    Include(o => o.OrderAddress).
                    Include(o=>o.OrdersInWork).
                    Include(o=>o.Invoice).
                    Include(o=>o.Confirm).
                    Include(o => o.Delete).
                    Include(o => o.Done).
                    Include(o=>o.PickupPoint).
                    Include(o=>o.CurrentStatusNavigation).
                    FirstOrDefault();

                ///////////Провереряем какой способ оплаты ///////////
                if (Order.Invoice != null)
                    PaymentMethodName = db.PaymentType.Find(Order.Invoice.PaymentTypeId).Name;

                    else
                        PaymentMethodName = "При получении";

                //Вытаскиваем полный адрес доставки
                if (Order != null && Order.OrderAddress == null)
                    Order.OrderAddress = db.OrderAddress.Where(o => o.OrderId == Order.Id).FirstOrDefault();

                if(Order != null && Order.OrderAddress!=null)
                    Address = db.Address.Where(a => a.Id == Order.OrderAddress.AdressId).Include(a => a.House).Include(a => a.House.Street).Include(a => a.House.Street.City).FirstOrDefault();

                //Вытаскиваем полный адрес доставки

                //Вытаскиваем стоимость доставки
                if (Order != null && Order.OrderAddress != null)
                {
                    ShipPrice = Order.OrderAddress.ShipPriceValue;
                    total += ShipPrice;
                }
                    

                if (Order.BotInfo == null)
                    Order.BotInfo = db.BotInfo.Where(b => b.Id == Order.BotInfoId).FirstOrDefault();

                if (Order.CurrentStatusNavigation == null)
                    Order.CurrentStatusNavigation = db.OrderStatus.Find(Order.CurrentStatus);

                if (Order.CurrentStatusNavigation != null && Order.CurrentStatusNavigation.Status == null)
                    Order.CurrentStatusNavigation.Status = db.Status.Find(Order.CurrentStatusNavigation.StatusId);


                if (Order.Paid == true)
                    Paid = "Оплачено";

                    else
                        Paid = "Не оплачено";


                if (Order.OrderProduct == null || Order.OrderProduct != null && Order.OrderProduct.Count == 0)
                    Order.OrderProduct = db.OrderProduct.Where(o => o.OrderId == Order.Id).ToList();

                total += Order.TotalPrice();

                /////////Формируем основную часть сообщения - Доставка
                if(Order != null && Order.OrderAddress!=null)
                    base.TextMessage = Bold("Номер заказа: ") + Order.Number.ToString() + NewLine()
                            + Order.PositionToString() + NewLine()
                            + Bold("Стоимость доставки:") + Order.OrderAddress.ShipPriceValue.ToString() + NewLine()
                            + Bold("Общая стоимость: ") + total.ToString() + NewLine()
                            + Bold("Комментарий: ") + Order.Text + NewLine()
                            + Bold("Способ получения заказа: ") + " Доставка" + NewLine()
                            + Bold("Адрес доставки: ") + Address.House.Street.City.Name + ", " + Address.House.Street.Name + ", д. " + Address.House.Number+", "+Address.House.Apartment + NewLine()
                            + Bold("Время: ") + Order.DateAdd.ToString() +NewLine()
                            + Bold("Способ оплаты: ") + PaymentMethodName + NewLine() 
                            +Bold("Статус заказа:")+ Order.CurrentStatusNavigation.Status.Name+NewLine()
                            +Bold("Оформлено через: ")+"@" + Order.BotInfo.Name +NewLine()
                            + Bold("Статус платежа: ") + Paid;

                /////////Формируем основную часть сообщения - Самовывоз
                if (Order != null && Order.PickupPoint != null)
                    base.TextMessage = Bold("Номер заказа: ")  + Order.Number.ToString() + NewLine()
                            + Order.PositionToString() + NewLine()
                            + Bold("Общая стоимость: ") + total.ToString() + NewLine()
                            + Bold("Комментарий: ") + Order.Text + NewLine()
                            + Bold("Способ получения заказа: ") + " Самовывоз" + NewLine()
                            + Bold("Адрес доставки: ") + Order.PickupPoint.Name + NewLine()
                            + Bold("Время: ") + Order.DateAdd.ToString() + NewLine()
                            + Bold("Способ оплаты: ") + PaymentMethodName + NewLine()
                            + Bold("Статус заказа:") + Order.CurrentStatusNavigation.Status.Name + NewLine()
                            + Bold("Оформлено через: ") + "@" + Order.BotInfo.Name + NewLine()
                            + Bold("Статус платежа: ") + Paid;


                InWorkFollowerId = WhoInWork(Order);

                CreateBtns();

                SetInlineKeyBoard();

                db.Dispose();

                return this;
            }
        }

        private int WhoInWork(Orders order)
        {
            if (order != null && order.OrdersInWork.Count > 0)
            {
                var in_work = order.OrdersInWork.OrderByDescending(o => o.Id).FirstOrDefault();

                if (in_work != null && in_work.InWork == true)
                    return Convert.ToInt32(in_work.FollowerId);

                else
                    return 0;
            }

            else
                return 0;
        }

        private void CreateBtns()
        {
            EditOrderPositionBtn = new InlineKeyboardCallbackButton("Изменить содержание заказа"+ " \ud83d\udd8a", BuildCallData(OrderProccesingBot.CmdEditOrderPosition, OrderProccesingBot.ModuleName, Order.Id));

            ViewTelephoneNumberBtn = new InlineKeyboardCallbackButton("Контактные данные"+ " \ud83d\udcde", BuildCallData(OrderProccesingBot.CmdGetTelephone, OrderProccesingBot.ModuleName, Order.Id));

            ViewAddressOnMapBtn = new InlineKeyboardCallbackButton("Показать на карте"+ " \ud83c\udfd8", BuildCallData(OrderProccesingBot.CmdViewAddressOnMap, OrderProccesingBot.ModuleName, Order.Id));

            ViewInvoiceBtn = new InlineKeyboardCallbackButton("Посмотреть счет" + " \ud83d\udcb5", BuildCallData(OrderProccesingBot.ViewInvoiceCmd, OrderProccesingBot.ModuleName, Order.Id));

            TakeOrderBtn = new InlineKeyboardCallbackButton("Взять в работу", BuildCallData("TakeOrder", OrderProccesingBot.ModuleName, Order.Id));

            FreeOrderBtn = new InlineKeyboardCallbackButton("Освободить", BuildCallData("FreeOrder", OrderProccesingBot.ModuleName, Order.Id));

            EditStatusBtn = new InlineKeyboardCallbackButton("Изменить статус", BuildCallData(OrderProccesingBot.CmdStatusEditor, OrderProccesingBot.ModuleName, Order.Id));

            StatusHistoryBtn = new InlineKeyboardCallbackButton("История изменений", BuildCallData(OrderProccesingBot.CmdStatusHistory, OrderProccesingBot.ModuleName, Order.Id));

        }

        private void SetInlineKeyBoard()
        {
            //Заявка еще ни кем не взята в обрабоку или Неизвстно кому мы отрпавляем это сообщение т.е переменная FollowerId=0
            if (InWorkFollowerId == 0  || FollowerId==0 || InWorkFollowerId!=FollowerId)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                    new[]
                    {
                        TakeOrderBtn
                    },

                });

            ///Заявка взята в обработку пользователем. Рисуем основные кнопки
            if (FollowerId==InWorkFollowerId && InWorkFollowerId!=0)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            ViewInvoiceBtn, FreeOrderBtn
                        },
                new[]
                        {
                            EditOrderPositionBtn
                        },
                new[]
                        {
                           EditStatusBtn, StatusHistoryBtn
                        },
                new[]
                        {
                            ViewTelephoneNumberBtn,
                            ViewAddressOnMapBtn
                        },

                 });

           


        }

    }
}
