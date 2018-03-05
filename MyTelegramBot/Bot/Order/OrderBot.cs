using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Bot
{
    public partial class OrderBot:BotCore
    {
        public const string ModuleName = "Order";

        /// <summary>
        /// Сообщение с просибой отправить свой номер телефона
        /// </summary>
        private RequestPhoneNumberMessage RequestPhoneNumberMsg { get; set; }

        /// <summary>
        /// Сообщение с предаварительным описанием заказа. Без номер и т.д
        /// </summary>
        private OrderTempMessage OrderPreviewMsg { get; set; }

        /// <summary>
        /// Сообщение с вариантами оплаты
        /// </summary>
        private PaymentsMethodsListMessage PaymentsMethodsListMsg { get; set; }

        /// <summary>
        /// Адреса пользователя
        /// </summary>
        private AddressListMessage ViewShipAddressMsg { get; set; }

        /// <summary>
        /// Сообщение с описанием заказа
        /// </summary>
        private OrderViewMessage OrderViewMsg { get; set; }

       
        private NewFeedBackAddedMessage NewFeedBackAddedMsg { get; set; }

        /// <summary>
        /// Сообщение с позициями заказа. Каждай позиция отдельная кнопка
        /// </summary>

        private MyOrdersMessage MyOrdersMsg { get; set; }

        /// <summary>
        /// сообщение с кнопками добавления отзыва к товару
        /// </summary>
        private FeedBackToProductEditorMessage FeedBackToProductEditorMsg { get; set; }

        private Messages.Admin.AdminOrderMessage OrderAdminMsg { get; set; }

        private InvoiceViewMessage InvoiceViewMsg { get; set; }

        private CheckPayMessage CheckPayMsg { get; set; }

        private MethodOfObtainingMessage MethodOfObtainingMsg { get; set; }

        /// <summary>
        /// сообщение с кнопками оценок для отзыва к заказу от 1 до 5
        /// </summary>
        private RaitingMessage RaitingMsg { get; set; }

        private int OrderId { get; set; }

        private Orders Order { get; set; }

        const string CmdAddDescToOrder = "Добавить комментарий к заказу";

        public const string CmdEnterDesc = "Введите комментарий";

        /// <summary>
        /// Выбран пункт самовывоза
        /// </summary>
        public const string SelectPickupPointCmd = "SelectPickupPoint";

        /// <summary>
        /// сообщение со списком пунктов самовывоза
        /// </summary>
        public PickupPointListMessage PickupPointListMsg { get; set; }

        /// <summary>
        /// Пользователь выбрал адрес доставки
        /// </summary>
        public const string CmdSelectAddress = "SelectAddress";

        /// <summary>
        /// Изменить адрес доставки заказа
        /// </summary>
        public const string CmdAddressEditor = "AddressEditor";

        /// <summary>
        /// Добавить комментарий к заказу
        /// </summary>
        public const string CmdOrderDesc = "OrderDesc";

        /// <summary>
        /// Отправить заказ на рассмотрение
        /// </summary>
        public const string CmdOrderSave = "OrderSave";

        public const string CmdBackFeedBackView = "BackFeedBackView";

        public const string CmdAddFeedBackProduct = "AddFbToPrdct";

        public const string CmdFeedBackRaiting = "FeedBackRaiting";

        /// <summary>
        /// Показать адрес доставки на карте
        /// </summary>
        public const string CmdViewAddressOnMap = "ViewAddressOnMap";

        public const string CmdAddFeedBack = "AddFeedBack";

        public const string MyOrdersListCmd = "MyOrdersList";

        public const string MyOrder = "/myorder";


        private const string ForceReplyAddFeedBack = "Добавить отзыв к заказу:";

        private const string GetOrderCmd = "/order";

        /// <summary>
        /// Выбран один из варинатов оплаты
        /// </summary>
        public const string PaymentMethodCmd = "GetPaymentMethod";

        /// <summary>
        /// Список все доступных варинатов оплаты
        /// </summary>
        public const string GetPaymentMethodListCmd = "GetPaymentMethodList";

        /// <summary>
        ///Кнопка Я оплатил
        /// </summary>
        public const string CheckPayCmd = "CheckPay";

        private const int QiwiPayMethodId = 2;

        private const int PaymentOnReceipt = 1;

        /// <summary>
        /// Выбрать способо получения заказа
        /// </summary>
        public const string SelectMethodOfObtainingCmd= "SelectMethodOfObtaining";

        /// <summary>
        /// Показать способы получения заказа
        /// </summary>
        public const string MethodOfObtainingListCmd = "MethodOfObtainingList";

        public const int IsDeliveryId = 1;

        public const string IsDeliveryName = "Доставка";

        public const int IsPickupId= 2;

        public const string IsPickupName = "Самовывоз";

        /// <summary>
        /// Оценка к отзыву
        /// </summary>
        public const string SelectRaitingCmd = "SelectRaiting";

        public const string CmdAddCommentFeedBack = "AddCommentFeedBack";

        public const string AddCommentFeedBackForce = "Добавить комментарий к отзыву:";

        public const string CmdSaveFeedBack = "SaveFeedBack";

        public const string CmdDebitCardСheckout = "DebitCardСheckout";

        int AddressId { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_update"></param>
        public OrderBot(Update _update) : base(_update)
        {            
         

        }

        protected override void Constructor()
        {
            if (Update.Message != null && Update.Message.ReplyToMessage != null)
                CommandName = Update.Message.ReplyToMessage.Text;

            try
            {
                PaymentsMethodsListMsg = new PaymentsMethodsListMessage();
                if (base.Argumetns.Count > 0)
                {
                    OrderId = Argumetns[0];
                    OrderViewMsg = new OrderViewMessage(this.OrderId);
                    MyOrdersMsg = new MyOrdersMessage(base.FollowerId,BotInfo.Id);
                    using (MarketBotDbContext db = new MarketBotDbContext())
                        Order = db.Orders.Where(o => o.Id == this.OrderId).Include(o => o.Confirm).
                            Include(o => o.Done).Include(o => o.Delete).
                            Include(o => o.OrderProduct).Include(o => o.Follower).Include(o => o.FeedBack).Include(o=>o.Invoice).FirstOrDefault();
                    CheckPayMsg = new CheckPayMessage(Order);

                }

                RequestPhoneNumberMsg = new RequestPhoneNumberMessage(base.FollowerId);
                ViewShipAddressMsg = new AddressListMessage(base.FollowerId);
                OrderPreviewMsg = new OrderTempMessage(base.FollowerId,BotInfo.Id);

            }

            catch
            {

            }

            finally
            {

            }
        }

        public async override Task<IActionResult> Response()
        {
            switch (base.CommandName)
            {
                //Пользователь нажал на кнопку Перейти к оформлению заказа
                //Сообщение с корзиной менятеся на сообщение со способами получения заказа
                case MethodOfObtainingListCmd:
                    return await SendMethodOfObtainingList();

                ///Поользователь выбрал адрес доставки нажав на кнопку,
                ///далее появляется сообщение с выбором метода оплаты, если доступны больше 1ого метода,
                ///если доступен только один метод, то появляется сообщение с заказом
                case GetPaymentMethodListCmd:
                   return await SendPaymentMethodsList();

                ///Польватель нажал изменить адрес доставки. 
                ///Сообщение с описание заказа редактируется на сообщение со списком его адресов
                case CmdAddressEditor:
                    return await SendAddressEditor();

                ///Пользвовательно нажал на кнопку "Комментарий к заказу"
                case CmdOrderDesc:
                    return await SendForceReplyAddDesc();

                /// Поользователь присал новый комментриай к заказу процитировав сообщение бота "Введите комментарий".
                /// Коммент сохрнаятеся в бд,а После этого бот присылает
                /// обновелнное описание заказа
                case CmdEnterDesc:
                    return await AddOrderTempDesc();

                //Пользователь нажал на кнопку "Отправить заказ"
                case CmdOrderSave:
                    return await OrderSave();

                ///Пользователь выбрал адрес доставки
                case CmdSelectAddress:
                    return await SelectAddressDelivery(Argumetns[0]);

                //Пользователь нажал на один из доступных вариантов оплаты
                case PaymentMethodCmd:
                    return await SelectPaymentMethod();

                //Пользователь нажал на кнопку Мои заказы
                case MyOrdersListCmd:
                    return await SendMyOrderList();

                //Пользователь записал свой ник в настройках, и нажал далее на картинке
                case "VerifyUserName":
                    return await UserNameCheck();

                case "BackToMyOrder":
                   return await BackToOrder();

                case "ViewInvoice":
                    return await SendInvoice();

                case CheckPayCmd:
                    return await CheckPay();

                case SelectPickupPointCmd:
                    return await SelectPickupPoint(Argumetns[0]);

                case CmdAddFeedBack:
                    return await SendFeedBackMyOrder(Argumetns[0]);

                case CmdDebitCardСheckout:
                    return await SendDebitCardInvoice();

                    //пользователь нажал на кнопку с товаром что бы добавить к нему отзыв
                case CmdAddFeedBackProduct:
                    return await SendFeedBackToProductEditor();

                case CmdFeedBackRaiting:
                    return await InsertFeedBack();

                case CmdAddCommentFeedBack:
                    return await AddCommentFeedback();

                case CmdSaveFeedBack:
                    return await SaveFeedback();

                case CmdBackFeedBackView:
                    return await BackToFeedBackView();


                default:
                    break;
                    
            }

            ///Пользователь выбрал способ получения заказа "Доставка" Отправляем ему списко его адресов
            if (base.CommandName == SelectMethodOfObtainingCmd && Argumetns[0] == IsDeliveryId)
                return await SendAddressList(base.MessageId);

            ///Пользователь выбрал способо получения заказа "Самовывоз"
            if (base.CommandName == SelectMethodOfObtainingCmd && Argumetns[0] == IsPickupId)
                return await SendPickupPointList();

            //Пользоватлеь отправил команду /myorder
            if (base.CommandName.Contains(MyOrder))
                return await SendMyOrder();

            //Пользователь нажал на кнопку "Я оплатил"
            if (base.CommandName == CheckPayCmd)
                return OkResult;

            if (base.OriginalMessage.Contains(AddCommentFeedBackForce))
                return await SaveFeedBackComment();

            if (Update.PreCheckoutQuery != null)
                return await answerPreCheckoutOrder();

            if (Update.Message!=null && Update.Message.SuccessfulPayment != null) // поступил платеж через банк. карту.
                return await SuccessfulPaymentCreditCard();

            else
                return null;
           
            
        }

       
    }

}
