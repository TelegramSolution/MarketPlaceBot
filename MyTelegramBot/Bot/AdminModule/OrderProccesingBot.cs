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

namespace MyTelegramBot.Bot.AdminModule
{
    public partial class OrderProccesingBot : Bot.BotCore
    {
        public const string ModuleName = "Or";

        StockChangesMessage StockChangesMsg { get; set; }
       
        /// <summary>
        /// Сообщение с описание заказа и админскими кнопками
        /// </summary>
        private AdminOrderMessage OrderAdminMsg { get; set; }


        /// <summary>
        /// Сообщение с позициями заказа. Каждай позиция отдельная кнопка
        /// </summary>
        private OrderPositionListMessage OrderPositionListMsg { get; set; }


        /// <summary>
        /// Предложить отсвить отзыв
        /// </summary>
        private FeedBackOfferMessage FeedBackOfferMsg { get; set; }

        private InvoiceViewMessage InvoiceViewMsg { get; set; }

        private int OrderId { get; set; }

        private Orders Order { get; set; }

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

        /// <summary>
        /// Заказа согласован
        /// </summary>
        public const string CmdConfirmOrder = "ConfirmOrder";

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


        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_update"></param>
        public OrderProccesingBot(Update _update) : base(_update)
        {


        }

        protected override void Constructor()
        {

            if (Update.Message != null && Update.Message.ReplyToMessage != null)
                CommandName = Update.Message.ReplyToMessage.Text;

            try
            {
                if (base.Argumetns.Count > 0)
                {

                    OrderId = Argumetns[0];
                    OrderAdminMsg = new AdminOrderMessage(this.OrderId,FollowerId);
                    OrderPositionListMsg = new OrderPositionListMessage(this.OrderId);
                    FeedBackOfferMsg = new FeedBackOfferMessage(this.OrderId);
                    using (MarketBotDbContext db = new MarketBotDbContext())
                        Order = db.Orders.Where(o => o.Id == this.OrderId).Include(o => o.Confirm).
                            Include(o => o.Done).Include(o => o.Delete).Include(o => o.OrderProduct).
                            Include(o => o.Follower).Include(o => o.FeedBack).Include(o=>o.OrderAddress).Include(o=>o.CurrentStatusNavigation).Include(o=>o.Invoice).Include(o=>o.OrdersInWork).FirstOrDefault();

                    InvoiceViewMsg = new InvoiceViewMessage(Order.Invoice, Order.Id);
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
                        return await GetContact();

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

                    case "ViewInvoice":
                        return await SendInvoice();

                    case CmdUpdateOrderStatus:
                        return await UpdateOrderStatus(Argumetns[1]);

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

                    default:
                        break;

                }

                //Администратор нажал на кнопку "Удалить заказ"
                if (base.CommandName == CmdOrderDelete && Order != null)
                    return await SendForceReplyMessage(ForceReplyOrderDelete + Order.Number.ToString());

                //Администратор нажал на кнопку "Заказ согласован"
                if (base.CommandName == CmdConfirmOrder && Order != null)
                    return await SendForceReplyMessage(ForceReplyOrderConfirm + Order.Number.ToString());

                /// /order показать заказ для админа
                if (base.CommandName.Contains(GetOrderCmd))
                    return await GetOrder();

                //пользователь на нажал на кнопку добавить комент к статусу
                if (OriginalMessage.Contains(ForceReplyAddCommentToStatus))
                    return await AddCommentToStatus();

                else
                    return null;
            }


            else
                return null;


        }

        /// <summary>
        /// Подтверждение того что пользователь назначает исполнителем себя вместо кого-то
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> ConfirmOverridePerformer()
        {
            using (MarketBotDbContext db=new MarketBotDbContext())
            {
                OrdersInWork ordersInWork = new OrdersInWork
                {
                    FollowerId = FollowerId,
                    InWork = true,
                    Timestamp = DateTime.Now,
                    OrderId = Order.Id
                };

                db.OrdersInWork.Add(ordersInWork);

                db.SaveChanges();

                OrderAdminMsg = new AdminOrderMessage(this.Order.Id, base.FollowerId);
                await EditMessage(OrderAdminMsg.BuildMsg());

                string notify = "Заказ №" + this.Order.Number.ToString() + " взят в работу. Пользователь " + GeneralFunction.FollowerFullName(base.FollowerId);

                var OrderMiniViewMsg = new OrderMiniViewMessage(notify, this.Order.Id);
                await SendMessageAllBotEmployeess(OrderMiniViewMsg.BuildMsg());

                return OkResult;
            }
        }

