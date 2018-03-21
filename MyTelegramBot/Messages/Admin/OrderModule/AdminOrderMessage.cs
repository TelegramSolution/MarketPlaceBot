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
        //private InlineKeyboardCallbackButton EditOrderPositionBtn { get; set; }

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

        private InlineKeyboardCallbackButton ViewFeedBackBtn { get; set; }

        /// <summary>
        /// после нажатия на эту кнопку появляются доп. кнопки обработки заказа
        /// </summary>
        private InlineKeyboardCallbackButton Page2Btn { get; set; }

        /// <summary>
        /// вернуться к списку заказов
        /// </summary>
        private InlineKeyboardCallbackButton BackToOrdersListBtn { get; set; }

        /// <summary>
        /// после нажатия на эту кнопку появляется основные кнопки обработки заказа
        /// </summary>
        private InlineKeyboardCallbackButton BackToMainPageBtn { get; set; }
      
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
            if (order != null)
            {
                this.Order = order;
                this.OrderId = this.Order.Id;
                this.FollowerId = FollowerId;
            }
        }

        public override BotMessage BuildMsg()
        {
            double total = 0.0;
            double ShipPrice = 0;
            string Position = "";
            string Paid = "";

            MarketBotDbContext db = new MarketBotDbContext();

                if(this.Order==null && this.OrderId>0)
                Order = db.Orders.Where(o => o.Id == OrderId).
                    Include(o=>o.FeedBack).
                    Include(o => o.OrderProduct).
                    Include(o => o.OrderAddress).
                    Include(o=>o.OrdersInWork).
                    Include(o=>o.Invoice.PaymentType).
                    Include(o=>o.PickupPoint).
                    Include(o=>o.CurrentStatusNavigation).
                    FirstOrDefault();

            if (Order != null)
            {
                ///////////Провереряем какой способ оплаты ///////////
                if (Order.Invoice != null && Order.Invoice.PaymentType != null)
                    PaymentMethodName = db.PaymentType.Find(Order.Invoice.PaymentTypeId).Name;

                else
                    PaymentMethodName = "При получении";

                //Вытаскиваем полный адрес доставки
                if (Order.OrderAddress == null)
                    Order.OrderAddress = db.OrderAddress.Where(o => o.OrderId == Order.Id).FirstOrDefault();

                if (Order.OrderAddress != null) //Вытаскиваем полный адрес доставки
                    Address = db.Address.Where(a => a.Id == Order.OrderAddress.AdressId)
                        .Include(a => a.House)
                        .Include(a => a.House.Street)
                        .Include(a => a.House.Street.City).FirstOrDefault();


                //Jghtltktzv стоимость доставки
                if (Order.OrderAddress != null)
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
                if (Order.OrderAddress != null)
                    base.TextMessage = Bold("Номер заказа: ") + Order.Number.ToString() +" /order" + Order.Number.ToString() + NewLine()
                            + Order.PositionToString() + NewLine()
                            + Bold("Стоимость доставки:") + Order.OrderAddress.ShipPriceValue.ToString() + NewLine()
                            + Bold("Общая стоимость: ") + total.ToString() + NewLine()
                            + Bold("Комментарий: ") + Order.Text + NewLine()
                            + Bold("Способ получения заказа: ") + " Доставка" + NewLine()
                            + Bold("Адрес доставки: ") + Address.House.Street.City.Name + ", " + Address.House.Street.Name + ", д. " + Address.House.Number + ", " + Address.House.Apartment + NewLine()
                            + Bold("Время: ") + Order.DateAdd.ToString() + NewLine()
                            + Bold("Способ оплаты: ") + PaymentMethodName + NewLine()
                            + Bold("Статус заказа:") + Order.CurrentStatusNavigation.Status.Name + NewLine()
                            + Bold("Оформлено через: ") + "@" + Order.BotInfo.Name + NewLine()
                            + Bold("Статус платежа: ") + Paid;

                /////////Формируем основную часть сообщения - Самовывоз
                if (Order.PickupPoint != null)
                    base.TextMessage = Bold("Номер заказа: ") + Order.Number.ToString() + " /order" + Order.Number.ToString() + NewLine()
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

                SetInlineKeyBoard();

                db.Dispose();

                return this;
            }

            else
            {
                db.Dispose();
                return null;
            }
            
        }

        /// <summary>
        /// кто обрабатывает заявку. id пользователя в бд
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
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


        private void SetInlineKeyBoard()
        {
            //Заявка еще ни кем не взята в обрабоку или Неизвстно кому мы отрпавляем это сообщение т.е переменная FollowerId=0
            if (InWorkFollowerId == 0  || FollowerId==0 || InWorkFollowerId!=FollowerId)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                    new[]
                    {
                        BuildInlineBtn("Взять в работу", BuildCallData("TakeOrder", OrderProccesingBot.ModuleName, Order.Id))
                    }

                });

            ///Заявка взята в обработку пользователем. Рисуем основные кнопки
            if (FollowerId == InWorkFollowerId && InWorkFollowerId != 0)
                base.MessageReplyMarkup = MainKeyboard();

           


        }


        public InlineKeyboardMarkup Page2Keyboard()
        {
            ViewAddressOnMapBtn = BuildInlineBtn("Показать на карте" + " \ud83c\udfd8", BuildCallData(OrderProccesingBot.CmdViewAddressOnMap, OrderProccesingBot.ModuleName, OrderId));

            StatusHistoryBtn = BuildInlineBtn("История изменений", BuildCallData(OrderProccesingBot.CmdStatusHistory, OrderProccesingBot.ModuleName, OrderId));

            ViewFeedBackBtn = BuildInlineBtn("Отзыв", BuildCallData(OrderProccesingBot.FeedBackOrderCmd, OrderProccesingBot.ModuleName, OrderId), base.StartEmodji);

            BackToMainPageBtn= BuildInlineBtn(base.Previuos2Emodji, BuildCallData(OrderProccesingBot.MainPageCmd, OrderProccesingBot.ModuleName, OrderId));

            return new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            ViewAddressOnMapBtn
                        },

                new[]
                        {
                           StatusHistoryBtn,ViewFeedBackBtn
                        },
                new[]
                        {
                            BackToMainPageBtn,BackToAdminPanelBtn()

                        },

                 });
        }

        public InlineKeyboardMarkup MainKeyboard()
        {
            ViewTelephoneNumberBtn = BuildInlineBtn("Контактные данные" + " \ud83d\udcde", BuildCallData(OrderProccesingBot.CmdGetTelephone, OrderProccesingBot.ModuleName, OrderId));

            ViewInvoiceBtn = BuildInlineBtn("Посмотреть счет" + " \ud83d\udcb5", BuildCallData(OrderProccesingBot.ViewInvoiceCmd, OrderProccesingBot.ModuleName, OrderId));

            TakeOrderBtn = BuildInlineBtn("Взять в работу", BuildCallData("TakeOrder", OrderProccesingBot.ModuleName, OrderId));

            FreeOrderBtn = BuildInlineBtn("Освободить", BuildCallData("FreeOrder", OrderProccesingBot.ModuleName, OrderId));

            EditStatusBtn = BuildInlineBtn("Изменить статус", BuildCallData(OrderProccesingBot.CmdStatusEditor, OrderProccesingBot.ModuleName, OrderId));

            BackToOrdersListBtn = BuildInlineBtn("Назад к заказам", BuildCallData(AdminBot.ViewOrdersListCmd, AdminBot.ModuleName), base.Previuos2Emodji, false);

            Page2Btn = BuildInlineBtn(base.Next2Emodji, BuildCallData(OrderProccesingBot.Page2Cmd, OrderProccesingBot.ModuleName,OrderId));

            return new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            EditStatusBtn, FreeOrderBtn
                        },

                new[]
                        {
                           ViewInvoiceBtn,ViewTelephoneNumberBtn
                        },
                new[]
                        {
                            BackToAdminPanelBtn()

                        },
                new[]
                        {
                            BackToOrdersListBtn,Page2Btn
                        }

                 });
        }
    }
}
