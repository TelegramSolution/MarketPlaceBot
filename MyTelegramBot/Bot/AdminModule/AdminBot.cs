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

namespace MyTelegramBot.Bot.AdminModule
{
    public class AdminBot : BotCore
    {
        public const string ModuleName = "Admin";

        public const string ProductCreateCmd = "ProductCreate";

        public const string ProductEditCmd = "ProductEdit";

        public const string CategoryEditorCmd = "CategoryEditor";

        public const string CategoryCreateCmd = "CategoryCreate";

        public const string NotyfiCreateCmd = "NotyfiCreate";

        public const string AdminProductInCategoryCmd = "AdminProductInCategory";

        public const string EnterNameNewProductCmd = "Введите данные для нового товара";

        public const string EnterNameNewCategoryCmd = "Введите название новой категории";

        public const string BackToAdminPanelCmd = "BackToAdminPanel";

        private const string AdminKeyCmd = "/adminkey";

        /// <summary>
        /// заблокировать
        /// </summary>
        public const string BlockFollowerCmd = "BlockFollower";

        /// <summary>
        /// Разблокировать
        /// </summary>
        public const string UnBlockFollowerCmd = "UnBlockFollower";

        public const string PaymentTypeEnableCmd = "PaymentTypeEnable";

        public const string PayMethodsListCmd = "/paymethods";

        public const string QiwiEditCmd = "QiwiEdit";

        public const string QiwiAddEdit = "QiwiAdd";

        private const string EnterPhoneNumber = "Введите номер телефона";

        private const string EnterQiwiApi = "Введите Qiwi ключ";

        public const string StatCmd = "/stat";

        public const string WhatIsQiwiApiCmd = "/whatisqiwiapi";

        private const string AddGroup = "/addchat";

        private const string RevomeGroup = "/delchat";

        private const string RemoveAvailableCityCmd = "/cityremove";

        private const string RemoveOperatorCmd = "/removeoperator";

        private const string OwnerReg = "/owner";

        public const string ViewFollowerListCmd = "ViewFollowerList";

        public const string ViewOrdersListCmd = "ViewOrderList";

        public const string ViewPaymentsListCmd = "ViewPaymentsList";

        public const string ViewCitiesCmd = "ViewCities";

        public const string ViewOperatosCmd = "ViewOperatos";

        public const string ViewPickupPointCmd = "ViewPickupPoint";

        public const string AddPickupPoint = "/addpickuppoint";

        public const string AddPickupPointForceReply = "Добавить пункт самовывоза";

        public const string EnablePickUpPointCmd = "/pickupenable";

        public const string DisablePickUpPointCmd = "/pickupdisable";

        public const string ViewStockProdCmd = "ViewStockProd";

        public const string StockHistoryProudctCmd="/stockhistory";

        public const string BlockUserCmd = "/userblock";

        public const string UnblockUserCmd = "/userunblock";

        private int Parametr { get; set; }
        public AdminBot(Update _update) : base(_update)
        {
          
        }

        protected override void Initializer()
        {
            try
            {
                
                if (base.Argumetns.Count > 0)
                {
                    Parametr = base.Argumetns[0];
                }


            }

            catch
            {

            }
        }

