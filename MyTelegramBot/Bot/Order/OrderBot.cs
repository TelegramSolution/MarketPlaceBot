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
using MyTelegramBot.BusinessLayer;

namespace MyTelegramBot.Bot
{
    public partial class OrderBot:BotCore
    {
        public const string ModuleName = "Order";

        public const string ViewInvoiceCmd = "ViewInvoice";

        public const string BackToMyOrderCmd = "BackToMyOrder";

        public OrderFunction OrderFunction { get; set; }

      
        private int OrderId { get; set; }

        private Orders Order { get; set; }

        const string CmdAddDescToOrder = "Добавить комментарий к заказу";

        public const string CmdEnterDesc = "Введите комментарий";

        /// <summary>
        /// Выбран пункт самовывоза
        /// </summary>
        public const string SelectPickupPointCmd = "SelectPickupPoint";


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


        public const string MyOrder = "/myorder";


        private const string ForceReplyAddFeedBack = "Добавить отзыв к заказу:";

        private const string GetOrderCmd = "/order";

        /// <summary>
        /// Выбран один из варинатов оплаты
        /// </summary>
        public const string SelectPaymentMethodCmd = "SelectPaymentMethod";

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

        protected override void Initializer()
        {
            if (Update.Message != null && Update.Message.ReplyToMessage != null)
                CommandName = Update.Message.ReplyToMessage.Text;

            try
            {
                if (base.Argumetns.Count > 0)
                {
                    OrderId = Argumetns[0];
                    using (MarketBotDbContext db = new MarketBotDbContext())
                        Order = db.Orders.Where(o => o.Id == this.OrderId).
                            Include(o => o.OrderProduct).Include(o => o.Follower).Include(o => o.FeedBack).Include(o=>o.Invoice).FirstOrDefault();

                }


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
                    return await base.SendForceReplyMessage(CmdEnterDesc);

                /// Поользователь присал новый комментриай к заказу процитировав сообщение бота "Введите комментарий".
                /// Коммент сохрнаятеся в бд,а После этого бот присылает
                /// обновелнное описание заказа
                case CmdEnterDesc:
                    return await AddCommentToOrderTmp();

                //Пользователь нажал на кнопку "Отправить заказ"
                case CmdOrderSave:
                    return await OrderSave();

                ///Пользователь выбрал адрес доставки
                case CmdSelectAddress:
                    return await SelectAddressDelivery(Argumetns[0]);

                //Пользователь нажал на один из доступных вариантов оплаты
                case SelectPaymentMethodCmd:
                    return await SelectPaymentMethod();

                //Пользователь записал свой ник в настройках, и нажал далее на картинке
                case "VerifyUserName":
                    return await AddUserName();

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
