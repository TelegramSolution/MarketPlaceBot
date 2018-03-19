using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.IO;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot.Core;
using MyTelegramBot.BusinessLayer;

namespace MyTelegramBot.Bot.AdminModule
{
    public partial class OrderProccesingBot : BotCore
    {
        public const string ModuleName = "Or";
       
        private int OrderId { get; set; }


        /// <summary>
        /// Админ. Показать номере телефона клиента
        /// </summary>
        public const string CmdGetTelephone = "GetTelephone";

        /// <summary>
        /// Админ. Показать заказ для Админа
        /// </summary>
        public const string CmdGetOrderAdmin = "GetOrderAdmin";

        /// <summary>
        /// Админ. Изменить позиции заказа
        /// </summary>
        public const string CmdEditOrderPosition = "EditOrderPosition";

        /// <summary>
        /// Обработать заказ. (Согласовать, Удалить и т.д)
        /// </summary>
        public const string CmdProccessOrder = "ProccessOrder";

        /// <summary>
        /// Показать адрес доставки на карте
        /// </summary>
        public const string CmdViewAddressOnMap = "ViewAddressOnMap";

        /// <summary>
        /// Изменить статус заказа
        /// </summary>
        public const string CmdUpdateOrderStatus = "UpdOrderStatus";

        /// <summary>
        /// Удалить заказ
        /// </summary>
        public const string CmdOrderDelete = "OrderDelete";

        public const string ViewInvoiceCmd = "ViewInvoice";

        /// <summary>
        /// отправить инвойс новым сообщением
        /// </summary>
        public const string SendInvoiceCmd = "SendIvoice";

        /// <summary>
        /// Заказа согласован
        /// </summary>
        public const string CmdConfirmOrder = "ConfirmOrder";

        public const string FeedBackOrderCmd = "FeedBackOrder";

        /// <summary>
        /// изменить статус заказа
        /// </summary>
        public const string CmdStatusEditor = "StatusEditor";

        /// <summary>
        /// история статусов заказа
        /// </summary>
        public const string CmdStatusHistory = "StatusHistory";

        /// <summary>
        /// Восстановить заказ (Если удален)
        /// </summary>
        public const string CmdRecoveryOrder = "RecoveryOrder";

        /// <summary>
        /// Заказ выполнен
        /// </summary>
        public const string CmdDoneOrder = "DoneOrder";

        /// <summary>
        /// подтвердить новый статус закзаа
        /// </summary>
        public const string CmdConfirmNewStatus = "ConfirmNewStatus";

        /// <summary>
        /// Назад
        /// </summary>
        public const string CmdBackToOrder = "BackToOrder";

        public const string CmdOpenOrder = "OpenOrder";

        public const string CmdOverridePerformerOrder = "OverridePerfOrder";

        public const string CmdBackOverride = "BackOverride";

        public const string PaymentCmd = "/payment";

        /// <summary>
        /// назад к выбору статуса заказа
        /// </summary>
        public const string CmdBackToStatusEditor = "BackToStatusEditor";

        private const string ForceReplyOrderDelete = "Удалить заказ:";

        private const string ForceReplyOrderDone = "Выполнить заказ:";

        private const string ForceReplyOrderConfirm = "Согласовать заказ:";

        private const string ForceReplyAddFeedBack = "Добавить отзыв к заказу:";

        private const string ForceReplyAddCommentToStatus = "Комментарий к статусу:";

        private const string GetOrderCmd = "/order";

        public const string ViewPaymentCmd = "ViewPayment";

        /// <summary>
        /// команда для отображения доп. кнопок обработки заказа
        /// </summary>
        public const string Page2Cmd = "Page2";

        /// <summary>
        /// команда для отображения основных кнопок обработки заказа
        /// </summary>
        public const string MainPageCmd = "MainPage";

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_update"></param>
        public OrderProccesingBot(Update _update) : base(_update)
        {


        }