        public async override Task<IActionResult> Response()
        {
            if(IsOperator() || IsOwner())
            {
                    switch (base.CommandName)
                    {
                        //Панель администратора /admin
                        case "/admin":
                            return await SendAdminControlPanelMsg();

                        //Вернуть в Панель администратора
                        case BackToAdminPanelCmd:
                            return await BackToAdminPanel();


                        case "/allprod":
                            return await SendAllProductsView();

                        case "ViewStock":
                            return await SendCurrentStock(0,MessageId);

                        case "/on":
                            return await OnOffPrivateMessage(true);

                        case "/off":
                            return await OnOffPrivateMessage(false);


                        case ViewOrdersListCmd:
                            return await SendOrderList();

                    case ViewStockProdCmd:
                        return await SendProductStockHistory(Argumetns[0],Argumetns[1], base.MessageId);

                    case BlockFollowerCmd:
                        return await BlockUser();

                    case UnblockUserCmd:
                        return await UnBlockUser();

                    default:
                            break;
                    }

                if (base.CommandName.Contains(StockHistoryProudctCmd))
                    await SendProductStockHistory(Convert.ToInt32(base.CommandName.Substring(StockHistoryProudctCmd.Length)));


            }

            if (IsOwner())
            {
                switch (base.CommandName)
                {
                    case ViewFollowerListCmd:
                        return await SendFollowerList();

                    case ViewPickupPointCmd:
                        return await SendPickupPointList();


                    case AddPickupPoint: // пользователь нажал на кнопку добавить пункт самовывоза
                        return await SendForceReplyMessage(AddPickupPointForceReply);


                    case PayMethodsListCmd:
                        return await SendPaymentMethods();


                    case ViewOperatosCmd:
                        return await SendOperatorList(MessageId);

                    case "GenerateKey":
                        return await GenerateKey();

                    case AddGroup:
                       return await AddBotToChat();

                    case ViewCitiesCmd:
                        return await SendAvailableCities(base.MessageId);

                    case "/newcity":
                        return await SendForceReplyMessage("Введите название города");

                    case "GetCategoryStock":
                        return await SendCurrentStock(Argumetns[0],MessageId);

                   

                    default:
                        break;
                }

                if (base.OriginalMessage.Contains("Введите название города"))
                    return await AddAvailableCity();

                if (base.CommandName.Contains(RemoveAvailableCityCmd))
                    return await RemoveAvailableCity();


                if (base.CommandName.Contains(RemoveOperatorCmd))
                    return await RemoveOperator();

                if (base.OriginalMessage.Contains(AddPickupPointForceReply))
                    return await InsertPicupPoint();

                if (base.CommandName.Contains(EnablePickUpPointCmd))
                    return await EnablePickUpPoint(EnablePickUpPointCmd);

                if (base.CommandName.Contains(DisablePickUpPointCmd))
                    return await EnablePickUpPoint(DisablePickUpPointCmd);


                else
                    return null;
            }

            else
            {
                if (base.CommandName.Contains("/key"))
                    return await CheckOperatorKey(CommandName.Substring(5));


                if (base.CommandName.Contains(OwnerReg))
                    return await OwnerRegister();

                else
                    return null;
            }
        }


        /// <summary>
        /// заблокировать пользователя
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> BlockUser()
        {
            var follower = BusinessLayer.FollowerFunction.Block(Argumetns[0]);

            BotMessage = new FollowerControlMessage(follower);
            await EditMessage(BotMessage.BuildMsg());

            return OkResult;
        }

        /// <summary>
        /// Разблокировать пользо
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UnBlockUser()
        {
            var follower = BusinessLayer.FollowerFunction.UnBlock(Argumetns[0]);

            BotMessage = new FollowerControlMessage(follower);
            await EditMessage(BotMessage.BuildMsg());

            return OkResult;
        }

        private async Task<IActionResult> SendProductStockHistory(int ProductId,int PageNumber=1,int MessageId=0)
        {
            if (ProductId>0)
                BotMessage = new ProductStockHistoryMessage(ProductId, PageNumber);

            var mess = BotMessage.BuildMsg();

            if (mess != null)
                await SendMessage(mess, MessageId);

            return OkResult;
        }

        private async Task<IActionResult> EnablePickUpPoint(string Command)
        {
            int id =Convert.ToInt32(base.CommandName.Substring(Command.Length));

            using(MarketBotDbContext db=new MarketBotDbContext())
            {
                var pickup = db.PickupPoint.Find(id);

                if (pickup != null && pickup.Enable == false)
                {
                    pickup.Enable = true;
                    db.SaveChanges();
                    return await SendPickupPointList();
                }

                if (pickup != null && pickup.Enable)
                {
                    pickup.Enable = false;
                    db.SaveChanges();
                    return await SendPickupPointList();
                }


                else
                    await SendPickupPointList();

                return OkResult;
            }
        }