        /// <summary>
        /// Сообщение с историей измений статусов
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendHistoryOrderStatus()
        {
            OrderStatusHistoryMessage orderStatusHistoryMsg = new OrderStatusHistoryMessage(Argumetns[0]);

            await EditMessage(orderStatusHistoryMsg.BuildMsg());

            return OkResult;
        }

        /// <summary>
        /// вернуть назад к выбору статусов. Перед этим удаляем уже добавленные данные
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> BackToStatusEditor()
        {
            using (MarketBotDbContext db=new MarketBotDbContext())
            {
                var OrderStatus = db.OrderStatus.Find(Argumetns[1]);

                db.OrderStatus.Remove(OrderStatus);

                db.SaveChanges();

                return await SendStatusEditor();
            }
        }

        /// <summary>
        /// Сохраняем новый статус заказа и уведомляем пользователей
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> ConfirmNewStatus()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            if (Argumetns != null && Argumetns.Count == 2)
            {
                var OrderStatus = db.OrderStatus.Find(Argumetns[1]);

                OrderStatus.Enable = true;

                OrderStatus.Timestamp = DateTime.Now;

                this.Order.CurrentStatus = OrderStatus.Id;

                var order = db.Orders.Find(OrderStatus.OrderId);

                order.CurrentStatus = OrderStatus.Id;

                db.SaveChanges();

                db.Dispose();

                //меняем текущее сообщение на сообщение с описание заказа
                await GetOrderAdmin(base.MessageId);

                OrderStatusConfirmMessage statusConfirmMsg = new OrderStatusConfirmMessage(this.Order, OrderStatus);

                //уведомляем всех о новом статусе заказа
                await base.SendMessageAllBotEmployeess(statusConfirmMsg.BuildNotyfiMessage());

                if (OrderStatus.StatusId == Core.ConstantVariable.OrderStatusVariable.Completed)
                {
                    var stock = this.Order.UpdateStock();

                    FeedBackOfferMsg = new FeedBackOfferMessage(this.Order); // предлагаем пользователю оставить отзыв

                    await SendMessage(this.Order.Follower.ChatId, FeedBackOfferMsg.BuildMsg());

                    StockChangesMessage stockChangesMessage = new StockChangesMessage(stock, this.Order.Id);

                    SendMessageAllBotEmployeess(stockChangesMessage.BuildMsg());
                }

                return OkResult;
            }

            else
                return OkResult;
        }

