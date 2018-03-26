using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.InlineKeyboardButtons;
using System.Security.Cryptography;
using System.Text;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Сообщение с описание заказа
    /// </summary>
    public class OrderViewMessage: BotMessage
    {
        private int OrderId { get; set; }

        private Orders Order { get; set; }

        private InlineKeyboardCallbackButton ChekPayBtn { get; set; }

        private InlineKeyboardCallbackButton AddFeedbackBtn { get; set; }

        private InlineKeyboardCallbackButton RemoveOrderBtn { get; set; }

        private InlineKeyboardCallbackButton ViewInvoiceBtn { get; set; }

        private const int QiwiPayMethodId = 2;

        private const int PaymentOnReceiptId = 1;

        private Address Address { get; set; }

        public OrderViewMessage (int OrderId)
        {
            this.OrderId = OrderId;
        }

        public OrderViewMessage (Orders order)
        {
            this.Order = order;
        }

        public override BotMessage BuildMsg()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                if (this.OrderId > 0) // если в конструктор был передан айди заявки
                    Order = db.Orders.Where(o => o.Id == OrderId).
                        Include(o => o.FeedBack).
                        Include(o => o.OrderProduct).
                        Include(o => o.PickupPoint).
                        Include(o => o.OrderAddress).
                        Include(o => o.BotInfo).
                        Include(o => o.Invoice).Include(o=>o.CurrentStatusNavigation.Status).
                        Include(o => o.OrderProduct).FirstOrDefault();

                if (Order != null && Order.CurrentStatusNavigation == null)
                    Order.CurrentStatusNavigation = db.OrderStatus.Where(o => o.Id == Order.CurrentStatus).Include(o=>o.Status).FirstOrDefault();

                if (Order!=null && Order.OrderProduct.Count==0)
                    Order.OrderProduct = db.OrderProduct.Where(op => op.OrderId == Order.Id).ToList();

                if (Order != null && Order.CurrentStatusNavigation != null && Order.CurrentStatusNavigation.Status==null)
                    Order.CurrentStatusNavigation.Status = db.Status.Find(Order.CurrentStatusNavigation.StatusId);


                if (Order != null && Order.PickupPoint == null && Order.PickupPointId > 0)
                    Order.PickupPoint = db.PickupPoint.Find(Order.PickupPointId);

                if (Order != null && Order.OrderAddress==null)
                    Order.OrderAddress = db.OrderAddress.Where(o => o.OrderId == Order.Id).FirstOrDefault();

                if (Order != null)
                {

                    string paid = "";

                    double total = Order.TotalPrice(); // общая строисоить заказа

                    if(Order.OrderAddress!=null) // способо получения - Доставка
                        Address = db.Address.Where(a => a.Id == Order.OrderAddress.AdressId).
                        Include(a => a.House).
                        Include(a => a.House.Street).
                        Include(a => a.House.Street.City).
                        FirstOrDefault();


                    try
                    {

                        if (Order.BotInfo == null)
                            Order.BotInfo = db.BotInfo.Where(o => o.Id == Order.BotInfoId).FirstOrDefault();

                        //Прибавляем стоимость доставки к стоимости заказа
                        if (Order.OrderAddress != null)
                            total += Order.OrderAddress.ShipPriceValue;
                        
                        if (Order.Paid == true) // Заказ оплачен
                            paid = "Да";

                        if (Order.OrderAddress != null)
                            base.TextMessage = Bold("Номер заказа: ") + Order.Number.ToString() + NewLine()
                                    + Order.PositionToString() + NewLine()
                                    + Bold("Стоимость доставки:") + Order.OrderAddress.ShipPriceValue.ToString() + NewLine()
                                    + Bold("Общая стоимость: ") + total.ToString() +" "+ Order.OrderProduct.FirstOrDefault().Price.Currency.ShortName + NewLine()
                                    + Bold("Комментарий: ") + Order.Text + NewLine()
                                    + Bold("Способ получения заказа: ") + " Доставка" + NewLine()
                                    + Bold("Адрес доставки: ") + Address.House.Street.City.Name + ", " + Address.House.Street.Name + ",д. " + Address.House.Number + "," + Address.House.Apartment + NewLine()
                                    + Bold("Время: ") + Order.DateAdd.ToString() + NewLine()
                                    + Bold("Оплачено: ") + paid
                                    + NewLine() + Bold("Статус заказа: ") + Order.CurrentStatusNavigation.Status.Name
                                    + NewLine() + Bold("Оформлен через:") + "@" + Order.BotInfo.Name;


                        if (Order.PickupPoint != null)
                            base.TextMessage = Bold("Номер заказа: ") + Order.Number.ToString() + NewLine()
                                    + Order.PositionToString() + NewLine()
                                    + Bold("Общая стоимость: ") + total.ToString() + " " + Order.OrderProduct.FirstOrDefault().Price.Currency.ShortName + NewLine()
                                    + Bold("Комментарий: ") + Order.Text + NewLine()
                                    + Bold("Способо получения закза: ") + " Самовывоз" + NewLine()
                                    + Bold("Пункт самовывоза: ") + Order.PickupPoint.Name + NewLine()
                                    + Bold("Время: ") + Order.DateAdd.ToString() + NewLine()
                                    + Bold("Оплачено: ") + paid
                                    + NewLine() + Bold("Статус: ") + Order.CurrentStatusNavigation.Status.Name
                                    + NewLine() + Bold("Оформлен через:") + "@" + Order.BotInfo.Name;

                    }
                    catch (Exception exp)
                    {

                    }

                    SetButton();
                    base.CallBackTitleText = "Номер заказа:" + Order.Number.ToString();
                }


            }

            return this;
        }

        private void SetButton()
        {
            if(this.Order!=null && this.Order.CurrentStatusNavigation.StatusId==Bot.Core.ConstantVariable.OrderStatusVariable.Completed && this.Order.Invoice!=null)               
                base.MessageReplyMarkup = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(
                    new[]{
                                new[]
                                    {
                                            AddFeedBack()
                                    },
                                new[]
                                    {
                                            ViewInvoice()
                                    },
                    });

            if (this.Order != null && this.Order.CurrentStatusNavigation.StatusId == Bot.Core.ConstantVariable.OrderStatusVariable.Completed && this.Order.Invoice == null)
                base.MessageReplyMarkup = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(
                new[]{
                                new[]
                                    {
                                            AddFeedBack()
                                    },

                });


            if (this.Order != null && this.Order.CurrentStatusNavigation.StatusId != Bot.Core.ConstantVariable.OrderStatusVariable.Completed && this.Order.Invoice != null)
                base.MessageReplyMarkup = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(
                new[]{
                                new[]
                                    {
                                           ViewInvoice()
                                    },

                });


        }

        private InlineKeyboardCallbackButton AddFeedBack()
        {
            return new InlineKeyboardCallbackButton("Отзыв к заказу", BuildCallData(Bot.OrderBot.CmdAddFeedBack, Bot.OrderBot.ModuleName, Order.Id)); 
        }


        private InlineKeyboardCallbackButton ViewInvoice()
        {
            InlineKeyboardCallbackButton button = new InlineKeyboardCallbackButton("Счет на оплату", BuildCallData(Bot.OrderBot.ViewInvoiceCmd, Bot.OrderBot.ModuleName, Convert.ToInt32(Order.Id)));
            return button;
        }

    }
}