        private async Task<IActionResult> InsertPicupPoint()
        {
            using (MarketBotDbContext db=new MarketBotDbContext())
            {
                if (db.PickupPoint.Where(p => p.Name == ReplyToMessageText).FirstOrDefault() == null)
                {
                    PickupPoint pickupPoint = new PickupPoint
                    {
                        Enable = true,
                        Name = ReplyToMessageText,

                    };

                    db.PickupPoint.Add(pickupPoint);
                    db.SaveChanges();
                    await SendPickupPointList();
                }

                else
                {
                    await SendMessage(new BotMessage { TextMessage = "Уже существует" });
                    await SendPickupPointList();
                }

                return OkResult;
            }
        }

        private async Task<IActionResult> SendPickupPointList(int MessageId=0)
        {
            if (Argumetns!=null && Argumetns.Count > 0)
                BotMessage = new PickUpPointListMessage(Argumetns[0]);

            else
                BotMessage = new PickUpPointListMessage();

            await SendMessage(BotMessage.BuildMsg(), MessageId);

            return OkResult;

        }

        /// <summary>
        /// Отправить список всех заказов
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendOrderList()
        {
            if (Argumetns != null && Argumetns.Count > 0)
                BotMessage = new OrdersListMessage(Argumetns[0]);

            else
                BotMessage = new OrdersListMessage();

            var mess = BotMessage.BuildMsg();

            if (mess != null)
                await EditMessage(mess);

            else
                await AnswerCallback("Данные отсутствуют", true);

            return OkResult;
        }

        /// <summary>
        /// Отправить список всех пользователей
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendFollowerList()
        {
            if (Argumetns != null && Argumetns.Count>0)
                BotMessage = new FollowerListMessage(Argumetns[0]);

            else
                BotMessage = new FollowerListMessage();

            var mess = BotMessage.BuildMsg();

            if (mess != null)
                await EditMessage(mess);

            else
                await AnswerCallback("Данные отсутствуют", true);

            return OkResult;
        }

        /// <summary>
        /// Подтверждение владельца
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> OwnerRegister()
        {
            string key = CommandName.Substring(OwnerReg.Length);

            using(MarketBotDbContext db=new MarketBotDbContext())
            {
                base.BotInfo = db.BotInfo.Where(b => b.Id == BotInfo.Id).FirstOrDefault();

                if (base.BotInfo.OwnerChatId == null && base.BotInfo.Token.Split(':').ElementAt(1).Substring(0, 15)== key)
                {
                    db.BotInfo.Where(b => b.Id == BotInfo.Id).FirstOrDefault().OwnerChatId =Convert.ToInt32(ChatId);
                    if (db.SaveChanges() > 0)
                         await SendMessage(new BotMessage { TextMessage = "Добро пожаловать! Нажмите сюда /admin" });
                    return OkResult;
                }

                else
                    return OkResult;
            }
        }

        /// <summary>
        /// удалить оператора
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> RemoveOperator()
        {
            try
            {
                int id = Convert.ToInt32(base.CommandName.Substring(RemoveOperatorCmd.Length));
                using(MarketBotDbContext db=new MarketBotDbContext())
                {
                  var admin= db.Admin.Where(a => a.Id == id).FirstOrDefault();

                    if (admin != null)
                    {
                        db.Admin.Remove(admin);
                        db.SaveChanges();
                        
                    }

                    return await SendOperatorList();
                }
            }

            catch
            {
                return await SendOperatorList();
            }
        }

        /// <summary>
        /// Отправить сообщение со списком доступных городов
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendAvailableCities(int MessageId=0)
        {
            try
            {
               BotMessage = new AvailableCitiesMessage();
               await SendMessage(BotMessage.BuildMsg(), MessageId);
               return OkResult;
            }

            catch
            {
                return OkResult;
            }
        }

        /// <summary>
        /// Добавить новый город к списку доступных
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> AddAvailableCity()
        {
            try
            {
                using (MarketBotDbContext db=new MarketBotDbContext())
                {
                    if (db.AvailableСities.Where(c => c.CityName == ReplyToMessageText).FirstOrDefault() == null)
                    {

                        AvailableСities availableСities = new AvailableСities
                        {
                            CityName = ReplyToMessageText,
                            Timestamp = DateTime.Now
                        };

                        db.AvailableСities.Add(availableСities);
                        db.SaveChanges();

                        await SendAvailableCities();
                    }

                    else
                    {
                        await SendMessage(new BotMessage { TextMessage = "Этот город уже добавлен в список" });
                    }

                    return OkResult;
                }
            }

            catch
            {
                return OkResult;
            }
        }