        protected override void Initializer()
        {

            if (Update.Message != null && Update.Message.ReplyToMessage != null)
                CommandName = Update.Message.ReplyToMessage.Text;

            try
            {
                if (base.Argumetns.Count > 0)
                {
                    //Команды которые относяттся к этому модулю в массиве аргементов первым элементом всегла должен быть id заказа
                    OrderId = Argumetns[0];

                }

            }

            catch (Exception e)
            {

            }

        }

        public async override Task<IActionResult> Response()
        {
            if (IsOperator() || IsOwner())
            {
                switch (base.CommandName)
                {
                    ///Сообщение с деталями заказа для администратора системы
                    case CmdGetOrderAdmin :
                        return await GetOrderAdmin();

                    case CmdOpenOrder:
                        return await GetOrderAdmin();

                    case CmdBackOverride:
                        return await GetOrderAdmin(base.MessageId);

                    ///Администратор нажал на кнопку "показать номер телефона"
                    case CmdGetTelephone:
                        return await SendContactUser();

                    ///Адмнистратор нажал на кнопку "Показать на карте"
                    case CmdViewAddressOnMap:
                        return await SendOrderAddressOnMap();


                    //админ нажал на кнопку изменить статус закза
                    case CmdStatusEditor:
                        return await SendStatusEditor();

                    ///Пользователь нажал на кнопку "Назад"
                    case CmdBackToOrder:
                        return await BackToOrder();

                    case CmdEditOrderPosition:
                        return await SendEditorOrderPositionList();

                    case "TakeOrder":
                        return await TakeOrder();

                    case "FreeOrder":
                        return await FreeOrder();

                    case CmdUpdateOrderStatus:
                        return await InsertOrderStatus(Argumetns[1]);

                    case "StatusAddComment":
                        return await SendForceReplyMessage(ForceReplyAddCommentToStatus + Argumetns[0].ToString());

                    case CmdConfirmNewStatus:
                        return await ConfirmNewStatus();

                    case CmdBackToStatusEditor:
                        return await BackToStatusEditor();

                    case CmdStatusHistory:
                        return await SendHistoryOrderStatus();

                    case CmdOverridePerformerOrder:
                        return await ConfirmOverridePerformer();

                    case ViewInvoiceCmd:
                        return await SendViewInvoice();

                    case ViewPaymentCmd:
                        return await SendViewPayment(Argumetns[1],MessageId);

                    case FeedBackOrderCmd:
                        return await SendFeedBackOrder(Argumetns[0]);

                    case Page2Cmd:
                        return await SendPage2Keyboard();

                    case MainPageCmd:
                        return await SendMainKeyboard();

                    default:
                        break;

                }

                if (base.CommandName.Contains(PaymentCmd))
                    return await SendViewPayment();


                /// /order показать заказ для админа
                if (base.CommandName.Contains(GetOrderCmd))
                    return await GetOrderByNumber();

                //пользователь на нажал на кнопку добавить комент к статусу
                if (OriginalMessage.Contains(ForceReplyAddCommentToStatus))
                    return await AddCommentToStatus();


                else
                    return null;
            }


            else
                return null;


        }

        private async Task<IActionResult> SendMainKeyboard()
        {
            AdminOrderMessage adminOrder = new AdminOrderMessage(OrderId);

            await EditInlineReplyKeyboard(adminOrder.MainKeyboard());

            return OkResult;
        }

        private async Task<IActionResult> SendPage2Keyboard()
        {
            AdminOrderMessage adminOrder = new AdminOrderMessage(OrderId);

            await EditInlineReplyKeyboard(adminOrder.Page2Keyboard());

            return OkResult;
        }

        /// <summary>
        /// отправить сообщение со счетом на оплату
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendViewInvoice()
        {

            var Invoice = InvoiceFunction.GetInvoiceByOrderId(OrderId);

            BotMessage = new AdminViewInvoice(Invoice);

            var mess = BotMessage.BuildMsg();

            if (mess != null)
                await EditMessage(mess);

            else
                await AnswerCallback("Счет на оплату отсутствует", true);

            return OkResult;
        }

