using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace MyTelegramBot.Controllers
{
    [Produces("application/json")]
    public class OrderController : Controller
    {
        MarketBotDbContext db;

        Orders Order;

        TelegramBotClient TelegramBotClient { get; set; }

        Messages.FeedBackOfferMessage feedBackOffer { get; set; }

        public IActionResult Index()
        {
            db = new MarketBotDbContext();

           return View(db.Orders.Include(o => o.BotInfo).OrderByDescending(o=>o.Id).ToList());


        }

        [HttpGet]
        public IActionResult GetHistoryList(int Id)
        {
            if (db == null)
                db = new MarketBotDbContext();



            var HistoryList = db.OrderHistory.Where(h => h.OrderId == Id).Include(h => h.Follower).Include(h=>h.Action).OrderByDescending(h => h.Id).ToList();
                

            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

            foreach (OrderHistory history in HistoryList)
            {
                Dictionary<string, string> value = new Dictionary<string, string>();
                value.Add("ActionName", history.Action.Name);

                if(history.Text!=null)
                    value.Add("Text", history.Text);

                else
                    value.Add("Text", String.Empty);

                value.Add("UserName", history.Follower.FirstName + history.Follower.LastName);
                value.Add("Timestamp", history.Timestamp.ToString());
                list.Add(value);
            }

            return Json(list);
        }


        [HttpGet]
        public IActionResult GetInWorkList(int Id)
        {
            if (db == null)
                db = new MarketBotDbContext();

            if (Id > 0)
                Order = db.Orders.Where(o => o.Id == Id).Include(o => o.OrdersInWork).FirstOrDefault();

            if (Order.OrdersInWork != null)
                foreach (OrdersInWork work in Order.OrdersInWork)
                    work.Follower = db.Follower.Where(f => f.Id == work.FollowerId).FirstOrDefault();

            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

            foreach (OrdersInWork work in Order.OrdersInWork)
            {
                Dictionary<string, string> name = new Dictionary<string, string>();
                name.Add("name", work.Follower.FirstName + work.Follower.LastName);
                name.Add("Timestamp", work.Timestamp.ToString());

                if (work.InWork == true)
                    name.Add("InWork", "Взял");
                if (work.InWork == false)
                    name.Add("InWork", "Освободил");

                list.Add(name);
            }

            return Json(list);
        }

        [HttpGet]
        public IActionResult Get(int Number)
        {
            if (db == null)
                db = new MarketBotDbContext();

            if(Number>0)
            Order = db.Orders.Where(o => o.Number == Number).Include(o=>o.Invoice).Include(o => o.Confirm).
                Include(o => o.Delete).Include(o => o.Done).Include(o => o.OrderProduct)
                .Include(o=>o.FeedBack).Include(o => o.OrderAddress)
                .Include(o => o.FeedBack).Include(o=>o.OrdersInWork).Include(o=>o.PickupPoint)
                .Include(o => o.Follower).FirstOrDefault();

            if (Order != null)
            {
                if(Order.OrderAddress!=null)
                    Order.OrderAddress.Adress = db.Address.Where(a => a.Id == Order.OrderAddress.AdressId).Include(a => a.House).Include(a => a.House.Street).Include(a => a.House.Street.City).FirstOrDefault();

                if (Order.Invoice != null)
                    Order.Invoice.PaymentType = db.PaymentType.Where(payment => payment.Id == Order.Invoice.PaymentTypeId).FirstOrDefault();

                if (Order.OrdersInWork != null)
                    foreach (OrdersInWork work in Order.OrdersInWork)
                        work.Follower = db.Follower.Where(f => f.Id == work.FollowerId).FirstOrDefault();

                foreach (OrderProduct Op in Order.OrderProduct)
                {
                    Op.Product = db.Product.Where(p => p.Id == Op.ProductId).FirstOrDefault();
                    Op.Price = db.ProductPrice.Where(price => price.Id == Op.PriceId).Include(price => price.Currency).FirstOrDefault();

                }

                var HistoryList = db.OrderHistory.Where(h => h.OrderId == Order.Id).Include(h => h.Follower).Include(h=>h.Action).ToList();

                Tuple<Orders,List<OrderHistory>> model = new Tuple<Orders,List<OrderHistory>>(Order, HistoryList);

                if (Order.OrderAddress != null)
                    ViewBag.TotalPrice = model.Item1.TotalPrice() + Order.OrderAddress.ShipPriceValue;

                else
                    ViewBag.TotalPrice = model.Item1.TotalPrice();



                return View(model);
            }

            else
                return NotFound();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<IActionResult> AddHistory ([FromBody] OrderHistory history)
        {
            db = new MarketBotDbContext();

            var inwork = CheckInWork(history.OrderId);

            string UserName = String.Empty;

            if (history != null)
            {
                history.FollowerId = db.Follower.Where(f => f.ChatId == db.BotInfo.FirstOrDefault().OwnerChatId).FirstOrDefault().Id;
                Order = db.Orders.Where(o => o.Id == history.OrderId).Include(o=>o.Done).Include(o => o.Delete).Include(o => o.Confirm).Include(o=>o.Follower).FirstOrDefault();
                feedBackOffer = new Messages.FeedBackOfferMessage(Order);
                UserName = Bot.GeneralFunction.FollowerFullName(history.FollowerId);
            }

            if (inwork.FollowerId != history.FollowerId)
                return Json("Ошибка! Заказ в обработке у " + inwork.Follower.FirstName + " " + inwork.Follower.LastName);

            //Заказ согласован
            if (Order!=null && history.ActionId==1 && inwork.FollowerId==history.FollowerId)
            {
                Order.ConfirmId = InsertHistory(history);
                db.SaveChanges();
                TelegramAdminSendMessage("Заказ №" + Order.Number.ToString() + " согласован /order" + Order.Number.ToString()+ " | Пользователь:" + UserName);

                return Json("Согласовано");
            }

            //Заказ выполнен
            if (Order!=null && history.ActionId == 2 && Order.ConfirmId>0 && inwork.FollowerId == history.FollowerId && Order.Delete==null) // проверяем согласован ли заявка и не удален ли он
            {
                Order.DoneId = InsertHistory(history);
                db.SaveChanges();
                await TelegramAdminSendMessage("Заказ № " + Order.Number.ToString() + " выполнен /order" + Order.Number.ToString()+ " | Пользователь:" + UserName);
                var mess = feedBackOffer.BuildMsg();
                //отрпаляем клиенту сообщение с предожением оставить отзыв
                TelegramSendMessage(Order.Follower.ChatId, mess.TextMessage, mess.MessageReplyMarkup);
                return Json("Выполнено");
            }

            //Заказ еще согласован, поэтому ошибка
            if (Order != null && history.ActionId == 2 && Order.ConfirmId == null)
                return Json("Заказ еще не соласован");

            //Удаление заказа
            if (Order != null && history.ActionId == 3 && inwork.FollowerId == history.FollowerId && Order.Delete==null) 
            {
                Order.DeleteId = InsertHistory(history);
                db.SaveChanges();
                TelegramAdminSendMessage("Заказ № " + Order.Number.ToString() + " удален /order" + Order.Number.ToString()+ " | Пользователь:" + UserName);
                return Json("Удалено");
            }

            ///Заказ уже удален. Ошибка
            if (Order != null && history.ActionId == 3 && inwork.FollowerId == history.FollowerId && Order.Delete != null)
                return Json("Заказ уже удален");
            
            //Восстановление заказа
            if(Order!=null && history.ActionId == 4 && inwork.FollowerId == history.FollowerId && Order.Delete !=null)
            {
                InsertHistory(history);
                Order.Delete = null;
                db.SaveChanges();
                TelegramAdminSendMessage("Заказ № " + Order.Number.ToString() + " восстановлен /order" + Order.Number.ToString()+ " | Пользователь:" + UserName);
                return Json("Восстановлено");
            }

            //Заказ еще удален, ошибка
            if (Order != null && history.ActionId == 4 && inwork.FollowerId == history.FollowerId && Order.Delete ==null)
                return Json("Заказ не удален");

            else
                return Json("Ошибка");
        }


        /// <summary>
        /// Взять заказ в обработку
        /// </summary>
        /// <param name="OrderId"></param>
        /// <param name="Take"></param>
        /// <param name="FollowerId"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<IActionResult> TakeInWork([FromBody] OrdersInWork inWork)
        {

            if (db == null)
                db = new MarketBotDbContext();

            inWork.FollowerId = db.Follower.Where(f => f.ChatId == db.BotInfo.FirstOrDefault().OwnerChatId).FirstOrDefault().Id;

            string UserName =Bot.GeneralFunction.FollowerFullName(inWork.FollowerId);

            var CurrentInWork = CheckInWork(Convert.ToInt32(inWork.OrderId));

            this.Order = db.Orders.Find(inWork.OrderId);

            //заявка ни кем не обрабатывается или уже  обрабатывается текущим пользовтелем
            if (CurrentInWork == null || CurrentInWork != null && CurrentInWork.FollowerId == inWork.FollowerId)
            {
                if (CurrentInWork == null ||
                    CurrentInWork != null && CurrentInWork.InWork == true && inWork.InWork == false
                    )
                // заявка ни кем не обрабатывается и пользователь берет ее в обработку или
                // пользователь хочет освободить заявку
                {
                    InsertInWork(inWork.OrderId, inWork.FollowerId, inWork.InWork);
                   
                }
                if (inWork.InWork == true)
                {
                    TelegramAdminSendMessage("Пользователь: " + UserName + " | Взял в работу заказ №" + Order.Number.ToString() + " | /order" + Order.Number.ToString());
                    return Json("В работе");
                }


                else
                {
                    TelegramAdminSendMessage("Пользователь: " + UserName + " | Освободил заказ №" + Order.Number.ToString() + " | /order" + Order.Number.ToString());
                    return Json("Свободна");
                    
                }
            }

            else
                return Json("Заявка в обработке у " + CurrentInWork.Follower.FirstName + " " + CurrentInWork.Follower.LastName);

            //Order= db.Orders.Where(o => o.Id == OrderId).Include(o => o.OrdersInWork).FirstOrDefault();

            //foreach (OrdersInWork work in Order.OrdersInWork)
            //    work.Follower = db.Follower.Find(work.FollowerId);


        }


        private int InsertHistory(OrderHistory history)
        {
            if (db == null)
                db = new MarketBotDbContext();

            if (history != null && history.OrderId > 0 && history.FollowerId > 0)
            {
                history.Value = true;
                history.Timestamp = DateTime.Now;
                db.OrderHistory.Add(history);
                db.SaveChanges();
                return history.Id;
            }

            else
                return -1;
        }

        private int InsertInWork(int? OrderId, int? FollowerId, bool? Take = true)
        {
            if (db == null)
                db = new MarketBotDbContext();

            if (OrderId > 0 && FollowerId > 0 && Take != null)
            {
                OrdersInWork inWork = new OrdersInWork
                {
                    FollowerId = FollowerId,
                    OrderId = OrderId,
                    InWork = Take,
                    Timestamp = DateTime.Now
                };

                db.OrdersInWork.Add(inWork);

                return db.SaveChanges();
            }

            else
                return -1;
        }


        private OrdersInWork CheckInWork(int? OrderId)
        {
            if (db == null)
                db = new MarketBotDbContext();

            var inwork = db.OrdersInWork.Where(o => o.OrderId == OrderId).OrderByDescending(o => o.Id).FirstOrDefault();

            if (inwork != null)
                inwork.Follower = db.Follower.Find(inwork.FollowerId);

            if (inwork != null && inwork.InWork == true)
                return inwork;

            else
                return null;
        }
        

        private async Task<bool> TelegramAdminSendMessage(string Text)
        {
            if (db == null)
                db = new MarketBotDbContext();

            var BotInfo = db.BotInfo.Where(b => b.Name == Bot.GeneralFunction.GetBotName()).Include(b=>b.Configuration).FirstOrDefault();

            if (BotInfo != null)
            {

                var token = BotInfo.Token;

                var GroupChat = BotInfo.Configuration.PrivateGroupChatId;

                var OperatorsList = db.Admin.Where(a => a.Enable).Include(a => a.Follower).ToList();

                TelegramBotClient = new TelegramBotClient(token);

                foreach (var admin in OperatorsList) // Отправляем все админам в лс
                {
                    if (admin.NotyfiActive)
                    {
                        await TelegramBotClient.SendTextMessageAsync(admin.Follower.ChatId, Text);
                        System.Threading.Thread.Sleep(300);
                    }
                }

                //отправляем в групповой чат
                await TelegramBotClient.SendTextMessageAsync(GroupChat, Text);

                return true;
            }

            else
                return false;
        }

        private async Task<bool> TelegramSendMessage(int ChatId, string Text, Telegram.Bot.Types.ReplyMarkups.IReplyMarkup reply)
        {
            if (db == null)
                db = new MarketBotDbContext();

            var BotInfo = db.BotInfo.Where(b => b.Name == Bot.GeneralFunction.GetBotName()).Include(b => b.Configuration).FirstOrDefault();

            if (BotInfo != null)
            {
                var token = BotInfo.Token;

                TelegramBotClient = new TelegramBotClient(token);

                await TelegramBotClient.SendTextMessageAsync(ChatId, Text,Telegram.Bot.Types.Enums.ParseMode.Default,false,false,0, reply);

                return true;
            }

            else
                return false;
        }
    }
}