        /// <summary>
        /// Удалить город из списка доступных
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> RemoveAvailableCity()
        {
            try
            {
                int id = Convert.ToInt32(CommandName.Substring(RemoveAvailableCityCmd.Length));

                using(MarketBotDbContext db=new MarketBotDbContext())
                {
                    var city = db.AvailableСities.Where(c => c.Id == id).FirstOrDefault();

                    if (city != null)
                    {
                        db.AvailableСities.Remove(city);
                        db.SaveChanges();
                    }

                    return await SendAvailableCities();
                }
            }

            catch
            {
                return OkResult;
            }
        }

        /// <summary>
        /// Делам так что бы бот мог отсылать в этот чат Админские уведомления
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> AddBotToChat()
        {
            try
            {
                using (MarketBotDbContext db=new MarketBotDbContext())
                {
                    db.Configuration.FirstOrDefault().PrivateGroupChatId =base.GroupChatId.ToString();

                    if (db.SaveChanges() > 0)
                        await SendMessage(base.GroupChatId, new BotMessage { TextMessage="Успех!" });

                    return OkResult;
                }
            }

            catch
            {
                return OkResult;
            }


        }

        /// <summary>
        /// Генерируем новый ключ для оператора. 
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> GenerateKey()
        {
            string hash= GeneralFunction.GenerateHash();

            if (hash != null)
            {
                using (MarketBotDbContext db = new MarketBotDbContext())
                {

                    AdminKey key = new AdminKey { DateAdd = DateTime.Now, Enable = false, KeyValue = hash };
                    db.AdminKey.Add(key);

                    if (db.SaveChanges() > 0)
                    {
                        await SendMessage(new BotMessage { TextMessage = "Пользователь который должен получить права оператора должен ввести следующую команду:" + BotMessage.NewLine()+ BotMessage.Italic("/key " + key.KeyValue) });
                        return OkResult;
                    }

                    else
                        return OkResult;

                }
            }

            else return OkResult;
           
        }

        /// <summary>
        /// Отправить сообщение со списком всех операторов
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendOperatorList(int MessageId=0)
        {
            try
            {
                BotMessage = new OperatosListMessage();
                await SendMessage(BotMessage.BuildMsg(), MessageId);
                return OkResult;
            }

            catch
            {
                return OkResult;
            }
        }

        /// <summary>
        /// Отправить сообщение со списком способов оплаты
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendPaymentMethods(int MessageId=0)
        {
            try
            {
                BotMessage = new AdminPayMethodsSettings();
                await SendMessage(BotMessage.BuildMsg(),MessageId);
                return OkResult;
            }

            catch
            {
                return OkResult;
            }
        }

        /// <summary>
        /// Отправить сообещние с текущими остатками по товарам
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendCurrentStock(int CategoryId=0, int MessageId=0)
        {
            try
            {
                BotMessage = new CurrentStockMessage(CategoryId);
                await SendMessage(BotMessage.BuildMsg(), MessageId);
                return OkResult;
            }
            catch
            {
                return OkResult;
            }
        }

        /// <summary>
        /// Отправить сообщние со списком всех товаров
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendAllProductsView()
        {
            try
            {
                BotMessage = new AdminAllProductsViewMessage();
                await SendMessage(BotMessage.BuildMsg());
                return OkResult;
            }

            catch
            {
                return OkResult; 
            }
        }



        /// <summary>
        /// Записываем в бд об успешном запросе
        /// </summary>
        /// <returns></returns>
        private int InsertReportsRequest()
        {
            using (MarketBotDbContext db=new MarketBotDbContext())
            {
                ReportsRequestLog log = new ReportsRequestLog
                {
                    FollowerId = FollowerId,
                    DateAdd = DateTime.Now
                };

                db.ReportsRequestLog.Add(log);

                return db.SaveChanges();
            }
        }