        /// <summary>
        /// Добавить комментраий к новому статусу
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> AddCommentToStatus()
        {
            int OrderStatusId = 0;
            MarketBotDbContext db = new MarketBotDbContext();
            try
            {
                OrderStatusId= Convert.ToInt32(base.OriginalMessage.Substring(ForceReplyAddCommentToStatus.Length));

                OrderStatus orderStatus = db.OrderStatus.Find(OrderStatusId);

                orderStatus.Text = base.ReplyToMessageText;

                this.Order = db.Orders.Find(orderStatus.OrderId);

                db.SaveChanges();

                OrderStatusConfirmMessage statusConfirmMsg = new OrderStatusConfirmMessage(this.Order, orderStatus);

                await SendMessage(statusConfirmMsg.BuildMsg());

                return OkResult;
            }
            catch
            {
                return OkResult;
            }

            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Изменить статус заказа
        /// </summary>
        /// <param name="NewStatusId"></param>
        /// <returns></returns>
        private async Task<IActionResult> UpdateOrderStatus(int NewStatusId)
        {
            MarketBotDbContext db = new MarketBotDbContext();


            if (this.Order != null)
            {
                OrderStatus orderStatus = new OrderStatus
                {
                    Enable = false,
                    FollowerId = FollowerId,
                    OrderId = Order.Id,
                    Timestamp = DateTime.Now,
                    StatusId = NewStatusId
                };

                db.OrderStatus.Add(orderStatus);

                db.SaveChanges();


                db.Dispose();

                OrderStatusConfirmMessage statusConfirmMsg = new OrderStatusConfirmMessage(this.Order, orderStatus);

                await EditMessage(statusConfirmMsg.BuildMsg());

                if (NewStatusId == Core.ConstantVariable.OrderStatusVariable.Completed)
                {

                }

                return OkResult;
            }

            else
                return OkResult;
        }

        /// <summary>
        /// Отправить сообщения с кнопками статусов заказа
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendStatusEditor()
        {
            if (this.Order != null)
            {
                OrderStatusEditorMessage OrderStatusEditorMsg = new OrderStatusEditorMessage(this.Order);
                await EditMessage(OrderStatusEditorMsg.BuildMsg());
                return OkResult;
            }

            else
                return OkResult;


        }

        private async Task<IActionResult> SendInvoice()
        {
            try
            {
                if (this.Order.Invoice != null && InvoiceViewMsg!=null)
                    await EditMessage(InvoiceViewMsg.BuildMsg());

                if (this.Order.Invoice == null)
                    await base.AnswerCallback("Счет отсутствует. Способ оплаты при получении", true);

                return OkResult;
            }

            catch
            {
                return NotFoundResult;
            }
        }

        private async Task<IActionResult> GetOrder()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                try
                {
                    int number = Convert.ToInt32(base.CommandName.Substring(GetOrderCmd.Length));

                    int id = db.Orders.Where(o => o.Number == number).FirstOrDefault().Id;

                    OrderAdminMsg = new AdminOrderMessage(id);
                    await SendMessage(OrderAdminMsg.BuildMsg());

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
            try
            {
                if (Order != null)
                {
                    using (MarketBotDbContext db = new MarketBotDbContext())
                    {
                        OrdersInWork ordersInWork = new OrdersInWork
                        {
                            Timestamp = DateTime.Now,
                            InWork = false,
                            OrderId = Order.Id,
                            FollowerId = FollowerId
                        };

                        db.OrdersInWork.Add(ordersInWork);

                        if (db.SaveChanges() > 0)
                        {
                            Order.OrdersInWork.Add(ordersInWork);
                            OrderAdminMsg = new AdminOrderMessage(Order, FollowerId);
                            await base.EditMessage(OrderAdminMsg.BuildMsg());

                            string notify = "Пользователь " + GeneralFunction.FollowerFullName(FollowerId) + " освободил заказ №" + Order.Number.ToString();

                            var OrderMiniViewMsg = new OrderMiniViewMessage(notify, this.Order.Id);
                            await SendMessageAllBotEmployeess(OrderMiniViewMsg.BuildMsg());

                        }
                        return OkResult;
                    }
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
        /// Показать номер телефона покупателя
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> GetContact()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {

                var order = db.Orders.Where(o => o.Id == OrderId).Include(o => o.Follower).FirstOrDefault();

                if (order.Follower != null && order.Follower.Telephone != null && order.Follower.Telephone != "")
                {
                    Contact contact = new Contact
                    {
                        FirstName = order.Follower.FirstName,
                        PhoneNumber = order.Follower.Telephone

                    };

                    await SendContact(contact);

                }

                if (order.Follower != null && order.Follower.UserName != null && order.Follower.UserName != "")
                {
                    string url = Bot.BotMessage.HrefUrl("https://t.me/" + order.Follower.UserName, order.Follower.UserName);
                    await SendMessage(new BotMessage { TextMessage = url });
                    return OkResult;
                }

                else
                    return base.OkResult;
            }
        }

        /// <summary>
        /// Показать адрес доставки на карте
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendOrderAddressOnMap()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var Address = db.OrderAddress.Where(o => o.OrderId == OrderId).Include(o => o.Adress.House).FirstOrDefault();


                Location location = new Location
                {
                    Latitude = Convert.ToSingle(Address.Adress.House.Latitude),
                    Longitude = Convert.ToSingle(Address.Adress.House.Longitude)
                };

                if (await SendLocation(location) != null)
                    return OkResult;

                else
                    return NotFoundResult;
            }

        }


        private async Task<IActionResult> GetOrderAdmin(int MessageId=0)
        {
            if (OrderAdminMsg == null)
                OrderAdminMsg = new AdminOrderMessage(OrderId,FollowerId);

            if (await SendMessage(OrderAdminMsg.BuildMsg(),MessageId) != null)
                return base.OkResult;


            else
                return base.NotFoundResult;
        }

        /// <summary>
        /// Позиции заказа. Кнопками
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendEditorOrderPositionList()
        {
            if (await EditMessage(OrderPositionListMsg.BuildMsg()) != null)
                return base.OkResult;

            else
                return base.NotFoundResult;
        }

        private async Task<IActionResult> TakeOrder()
        {
            try
            {
                using(MarketBotDbContext db=new MarketBotDbContext())
                {
                    OrdersInWork inWork = new OrdersInWork { FollowerId = FollowerId, Timestamp = DateTime.Now, OrderId = Order.Id, InWork = true };
                    db.OrdersInWork.Add(inWork);

                    var InWorkNow = Order.OrdersInWork.OrderByDescending(o => o.Id).FirstOrDefault();

                    //заказ не находится ни у кого в обработке
                    if (Order!=null  && InWorkNow==null && db.SaveChanges() > 0 ||
                        Order != null && InWorkNow != null&&
                        InWorkNow.InWork==false && db.SaveChanges() > 0)
                    {
                        Order.OrdersInWork.Add(inWork);
                        OrderAdminMsg = new AdminOrderMessage(Order,FollowerId);
                        await EditMessage(OrderAdminMsg.BuildMsg());
                        string notify = "Заказ №" + this.Order.Number.ToString() + " взят в работу. Пользователь " + GeneralFunction.FollowerFullName(base.FollowerId);

                        var OrderMiniViewMsg = new OrderMiniViewMessage(notify, this.Order.Id);
                        await SendMessageAllBotEmployeess(OrderMiniViewMsg.BuildMsg());
                      
                    }

                    //заказ уже кем то обрабатывается
                    if (InWorkNow != null && InWorkNow.FollowerId != FollowerId && InWorkNow.InWork==true)

                    {
                        OverridePerformerConfirmMessage overridePerformerConfirmMsg = new OverridePerformerConfirmMessage(this.Order, InWorkNow);
                        var mess = overridePerformerConfirmMsg.BuildMsg();
                        await EditMessage(mess);
                    }

                    //заявка уже в обработке у пользователя
                    if (InWorkNow != null && InWorkNow.FollowerId == FollowerId && InWorkNow.InWork == true)
                    {
                        OrderAdminMsg = new AdminOrderMessage(Order, FollowerId);
                        await EditMessage(OrderAdminMsg.BuildMsg());
                    }

                        return OkResult;
                }
            }

            catch
            {
                return NotFoundResult;
            }
        }

        /// <summary>
        /// Предложение оставить отзыв
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendFeedBackOffer()
        {
            if (Order != null && Order.Follower != null && await SendMessage(Order.Follower.ChatId, FeedBackOfferMsg.BuildMsg()) != null)
                return OkResult;

            else
                return NotFoundResult;


        }

        /// <summary>
        /// Назад
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> BackToOrder()
        {
            if (OrderAdminMsg != null && await EditMessage(OrderAdminMsg.BuildMsg()) != null)
                return base.OkResult;

            if (OrderAdminMsg == null && this.OrderId > 0)
            {
                OrderAdminMsg = new AdminOrderMessage(this.OrderId);
                await EditMessage(OrderAdminMsg.BuildMsg());
                return OkResult;
            }

            else
            {
                OrderId = Argumetns[0];
                OrderAdminMsg = new AdminOrderMessage(this.OrderId);
                await EditMessage(OrderAdminMsg.BuildMsg());
                return OkResult;
            }

        }



    }
}
