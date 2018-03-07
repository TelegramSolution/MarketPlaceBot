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
    public partial class OrderBot
    {
        /// <summary>
        /// Заносим информацию о полученом платеже в бд и уведовляем об этом операторов
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SuccessfulPaymentCreditCard()
        {
            var id =Convert.ToInt32(this.Update.Message.SuccessfulPayment.InvoicePayload);

            this.OrderId = Convert.ToInt32(id);

            OrderFunction = new BusinessLayer.OrderFunction();

            OrderFunction.OpenConnection();

            var Payment= OrderFunction.AddCreditCardPayment(this.OrderId,
                (this.Update.Message.SuccessfulPayment.TotalAmount / 100),
                this.Update.Message.SuccessfulPayment.ProviderPaymentChargeId,
                "идентификатор платежа в Telegram:" + this.Update.Message.SuccessfulPayment.TelegramPaymentChargeId);

            if (Payment!=null)
                await SendMessageAllBotEmployeess(new BotMessage { TextMessage = "Поступил новый платеж. Заказ /order" + Order.Number.ToString() });
            
            return OkResult;
        }

        /// <summary>
        /// отправить инвойс для оплаты банковской картой внутри бота
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendDebitCardInvoice()
        {
            TelegramDebitCardInvoice telegramDebitCardInvoice = new TelegramDebitCardInvoice(this.Order, base.BotInfo);
            var invoice=telegramDebitCardInvoice.CreateInvoice();

            await base.SendInvoice(invoice);

            return OkResult;
        }

        /// <summary>
        /// Проверка данных перед совершением оплаты через банковскую карту
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> answerPreCheckoutOrder()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            this.OrderId = Convert.ToInt32(base.Update.PreCheckoutQuery.InvoicePayload);

            this.Order = db.Orders.Where(o => o.Id == OrderId).Include(o=>o.Invoice).Include(o => o.CurrentStatusNavigation).FirstOrDefault();

            db.Dispose();

            if (this.Order.CurrentStatusNavigation.StatusId==ConstantVariable.OrderStatusVariable.Canceled)
                await answerPreCheckoutQuery(false,"Заказ отменен");

            if (this.Order.CurrentStatusNavigation.StatusId == ConstantVariable.OrderStatusVariable.Completed)
                await answerPreCheckoutQuery(false, "Заказ уже выполнен");

            if(this.Order.Paid==true)
                await answerPreCheckoutQuery(false, "Заказ уже оплачен");


            if (this.Order.Invoice.CreateTimestamp.Value.Date!=DateTime.Today &&
                DateTime.Now.TimeOfDay > (this.Order.Invoice.CreateTimestamp.Value.TimeOfDay + this.Order.Invoice.LifeTimeDuration))
            {
                await answerPreCheckoutQuery(false, "Вы должны были оплатить заказ до " +
                    (this.Order.Invoice.CreateTimestamp.Value.TimeOfDay + this.Order.Invoice.LifeTimeDuration).Value.ToString());
            }
   

            else
                await answerPreCheckoutQuery(true);

            return OkResult;
        }

        /// <summary>
        /// Сохранить отзыв и отправить его все операторам и владельцу 
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SaveFeedback()
        {
            if (Argumetns.Count > 0)
            {
                int FeedBackId = Argumetns[0];

                var feedbak= FeedbackFunction.EnableFeedback(FeedBackId);

                FeedBackToProductEditorMsg = new FeedBackToProductEditorMessage(feedbak);

                await EditMessage(FeedBackToProductEditorMsg.BuildMsg());

                await SendMessageAllBotEmployeess(
                    new BotMessage
                    {
                        TextMessage = "Добавлен новый отзыв к заказу " + 
                                     Order.Number.ToString() + 
                                     " /order" + Order.Number.ToString()
                    });

            }

            return OkResult;
        }

        /// <summary>
        /// Сохранить комментарий к отзыву
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SaveFeedBackComment()
        {
            int id;

            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                id = Convert.ToInt32(OriginalMessage.Substring(AddCommentFeedBackForce.Length));

                var feedback = db.FeedBack.Find(id);

                this.Order = db.Orders.Where(o => o.Id == feedback.OrderId).Include(o => o.Follower).FirstOrDefault();

                if (feedback != null && this.Order.Follower.ChatId==base.ChatId)
                {
                    feedback.Text = ReplyToMessageText;
                    db.SaveChanges();

                    FeedBackToProductEditorMsg = new FeedBackToProductEditorMessage(feedback.Id);
                    var mess = FeedBackToProductEditorMsg.BuildMsg();
                    await SendMessage(mess);
                   
                }

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
        /// ПОльзотватель нажал на кнопку добавить комм. к отзыву.В ответ ему приходит forcereply сообщение
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> AddCommentFeedback()
        {
            if (Argumetns.Count == 2)
                return await SendForceReplyMessage(AddCommentFeedBackForce + Argumetns[1].ToString());

            else
                return OkResult;
        }

        /// <summary>
        /// Добавить отзыв в таблицу после нажатия на кнопку с оценкой
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> InsertFeedBack()
        {
            if (Argumetns.Count == 3)
            {
                int OrderId = Argumetns[0];
                int ProductId = Argumetns[1];
                int Raiting = Argumetns[2];

                var feedBack= FeedbackFunction.InsertFeedBack(Raiting, ProductId, OrderId);

                FeedBackToProductEditorMsg = new FeedBackToProductEditorMessage(feedBack);
                var mess = FeedBackToProductEditorMsg.BuildMsg();
                await EditMessage(mess);
               
            }

            return OkResult;
        }

        /// <summary>
        /// назад к сообещнию с отзывом к заказу
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> BackToFeedBackView()
        {
            //у пользователя было сообщение с добавлением отзыва к товару,
            //он нажал назад и вернулся на сообщение с отображением отзыва ко всему заказу
            //проверяем добавил ли он уже оценку к товару из этого заказа. Если добавил,
            //то удаляем
            if (Argumetns.Count == 2)
                FeedbackFunction.RemoveFeedBack(Argumetns[1]); 

            return await SendFeedBackMyOrder(Argumetns[0]);
        }

        /// <summary>
        /// отправить сообщение с кнопками добавления отзыва к товару
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendFeedBackToProductEditor()
        {
            if(Argumetns.Count==2)
                FeedBackToProductEditorMsg = new FeedBackToProductEditorMessage(Argumetns[0], Argumetns[1]);

            if(Argumetns.Count==1)
                FeedBackToProductEditorMsg = new FeedBackToProductEditorMessage(Argumetns[0]);

            var mess = FeedBackToProductEditorMsg.BuildMsg();

            if (mess != null)
                await EditMessage(mess);

            return OkResult;
        }

        /// <summary>
        /// Отправить сообщение с отзывом к заказу
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        private async Task<IActionResult> SendFeedBackMyOrder(int OrderId)
        {
            FeedBackOfferMessage FeedBackOfferMsg = new FeedBackOfferMessage(OrderId);

            var mess = FeedBackOfferMsg.BuildMsg();

            await EditMessage(mess);

            return OkResult;
        }


        /// <summary>
        /// Пользователь выбрал адрес доставки. Сохраняем и предлагаем выбрать способ оплаты
        /// </summary>
        /// <param name="AddressId"></param>
        /// <returns></returns>
        private async Task<IActionResult> SelectAddressDelivery(int AddressId)
        {
            OrderFunction = new OrderFunction();

            OrderFunction.OpenConnection();

            if (OrderFunction.AddAddressToOrderTmp(FollowerId, BotInfo.Id, AddressId) != null)
            {
                OrderFunction.Dispose();
                return await SendPaymentMethodsList();
            }
            else
                await AnswerCallback("Произошла ошибка при выборе адреса доставки", true);

            return OkResult;
        }
        

        /// <summary>
        /// ПОказать одним сообщеним все адреса пользователя
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendAddressList(int MessageId = 0)
        {
            AddressListMessage ViewAddressListMsg = new AddressListMessage(base.FollowerId);

            await SendMessage(ViewAddressListMsg.BuildMsg(), MessageId);

            return base.OkResult;
        }

        /// <summary>
        /// Отправить сообщение со способами получения заказа
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendMethodOfObtainingList()
        {
            MethodOfObtainingMsg = new MethodOfObtainingMessage(base.BotInfo.Name);

            await EditMessage(MethodOfObtainingMsg.BuildMsg());

            return base.OkResult;
        }

        /// <summary>
        /// Отправить сообщение со списком пунктов самовывоза
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendPickupPointList()
        {
            PickupPointListMsg = new PickupPointListMessage();

            await EditMessage(PickupPointListMsg.BuildMsg());

            return OkResult;

        }

        /// <summary>
        /// Пользователь выбрал пункт самовывоза. Сохраняем
        /// </summary>
        /// <param name="PickupPointId"></param>
        /// <returns></returns>
        private async Task<IActionResult> SelectPickupPoint(int PickupPointId)
        {
            OrderFunction = new OrderFunction();

            OrderFunction.OpenConnection();

            if (OrderFunction.AddPickUpPointToOrderTmp(FollowerId, BotInfo.Id, PickupPointId) != null)
            {
                OrderFunction.Dispose();
                return await SendPaymentMethodsList();
            }

            else
               await AnswerCallback("Произошла ошибка при выборе пункта самовывозы", true);

            return OkResult;
        }

        private async Task<IActionResult> SendInvoice()
        {
            try
            {
                using (MarketBotDbContext db = new MarketBotDbContext())
                {

                    if (Order != null  && Order.Invoice!=null)
                    {
                        InvoiceViewMsg = new InvoiceViewMessage(Order.Invoice, Order.Id, "BackToMyOrder");
                        await EditMessage(InvoiceViewMsg.BuildMsg());
                        return OkResult;
                    }

                    else 
                        return OkResult;
                 }
            }

            catch
            {
                return OkResult;
            }
        }

        /// <summary>
        /// Показать заказ пользователя. Команда /myorder
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendMyOrder()
        {
            try
            {
                int number = Convert.ToInt32(base.CommandName.Substring(MyOrder.Length));

                OrderFunction = new OrderFunction();

                OrderFunction.OpenConnection();

                this.Order = OrderFunction.GetFollowerOrder(number, FollowerId);

                if (this.Order != null)
                {
                    OrderViewMsg = new OrderViewMessage(this.Order);
                    await SendMessage(OrderViewMsg.BuildMsg());
                }

                return base.OkResult;
                
            }

            catch
            {
                return base.OkResult;
            }
        }

        private async Task<IActionResult> BackToOrder()
        {                
            OrderViewMsg = new OrderViewMessage(this.Order);
            await EditMessage(OrderViewMsg.BuildMsg());
            return OkResult;
        }

        /// <summary>
        /// Отправить пользователю список его заказов
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendMyOrderList()
        {
            try
            {
                if (MyOrdersMsg != null)
                {
                    await SendMessage(MyOrdersMsg.BuildMsg());
                    return OkResult;
                }

                else
                {
                    MyOrdersMsg = new MyOrdersMessage(base.FollowerId,BotInfo.Id);
                    await SendMessage(MyOrdersMsg.BuildMsg());
                    return OkResult;
                }
            }

            catch
            {
                return OkResult;
            }
        }

        /// <summary>
        /// Пользователь выбрал методо оплаты
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SelectPaymentMethod()
        {
            OrderFunction = new OrderFunction();

            OrderFunction.OpenConnection();

            int PaymentTypeId=0;

            bool TestConnection = false;

            bool ExistUserName = false;

            bool ExistTelephone = false;

            OrderTemp orderTemp = null;

            ConfigurationBot=GetConfigurationBot(BotInfo.Id);

            if (Argumetns.Count > 0)
            {
                PaymentTypeId = Argumetns[0];

                TestConnection = OrderFunction.PaymentTypeTestConnection(PaymentTypeId);
            }

            if (TestConnection == false)
            {
                await AnswerCallback("Ошибка. Данный способ оплаты недоступен!", true);
                OrderFunction.Dispose();
                return OkResult;
            }

            if (TestConnection)
            {
                orderTemp = OrderFunction.AddPaymentMethodToOrderTmp(FollowerId, BotInfo.Id, PaymentTypeId);
                OrderFunction.Dispose();
            }


            //Данные о выбраном способоне оплаты успешно занесены в БД.
            //Если в настройках бота включена верификация по номеру телефона, то проверяем указан ли номер телефона у этого пользователя
            //Если не указан то просим указать
            ExistTelephone = FollowerFunction.ExistTelephone(FollowerId);
            if (orderTemp!=null && ConfigurationBot != null && ConfigurationBot.VerifyTelephone && !ExistTelephone)
                await SendMessage(RequestPhoneNumberMsg.BuildMsg());

            //телефон указан
            if (orderTemp != null && ConfigurationBot != null && ConfigurationBot.VerifyTelephone && ExistTelephone)
                return await SendOrderTemp();


            //Данные о выбраном способоне оплаты успешно занесены в БД.
            //Если в настройках бота верификация по телефону отключена, проверяем указан ли у пользователя UserName 
            ExistUserName = FollowerFunction.ExistUserName(FollowerId);
            if (orderTemp!=null && ConfigurationBot != null && !ConfigurationBot.VerifyTelephone && !ExistUserName)
                return await SendUserNameAddedFaq();

            //UserName указан
            if (orderTemp != null && ConfigurationBot != null && !ConfigurationBot.VerifyTelephone && ExistUserName)
                return await SendOrderTemp();


            return OkResult;
        }


        /// <summary>
        /// После того, как пользователь выбрал адрес доставки, Отправляем сообщение с выбором варианта оплаты
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendPaymentMethodsList()
        {
            var message = PaymentsMethodsListMsg.BuildMsg();

            if (message != null && await EditMessage(message)!=null)            
                return OkResult;
            

            else // если сообщение с вариантами оплаты пустое, значит все варианты оплаты выключены. Отправляем пользователю
            //сообщение с оишбкой
            {
                await SendMessage(new BotMessage { TextMessage = "Нет досутпных способов оплаты. Свяжитесь с технической поддержкой /help" });

                return OkResult;
            }

           
        }

 
        /// <summary>
        /// отправить сообщение с инструкцией о том как добавить userName в Telegram
        /// </summary>
        /// <param name="configuration">Конфигурация бота. Может быть пустым в случае когда пользователь нажал далее</param>
        /// <param name="PaymentType">Тип оплаты</param>
        /// <returns></returns>
        private async Task<IActionResult> SendUserNameAddedFaq(Configuration configuration=null)
        {
            UserNameImageMessage userNameImage = new UserNameImageMessage(BotInfo.Configuration);

            var message = userNameImage.BuildMsg();

            var PhotoSend= await SendPhoto(message);

            // добавляем ID файла в бд, что бы потом не отправлять сам файл,а только ID на сервере телегарм
            if (BotInfo.Configuration != null && BotInfo.Configuration.UserNameFaqFileId == null && PhotoSend != null)
               ConfigurationFunction.AddFileIdUserNameFaqImage(BotInfo.Configuration,PhotoSend.Photo[PhotoSend.Photo.Length - 1].FileId);

            return OkResult;
        }

        /// <summary>
        /// Добавить UserName для пользователя
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> AddUserName()
        {
            //Пользователь написал username в настройках Telegram  и нажал на кнопку
            if (Update.CallbackQuery != null && Update.CallbackQuery.From != null &&
                FollowerFunction.AddUserName(FollowerId, Update.CallbackQuery.From.Username) != null)
                return await SendOrderTemp();

            else // Не написал
                return await SendUserNameAddedFaq();
        }
        /// <summary>
        /// Сообщение с деталями Заказа из таблицы OrderTemp
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendOrderTemp()
        {
            if (OrderPreviewMsg == null)
                OrderPreviewMsg = new OrderTempMessage(base.FollowerId,BotInfo.Id);

            var message = OrderPreviewMsg.BuildMessage();

            if (message != null)
                await EditMessage(message);

            if (message == null)
                await AnswerCallback("Корзина пуста", true);

            return OkResult;

        }


        /// <summary>
        /// Добавить комментарий к заказу. БД
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> AddCommentToOrderTmp()
        {
            OrderFunction = new OrderFunction();

            OrderFunction.OpenConnection();

            OrderFunction.AddCommentToOrderTmp(FollowerId,BotInfo.Id, Update.Message.Text);

            OrderFunction.Dispose();

            OrderPreviewMsg = new OrderTempMessage(FollowerId, BotInfo.Id);

            await SendMessage(OrderPreviewMsg.BuildMessage());

            return base.OkResult;


        }



        /// <summary>
        /// Изменить адрес доставки для заказка
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendAddressEditor()
        {

            if (await EditMessage(ViewShipAddressMsg.BuildMsg()) != null)
                return base.OkResult;

            else
                return base.OkResult;
        }

        /// <summary>
        /// Сохрнанить
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> OrderSave()
        {
            Orders new_order = null;
            bool blocked = false;

            OrderFunction orderFunction = new OrderFunction();

            orderFunction.OpenConnection();

            ConfigurationBot = base.GetConfigurationBot(BotInfo.Id);

            blocked = FollowerFunction.IsBlocked(FollowerId);

            if (blocked)
                await AnswerCallback("Вы заблокированы администратором системы!", true);

            // если в настройках бота указано время работы магазина, то проверяем подходит ли текщее время 
            //под это правило. Если подходит то офрмляем заказ
            if (!blocked && ConfigurationBot.StartTime!=null &&
                ConfigurationBot.EndTime!=null && ConfigurationBot.StartTime.Value.Hours <=
                DateTime.Now.Hour && ConfigurationBot.StartTime.Value<= DateTime.Now.TimeOfDay &&
                 ConfigurationBot.EndTime.Value>DateTime.Now.TimeOfDay)
                      new_order= orderFunction.CreateOrder(FollowerId,BotInfo);
                

            //Время работы магазина не указано.
            else if (!blocked && ConfigurationBot.EndTime==null && ConfigurationBot.StartTime==null)
                new_order = orderFunction.CreateOrder(FollowerId, BotInfo);

            else
                await AnswerCallback("Мы обрабатываем заказы только в период с "+ConfigurationBot.StartTime.ToString()+ 
                    " и по "+ConfigurationBot.EndTime.ToString() , true);

            if (new_order != null && new_order.Invoice != null)
            {
                InvoiceViewMsg = new InvoiceViewMessage(new_order.Invoice, new_order.Id, "BackToMyOrder");
                await EditMessage(InvoiceViewMsg.BuildMsg());
            }

            if(new_order!=null && new_order.Invoice == null)
            {
                OrderViewMsg = new OrderViewMessage(new_order);
                await EditMessage(OrderViewMsg.BuildMsg());
            }

            //то отправляем уведомление о новом заказке Админам
            if (new_order != null)
            {
                OrderAdminMsg = new AdminOrderMessage(new_order);

                var message = OrderAdminMsg.BuildMsg();

                await SendMessageAllBotEmployeess(message);
            }


            OrderFunction.Dispose();

            return OkResult;
        }



        private async Task<IActionResult> CheckPay()
        {

           var mess= await CheckPayMsg.BuildMessage();

            await AnswerCallback(mess.TextMessage, true);

            if (mess.Order.Paid == true)
            {
                InvoiceViewMsg = new InvoiceViewMessage(mess.Order.Invoice, mess.Order.Id, "BackToMyOrder");
                await EditMessage(InvoiceViewMsg.BuildMsg());


                await SendMessageAllBotEmployeess(new BotMessage { TextMessage= "Поступил новый платеж. Заказ /order" + mess.Order.Number.ToString() });
            }

            return OkResult;
        }


    }
}