        /// <summary>
        /// Время последнего запроса к БД, для формирования запроса
        /// </summary>
        /// <returns></returns>
        private DateTime LastReportsRequest()
        {
            using (MarketBotDbContext db=new MarketBotDbContext())
            {
               var log= db.ReportsRequestLog.Where(r => r.FollowerId == FollowerId).OrderByDescending(r => r.Id).FirstOrDefault();

                if (log != null)
                    return Convert.ToDateTime(log.DateAdd);

                else
                   return DateTime.Now.AddDays(-1);
            }
        }




        /// <summary>
        /// Сообщение с панелью администратора
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendAdminControlPanelMsg()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                BotMessage = new ControlPanelMessage(base.FollowerId);
                if (BotMessage != null && await SendMessage(BotMessage.BuildMsg()) != null)
                    return base.OkResult;

                else
                    return base.OkResult;
            }

        }

        /// <summary>
        /// Пользователй хочет получить права оператора. Проверка ключа
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private async Task<IActionResult> CheckOperatorKey(string key)
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var Key = db.AdminKey.Where(a=>a.Enable==false && a.KeyValue==key).Include(a=>a.Admin).FirstOrDefault();

                if (Key != null && Key.Admin.Count==0)
                    return await AddNewOpearator(Key);

                else
                    return base.OkResult;
            }
        }

        /// <summary>
        /// Добавить нового оператора
        /// </summary>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        private async Task<IActionResult> AddNewOpearator(AdminKey adminKey)
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var admin = db.Admin.Where(a => a.FollowerId == FollowerId && a.Enable).Include(a=>a.AdminKey).Include(a=>a.Follower).FirstOrDefault();

                Admin adminnew = new Admin
                {
                    AdminKeyId = adminKey.Id,
                    FollowerId = FollowerId,
                    DateAdd = DateTime.Now,
                    NotyfiActive = true,
                    Enable = true,

                };


                if (admin != null)
                    return await SendAdminControlPanelMsg();

                else
                {
                    Admin NewAdmin = new Admin
                    {
                        FollowerId = FollowerId,
                        DateAdd = DateTime.Now,
                        AdminKeyId = adminKey.Id,
                        Enable = true,
                        NotyfiActive=true

                    };
                   
                    db.Admin.Add(NewAdmin);
                    adminKey.Enable = true;
                    if (db.SaveChanges() > 0)
                    {
                        string meessage = "Зарегистрирован новый оператор системы: " + db.Follower.Where(f=>f.Id==FollowerId).FirstOrDefault().FirstName
                            +BotMessage.NewLine()+"Ключ: "+ adminKey.KeyValue;
                        await SendMessage(BotOwner, new BotMessage { TextMessage = meessage });
                        return await SendAdminControlPanelMsg();
                    }
                    else
                        return OkResult;
                }
            }
        }





        /// <summary>
        /// Вернуть к панели администратора
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> BackToAdminPanel()
        {
            BotMessage = new ControlPanelMessage(FollowerId);
            if (await EditMessage(BotMessage.BuildMsg()) != null)
                return OkResult;

            else
                return OkResult;
        }


        /// <summary>
        /// вкл/выкл уведомления от бота в лс
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> OnOffPrivateMessage(bool value)
        {
            try
            {
                using (MarketBotDbContext db=new MarketBotDbContext())
                {


                    var admin= db.Admin.Where(a => a.FollowerId == FollowerId).FirstOrDefault();

                    if (IsOperator() && admin != null)
                    {
                        admin.NotyfiActive = value;

                        if(db.SaveChanges()>0)
                            await SendMessage(new BotMessage { TextMessage = "Сохранено" });
                    }

                    if(IsOwner())
                    {
                        var conf = db.Configuration.Where(c => c.BotInfoId == BotInfo.Id).FirstOrDefault();

                        conf.OwnerPrivateNotify = value;

                        if (db.SaveChanges() > 0)
                            await SendMessage(new BotMessage { TextMessage = "Сохранено" });
                    }

                    return OkResult;
                }
            }

            catch (Exception e)
            {
                return OkResult;
            }
        }


    }
}
class NewProduct
{
    public string Name { get; set; }

    public int CategoryId { get; set; }

    public string Text { get; set; }

    public double Price { get; set; }

    public int AttacmentFsId { get; set; }

    public int Currency { get; set; }

    public int Unit { get; set; }

    public int Volume { get; set; }

    public int Stock { get; set; }

}