        /// <summary>
        /// Показать детали платежа /payment[id]
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendViewPayment()
        {
            try
            {
                int Id =Convert.ToInt32(base.CommandName.Substring(PaymentCmd.Length));

                var Payment = PaymentsFunction.GetPayment(Id);

                BotMessage = new PaymentViewMessage(Payment);
                await SendMessage(BotMessage.BuildMsg());
                return OkResult;
            }

            catch
            {
                return OkResult;
            }
        }

        /// <summary>
        /// Показать платеж по id счета
        /// </summary>
        /// <param name="InvoiceId"></param>
        /// <param name="MessageId"></param>
        /// <returns></returns>
        private async Task<IActionResult> SendViewPayment(int PaymentId, int MessageId=0)
        {
            try
            {
                var Payment = PaymentsFunction.GetPayment(PaymentId);

                if (Payment != null)
                {
                    BotMessage = new PaymentViewMessage(Payment);
                    await SendMessage(BotMessage.BuildMsg(), MessageId);
                }

                else
                   await AnswerCallback("Данные по оплате отсутствуют", true);

                return OkResult;
            }

            catch
            {
                return OkResult;
            }
        }


        private async Task<IActionResult> SendFeedBackOrder(int OrderId)
        {
            BotMessage = new ViewFeedBackOrderMessage(OrderId);
            var mess = BotMessage.BuildMsg();

            if (mess != null)
                await EditMessage(mess);

            else
                await AnswerCallback("Отзыва еще нет", true);

            return OkResult;
        }


        /// <summary>
        /// Подтверждение того что пользователь назначает исполнителем себя вместо кого-то
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> ConfirmOverridePerformer()
        {

            var Order = OrderFunction.GetOrder(OrderId);

            var Inwork = OrderFunction.InsertOrderInWork(OrderId, FollowerId);

            Order.OrdersInWork.Add(Inwork);

            BotMessage = new AdminOrderMessage(Order, FollowerId);

            await EditMessage(BotMessage.BuildMsg());

            //уведомляем всех о том что кто то взял заказ в обработку
            string notify = "Заказ №" + Order.Number.ToString() + " взят в работу. Пользователь " + GeneralFunction.FollowerFullName(base.FollowerId);
            BotMessage = new OrderMiniViewMessage(notify, Order.Id);
            await SendMessageAllBotEmployeess(BotMessage.BuildMsg());

            return OkResult;
            
        }

        /// <summary>
        /// Сообщение с историей измений статусов
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendHistoryOrderStatus()
        {
            BotMessage = new OrderStatusHistoryMessage(Argumetns[0]);

            await EditMessage(BotMessage.BuildMsg());

            return OkResult;
        }

        /// <summary>
        /// вернуть назад к выбору статусов. Перед этим удаляем уже добавленные данные
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> BackToStatusEditor()
        {
            int OrderStatusId = Argumetns[1];

            OrderFunction.RemoveStatus(OrderStatusId);

            return await SendStatusEditor();
            
        }

