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
    public partial class OrderBot
    {

        private async Task<IActionResult> answerPreCheckoutOrder()
        {
            await answerPreCheckoutQuery(true);
            return OkResult;
        }

        /// <summary>
        /// Сохранить отзыв и отправить его все операторам и владельцу 
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SaveFeedback()
        {
            using (MarketBotDbContext db=new MarketBotDbContext())
            {
                var feedback = db.FeedBack.Find(Argumetns[1]);

                if (feedback != null)
                {
                    feedback.DateAdd = DateTime.Now;
                    feedback.Enable = true;
                    db.SaveChanges();

                    FeedBackToProductEditorMsg = new FeedBackToProductEditorMessage(feedback.Id);

                    await EditMessage(FeedBackToProductEditorMsg.BuildMsg());

                    await SendMessageAllBotEmployeess(new BotMessage { TextMessage = "Добавлен новый отзыв к заказу " + Order.Number.ToString() + " /order" + Order.Number.ToString() });
                }

                    return OkResult;
            }
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
                return NotFoundResult;
        }

        /// <summary>
        /// Добавить отзыв в таблицу после нажатия на кнопку с оценкой
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> InsertFeedBack()
        {
            if (Argumetns.Count == 3)
            {
                MarketBotDbContext db = new MarketBotDbContext();

                try
                {
                    FeedBack feedBack = new FeedBack
                    {
                        OrderId = Argumetns[0],
                        ProductId = Argumetns[1],
                        RaitingValue = Argumetns[2],
                        Enable = false
                    };

                    db.FeedBack.Add(feedBack);
                    db.SaveChanges();

                    FeedBackToProductEditorMsg = new FeedBackToProductEditorMessage(feedBack.Id);
                    var mess = FeedBackToProductEditorMsg.BuildMsg();
                    await EditMessage(mess);
                }

                catch (Exception e)
                {

                }

                finally
                {
                    db.Dispose();
                }
                
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

            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                if (Argumetns.Count == 2)
                {
                    var Feed = db.FeedBack.Find(Argumetns[1]);
                    db.FeedBack.Remove(Feed);
                    db.SaveChanges();
                }
            }

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
        /// Пользователь выбрал адрес доставки. Сохраняем
        /// </summary>
        /// <param name="AddressId"></param>
        /// <returns></returns>
        private async Task<IActionResult> SelectAddressDelivery(int AddressId)
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var OrderTemp = db.OrderTemp.Where(o => o.BotInfoId == BotInfo.Id && o.FollowerId==FollowerId).Include(o=>o.PickupPoint).FirstOrDefault();

                if (OrderTemp != null)
                {
                    OrderTemp.AddressId = AddressId;
                    OrderTemp.PickupPoint = null;

                }

                else
                {
                    OrderTemp orderTemp = new OrderTemp
                    {
                        FollowerId = FollowerId,
                        BotInfoId = BotInfo.Id,
                        AddressId = AddressId,
                    };

                    db.OrderTemp.Add(orderTemp);
                    int save = db.SaveChanges();
                }

               

                return await SendPaymentMethodsList();
            }
        }

        /// <summary>
        /// ПОказать одним сообщеним все адреса пользователя
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendAddressList(int MessageId = 0)
        {
            AddressListMessage ViewAddressListMsg = new AddressListMessage(base.FollowerId);
            if (await SendMessage(ViewAddressListMsg.BuildMsg(), MessageId) != null)
                return base.OkResult;

            else
                return base.NotFoundResult;
        }

        /// <summary>
        /// Отправить сообщение со способами получения заказа
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendMethodOfObtainingList()
        {
            MethodOfObtainingMsg = new MethodOfObtainingMessage(base.BotInfo.Name);
            if (await EditMessage(MethodOfObtainingMsg.BuildMsg()) != null)
                return base.OkResult;

            else
                return NotFoundResult;
        }

        /// <summary>
        /// Отправить сообщение со списком пунктов самовывоза
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendPickupPointList()
        {
            PickupPointListMsg = new PickupPointListMessage();

            if (await EditMessage(PickupPointListMsg.BuildMsg()) != null)
                return OkResult;

            else
                return NotFoundResult;
        }

        private async Task<IActionResult> SelectPickupPoint(int PickupPointId)
        {
            using(MarketBotDbContext db=new MarketBotDbContext())
            {
                var OrderTemp = db.OrderTemp.Where(o => o.BotInfoId == BotInfo.Id).Include(o=>o.Address).FirstOrDefault();

                if (OrderTemp != null)
                {
                    OrderTemp.PickupPointId = PickupPointId;
                    OrderTemp.Address = null;
                }

                else
                {
                    OrderTemp orderTemp = new OrderTemp
                    {
                        FollowerId = FollowerId,
                        BotInfoId = BotInfo.Id,
                        PickupPointId = PickupPointId,
                    };

                    db.OrderTemp.Add(orderTemp);
                }

                db.SaveChanges();

                return await SendPaymentMethodsList();
            }
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
                        return NotFoundResult;
                 }
            }

            catch
            {
                return NotFoundResult;
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
                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    Orders order = new Orders();

                    // Пользователь будет видеть заказы оформелнные через других ботов
                    order = db.Orders.Where(o => o.Number == number && o.FollowerId == FollowerId).Include(o => o.Confirm)
                       .Include(o => o.Delete).Include(o => o.Done).Include(o => o.FeedBack).
                        Include(o => o.OrderProduct).Include(o => o.OrderAddress).Include(o=>o.PickupPoint).Include(o=>o.BotInfo).Include(o=>o.Invoice).FirstOrDefault();


                    if (order != null)
                    {
                        OrderViewMsg = new OrderViewMessage(order);
                        await SendMessage(OrderViewMsg.BuildMsg());
                        return base.OkResult;
                    }

                    else
                        return base.OkResult;
                }
            }

            catch
            {
                return base.NotFoundResult;
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
                return NotFoundResult;
            }
        }

        /// <summary>
        /// Пользователь выбрал методо оплаты
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SelectPaymentMethod()
        {
            int id= 0;
            if (Argumetns.Count > 0)
                id = Argumetns[0];

            base.ConfigurationBot = GetConfigurationBot(BotInfo.Id);

            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var method = db.PaymentType.Where(p => p.Enable == true).FirstOrDefault();
                //если включена верификация номера телефона, то проверяем есть ли БД номер телефона текущего пользователя
                if (method != null && ConfigurationBot!=null && ConfigurationBot.VerifyTelephone)
                    return await TelephoneCheck(id);

                else // Если верификации номера телефона нет, то проверяем указан ли у пользователя UserName
                    return await UserNameCheck(base.ConfigurationBot, id);

            }

        }

        /// <summary>
        /// После того, как пользователь выбрал адрес доставки, Отправляем сообщение с выбором варианта оплаты
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendPaymentMethodsList()
        {
            int AddressId = 0;

            //if (Argumetns.Count > 0)
            //{ // сохраняем адрес
            //    AddressId = Argumetns[0];

            //    AddAddressToOrderTemp(AddressId);
            //}

            var message = PaymentsMethodsListMsg.BuildMsg();

            if (message != null && await EditMessage(message)!=null)            
                return OkResult;
            

            else // если сообщение с вариантами оплаты пустое, значит все варианты оплаты выключены. Отправляем пользователю
            //сообщение с оишбкой
            {
                await SendMessage(new BotMessage { TextMessage = "Нет досутпных способов оплаты. Свяжитесь с технической поддержкой /help" });

                                    //using (MarketBotDbContext db = new MarketBotDbContext())
                //{
                //    var method = db.PaymentType.Where(p => p.Enable == true).FirstOrDefault();
                //    base.ConfigurationBot = GetConfigurationBot(BotInfo.Id);

                //    //если включена верификация номера телефона, то проверяем есть ли БД номер телефона текущего пользователя
                //    if (method != null && ConfigurationBot.VerifyTelephone)
                //        await TelephoneCheck(method.Id);

                //    if(method != null && ConfigurationBot.VerifyTelephone==false) // Если верификации номера телефона нет, то проверяем указан ли у пользователя UserName
                //        await UserNameCheck(base.ConfigurationBot, method.Id);

                //    //Если Все методы оплаты выключены, то будет выбран метод оплаты "При получении"
                //    if (method == null && ConfigurationBot.VerifyTelephone)
                //        await TelephoneCheck(PaymentType.GetTypeId(Services.PaymentTypeEnum.PaymentOnReceipt));

                //    //Если Все методы оплаты выключены, то будет выбран метод оплаты "При получении"
                //    if (method == null && ConfigurationBot.VerifyTelephone==false)
                //        await UserNameCheck(base.ConfigurationBot, PaymentType.GetTypeId(Services.PaymentTypeEnum.PaymentOnReceipt));

                //}

                return OkResult;
            }

           
        }


        /// <summary>
        /// ПОльзователь выбрал вариант оплты из списка. Далее ему отправялется  сообщение с просьбой указать свой номер телефона,
        /// если номер уже есть в базе, то отсылается сообщение с превью заказа.
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> TelephoneCheck(int PaymentType=0)
        {
            int PaymentTypeId = PaymentType;

            if (Argumetns.Count > 0 && PaymentType==0)
                PaymentTypeId = Argumetns[0];

            if (PaymentTypeId > 0)
                AddPaymentMethodToOrderTemp(PaymentTypeId);

            //Сообщение с просьбой отрпвить свой номер телефон. Если сообщение пустое, значит номер телефона уже есть в БД
            var message = RequestPhoneNumberMsg.BuildMsg(); // 


            if (message != null) // Номера телефона нет в базе
            {
                if (await SendMessage(message) != null)
                    return base.OkResult;

                else
                    return base.NotFoundResult;
            }

            else // Номер телефона есть в базе. ПОказываем превью заказа
                return await SendOrderTemp();

        }

        /// <summary>
        /// Проверка юзер нейм пользователя в телеграме. 
        /// Эта функция работает если выключена верификация телефона. Это нужно для того что бы с пользователем можно было выйти на связь
        /// </summary>
        /// <param name="configuration">Конфигурация бота. Может быть пустым в случае когда пользователь нажал далее</param>
        /// <param name="PaymentType">Тип оплаты</param>
        /// <returns></returns>
        private async Task<IActionResult> UserNameCheck(Configuration configuration=null, int PaymentType = 0)
        {

            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                string name;

                int PaymentTypeId = PaymentType;
                
                if (Argumetns.Count > 0 && PaymentType == 0)
                    PaymentTypeId = Argumetns[0];

                if (PaymentTypeId > 0)
                    AddPaymentMethodToOrderTemp(PaymentTypeId);

                if (configuration==null)
                    configuration = db.Configuration.Where(c => c.BotInfoId == BotInfo.Id).FirstOrDefault();

                // Проверяем указан ли у пользователя UserName
                if (Update.CallbackQuery != null && Update.CallbackQuery.Message != null
                    && Update.CallbackQuery.Message.Chat != null && Update.CallbackQuery.Message.Chat.Username != null)
                {
                    name = Update.CallbackQuery.Message.Chat.Username;
                    var follower = db.Follower.Where(f => f.Id == FollowerId).FirstOrDefault();
                    follower.UserName = name;

                    if(name!=follower.UserName)
                        db.SaveChanges();

                    await SendOrderTemp();
                }

                else // Если не указан Просим указать
                {
                    UserNameImageMessage userNameImage = new UserNameImageMessage(configuration);
                    var message = userNameImage.BuildMsg();
                    var PhotoSend= await SendPhoto(message);
                    // добавляем ID файла в бд, что бы потом не отправлять сам файл,а только ID на сервере телегарм
                    if (configuration != null && configuration.UserNameFaqFileId == null && PhotoSend != null)
                    {
                        configuration.UserNameFaqFileId = PhotoSend.Photo[PhotoSend.Photo.Length - 1].FileId;
                        db.SaveChanges();
                    }
                }

                
            }

            return OkResult;
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
        /// Forcre Reply сообщение с просьбой отправить сообщение с комментарием для заказа
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendForceReplyAddDesc()
        {
            ForceReply forceReply = new ForceReply
            {
                Force = true,

                Selective = true
            };

            if (await SendMessage(new BotMessage { TextMessage = CmdEnterDesc, MessageReplyMarkup = forceReply }) != null)
                return base.OkResult;

            else
                return base.NotFoundResult;
        }

        /// <summary>
        /// Добавить комментарий к заказу. БД
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> AddOrderTempDesc()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var OrderTmp = db.OrderTemp.Where(o => o.FollowerId == FollowerId).FirstOrDefault();
                if (OrderTmp != null)
                {
                    OrderTmp.Text = Update.Message.Text;

                    if (db.SaveChanges() > 0 && await SendMessage(OrderPreviewMsg.BuildMessage()) != null)
                        return base.OkResult;

                    else
                        return base.NotFoundResult;
                }

                else
                    return base.OkResult;
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

                if(order.Follower != null && order.Follower.UserName != null && order.Follower.UserName != "")
                {
                   string url= BotMessage.HrefUrl("https://t.me/" + order.Follower.UserName, order.Follower.UserName);
                   await SendMessage(new BotMessage { TextMessage = url });
                   return OkResult;
                }

                else
                    return base.OkResult;
            }
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
                return base.NotFoundResult;
        }

        /// <summary>
        /// Сохрнанить
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> OrderSave()
        {
            Orders new_order = null;
            bool blocked = false;

            Bot.Order.InsertNewOrder insertNewOrder = new Bot.Order.InsertNewOrder(FollowerId, BotInfo);

            ConfigurationBot = base.GetConfigurationBot(BotInfo.Id);

            using (MarketBotDbContext db = new MarketBotDbContext())
                blocked = db.Follower.Find(FollowerId).Blocked;
            

            if (blocked)
                await AnswerCallback("Вы заблокированы администратором системы!", true);

            // если в настройках бота указано время работы магазина, то проверяем подходит ли текщее время 
            //под это правило. Если подходит то офрмляем заказ
            if (!blocked && ConfigurationBot.StartTime!=null &&
                ConfigurationBot.EndTime!=null && ConfigurationBot.StartTime.Value.Hours <=
                DateTime.Now.Hour && ConfigurationBot.StartTime.Value<= DateTime.Now.TimeOfDay &&
                 ConfigurationBot.EndTime.Value>DateTime.Now.TimeOfDay)
                      new_order= insertNewOrder.AddOrder();
                

            //Время работы магазина не указано.
            else if (!blocked && ConfigurationBot.EndTime==null && ConfigurationBot.StartTime==null)
                new_order = insertNewOrder.AddOrder();

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
            if (new_order!=null)
                await OrderRedirectToAdmins(new_order.Id);
            


            return OkResult;
        }

        /// <summary>
        /// После того как пользователь нажал на кнопку "Отправить", информация о заказе пересылается Операторам в личку и в общий чат (если он есть)
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        private async Task<IActionResult> OrderRedirectToAdmins(int OrderId, Orders order=null)
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                try
                {
                    var admins = db.Admin.Include(a => a.Follower).ToList();

                    if(order==null)
                        OrderAdminMsg = new AdminOrderMessage(OrderId);

                    else
                        OrderAdminMsg = new AdminOrderMessage(order);

                    var message = OrderAdminMsg.BuildMsg();

                    await SendMessageAllBotEmployeess(message);

                    return base.OkResult;
                }

                catch
                {
                    return base.NotFoundResult;
                }
            }

        }


        /// <summary>
        /// Добавить отзыв к заказу
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> AddNewFeedBack()
        {
            int orderid = 0;
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                try
                {
                    int number = Convert.ToInt32(base.OriginalMessage.Substring(ForceReplyAddFeedBack.Length));

                    var order = db.Orders.Where(o => o.Number == number).FirstOrDefault();

                    if (order != null)
                        orderid = order.Id;

                    var feed = db.FeedBack.Where(f => f.OrderId == orderid);

                    string text = base.ReplyToMessageText;

                    if (feed != null && feed.Count() == 0) // если отзывов еще нет
                    {
                        FeedBack feedBack = new FeedBack
                        {
                            DateAdd = DateTime.Now,
                            OrderId = orderid,
                            Text = text
                        };

                        db.FeedBack.Add(feedBack);

                        if (db.SaveChanges() > 0)
                        {
                            RaitingMsg = new RaitingMessage(feedBack.Id);
                            //отправляем сообщение с кнопками от 1 до 5
                            await EditMessage(RaitingMsg.BuildMsg());


                        }

                        return base.OkResult;
                    }

                    else // если отзыв уже есть
                        return base.OkResult;
                }

                catch
                {
                    return base.NotFoundResult;
                }
            }
        }

        /// <summary>
        /// Добавить адрес доставки к заказу (в таблицу OrderTemp !!!)
        /// </summary>
        private void AddAddressToOrderTemp(int AddressId)
        {
            OrderTemp OrderTmp = new OrderTemp();

            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                OrderTmp = db.OrderTemp.Where(o => o.FollowerId == FollowerId && o.BotInfoId==BotInfo.Id).FirstOrDefault();
                if (OrderTmp != null && AddressId > 0)
                {
                    OrderTmp.AddressId = AddressId;
                    db.SaveChanges();
                }

                if (OrderTmp == null && AddressId > 0)
                {
                    db.OrderTemp.Add(new OrderTemp { AddressId = Argumetns[0], FollowerId = base.FollowerId , BotInfoId=BotInfo.Id});
                    db.SaveChanges();
                }
            }

        }

        /// <summary>
        /// Добавить метод оплаты к заказу (в таблицу OrderTemp !!!)
        /// </summary>
        /// <param name="PaymentTypeId"></param>
        /// <returns></returns>
        private int AddPaymentMethodToOrderTemp (int PaymentTypeId)
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var order = db.OrderTemp.Where(o => o.FollowerId == FollowerId && o.BotInfoId==BotInfo.Id).FirstOrDefault();

                if (order != null && PaymentTypeId > 0)
                {
                    order.PaymentTypeId = PaymentTypeId;
                    return db.SaveChanges();
                }

                else
                    return 0;

            }
        }



        private async Task<IActionResult> CheckPay()
        {
            

           var mess= await CheckPayMsg.BuildMessage();

            await AnswerCallback(mess.TextMessage, true);

            if (mess.Order.Paid == true)
            {
                InvoiceViewMsg = new InvoiceViewMessage(mess.Order.Invoice, mess.Order.Id, "BackToMyOrder");
                await EditMessage(InvoiceViewMsg.BuildMsg());
                await OrderRedirectToAdmins(mess.Order.Id);
            }

            return OkResult;
        }


    }
}