        /// <summary>
        /// Сохраняем новый статус заказа и уведомляем пользователей
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> ConfirmNewStatus()
        {
            try
            {
                if (Argumetns != null && Argumetns.Count == 2)
                {
                    var status = OrderFunction.ConfirmOrderStatus(Argumetns[1]);

                    //меняем текущее сообщение на сообщение с описание заказа
                    await GetOrderAdmin(base.MessageId);

                    var Order = OrderFunction.GetOrder(OrderId);

                    //уведомляем всех о новом статусе заказа
                    string textmsg = "Пользователь:" + GeneralFunction.FollowerFullName(Order.CurrentStatusNavigation.FollowerId)
                        + "изменил статус заказа №" + Order.Number.ToString()
                        + Core.BotMessage.NewLine() + Order.CurrentStatusNavigation.Status.Name + ": " + Order.CurrentStatusNavigation.Text;
                    BotMessage = new OrderMiniViewMessage(textmsg, Order.Id);
                    await SendMessageAllBotEmployeess(BotMessage.BuildMsg());

                    ///Если поставили статус "Выполено" то пользователю оформившему данные заказ приходил сообщение с просьбой 
                    ///оставить отзыв. Остатки на скалде пересчитываются и операторам приходит уведомление об изменениях в остатках
                    if (status != null && status.StatusId == Core.ConstantVariable.OrderStatusVariable.Completed)
                    {
                        var stock = Order.UpdateStock();

                        BotMessage = new FeedBackOfferMessage(Order);

                        await SendMessage(Order.Follower.ChatId, BotMessage.BuildMsg());

                        BotMessage = new StockChangesMessage(stock, Order.Id);

                        await SendMessageAllBotEmployeess(BotMessage.BuildMsg());
                    }

                    return OkResult;
                }

                else
                    return OkResult;
            }
            catch
            {
                return OkResult;
            }
        }

        /// <summary>
        /// Добавить комментраий к новому статусу
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> AddCommentToStatus()
        {
            int OrderStatusId = 0;
            try
            {

                OrderStatusId = Convert.ToInt32(base.OriginalMessage.Substring(ForceReplyAddCommentToStatus.Length));

                var orderStatus = OrderFunction.AddCommentToStatus(OrderStatusId, ReplyToMessageText);

                var Order = OrderFunction.GetOrder(Convert.ToInt32(orderStatus.OrderId));

                BotMessage = new OrderStatusConfirmMessage(Order, orderStatus);

                await SendMessage(BotMessage.BuildMsg());

                return OkResult;
            }
            catch
            {
                return OkResult;
            }

        }

        /// <summary>
        /// Добавить новый статус заказа
        /// </summary>
        /// <param name="NewStatusId"></param>
        /// <returns></returns>
        private async Task<IActionResult> InsertOrderStatus(int NewStatusId)
        {
            var Order = OrderFunction.GetOrder(OrderId);

            if (Order != null)
            {
                var orderStatus = OrderFunction.InsertOrderStarus(OrderId, NewStatusId, FollowerId, false);

                BotMessage = new OrderStatusConfirmMessage(Order, orderStatus);

                await EditMessage(BotMessage.BuildMsg());

             
            }

            return OkResult;
        }

        /// <summary>
        /// Отправить сообщения с кнопками статусов заказа (1. Согласован 2. Отменен 3. Удален и тд)
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendStatusEditor()
        {
            var Order = OrderFunction.GetOrder(OrderId);

            if (Order != null)
            {
                BotMessage= new OrderStatusEditorMessage(Order);
                await EditMessage(BotMessage.BuildMsg());
                return OkResult;
            }

            else
                return OkResult;


        }


        /// <summary>
        /// /order[номер закааз]
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> GetOrderByNumber()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                try
                {
                    int number = Convert.ToInt32(base.CommandName.Substring(GetOrderCmd.Length));

                    int id = db.Orders.Where(o => o.Number == number).FirstOrDefault().Id;

                    BotMessage = new AdminOrderMessage(id,FollowerId);
                    await SendMessage(BotMessage.BuildMsg());

                    return OkResult;
                }

                catch
                {
                    return OkResult;
                }
            }

        }

        /// <summary>
        /// Освободить заказ.
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> FreeOrder()
        {

            var Inwork = OrderFunction.InsertOrderInWork(OrderId, FollowerId, false);

            var Order = OrderFunction.GetOrder(OrderId);
                        
            BotMessage = new AdminOrderMessage(Order, FollowerId);
            await base.EditMessage(BotMessage.BuildMsg());

            string notify = "Пользователь " + GeneralFunction.FollowerFullName(FollowerId) + " освободил заказ №" + Order.Number.ToString();

            BotMessage= new OrderMiniViewMessage(notify, Order.Id);
            await SendMessageAllBotEmployeess(BotMessage.BuildMsg());

            return OkResult;
            
        }


        /// <summary>
        /// Показать номер телефона покупателя
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendContactUser()
        {

            var order = OrderFunction.GetOrder(OrderId);

            if (order != null && order.Follower!=null && order.Follower.Telephone!=null && order.Follower.Telephone!="")
            {
                Contact contact = new Contact
                {
                    FirstName = order.Follower.FirstName,
                    PhoneNumber = order.Follower.Telephone,
                     UserId=order.Follower.ChatId

                };

                await SendContact(contact);

            }

            if (order != null && order.Follower!=null && order.Follower.UserName != null && order.Follower.UserName != "")
                await SendUrl("https://t.me/" + order.Follower.UserName);
  

            return OkResult;
            
        }

        /// <summary>
        /// Показать адрес доставки на карте
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendOrderAddressOnMap()
        {
            var Address = OrderFunction.GetAddress(OrderId);

            if (Address != null && Address.Adress.House!=null)
            {
                Location location = new Location
                {
                    Latitude = Convert.ToSingle(Address.Adress.House.Latitude),
                    Longitude = Convert.ToSingle(Address.Adress.House.Longitude)
                };

                await SendLocation(location);
                       
            }
            return OkResult;
            
        }


        private async Task<IActionResult> GetOrderAdmin(int MessageId=0)
        {
            BotMessage = new AdminOrderMessage(OrderId,FollowerId);

            await SendMessage(BotMessage.BuildMsg(), MessageId);

            return base.OkResult;

        }

        /// <summary>
        /// Позиции заказа. Кнопками
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendEditorOrderPositionList()
        {
            BotMessage = new OrderPositionListMessage(this.OrderId);
            await EditMessage(BotMessage.BuildMsg());
            return base.OkResult;

        }

        private async Task<IActionResult> TakeOrder()
        {
            var Order = OrderFunction.GetOrder(OrderId);


            //Заказ ни кем не обрабатывается 
            if(Order!=null && Order.OrdersInWork.Count == 0 
                || Order != null && Order.OrdersInWork.Count>0 && Order.OrdersInWork.LastOrDefault().InWork==false)
            {
                Order.OrdersInWork.Add(OrderFunction.InsertOrderInWork(OrderId, FollowerId));
                BotMessage = new AdminOrderMessage(Order,FollowerId);
                await EditMessage(BotMessage.BuildMsg());

                //уведомляем всех о том что кто-то взял заказ работу
                string notify = "Заказ №" + Order.Number.ToString() + " взят в работу. Пользователь " + GeneralFunction.FollowerFullName(base.FollowerId);
                BotMessage = new OrderMiniViewMessage(notify, this.OrderId);
                await SendMessageAllBotEmployeess(BotMessage.BuildMsg());
                return OkResult;
            }

            //заявка в обработке у тек. пользователя
            if(Order != null && Order.OrdersInWork.Count > 0 && Order.OrdersInWork.LastOrDefault().InWork == true &&
                Order.OrdersInWork.LastOrDefault().FollowerId==FollowerId)
            {
                BotMessage = new AdminOrderMessage(Order, FollowerId);
                await EditMessage(BotMessage.BuildMsg());
                return OkResult;
            }

            //заказ в обработке у кого то другого. Отправляем сообщение с вопрос о переназначении исполнителя
            if (Order != null && Order.OrdersInWork.Count > 0 && Order.OrdersInWork.LastOrDefault().InWork == true &&
                Order.OrdersInWork.LastOrDefault().FollowerId != FollowerId)
            {
                BotMessage = new OverridePerformerConfirmMessage(Order, Order.OrdersInWork.LastOrDefault());
                var mess = BotMessage.BuildMsg();
                await EditMessage(mess);
                return OkResult;
            }

            else
                return OkResult;

        }


        /// <summary>
        /// Назад
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> BackToOrder()
        {
            BotMessage = new AdminOrderMessage(OrderId, FollowerId);
            await EditMessage(BotMessage.BuildMsg());
            return OkResult;

        }



    }
}
