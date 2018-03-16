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
    public class AdminBot : BotCore
    {
        public const string ModuleName = "Admin";

        public const string ProductCreateCmd = "ProductCreate";

        public const string ProductEditCmd = "ProductEdit";

        public const string CategoryEditorCmd = "CategoryEditor";

        public const string CategoryCreateCmd = "CategoryCreate";

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

        public const string ViewStockHistoryProdCmd = "ViewStockProd";

        public const string ViewStockCmd = "ViewStock";

        public const string StockHistoryProudctCmd="/stockhistory";

        public const string BlockUserCmd = "/userblock";

        public const string UnblockUserCmd = "/userunblock";

        public const string AdminPage2Cmd = "AdminPage2";

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
                            return await SendCurrentStock(MessageId);

                        case "/on":
                            return await OnOffPrivateMessage(true);

                        case "/off":
                            return await OnOffPrivateMessage(false);


                        case ViewOrdersListCmd:
                            return await SendOrderList();

                    case ViewStockHistoryProdCmd:
                        return await SendProductStockHistory(Argumetns[0],Argumetns[1], base.MessageId);

                    case BlockFollowerCmd:
                        return await BlockUser();

                    case UnBlockFollowerCmd:
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

                    case AdminPage2Cmd:
                        return await SendPage2Btn();

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

        private async Task<IActionResult> SendPage2Btn()
        {
            ControlPanelMessage ControlPanelMessage = new ControlPanelMessage();

            await EditInlineReplyKeyboard(ControlPanelMessage.Page2Btn());

            return OkResult;

        }

        /// <summary>
        /// заблокировать пользователя
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> BlockUser()
        {
            var follower = BusinessLayer.FollowerFunction.Block(Argumetns[0]);

            if (follower!=null)
            {
                BotMessage = new FollowerControlMessage(follower);
                await EditMessage(BotMessage.BuildMsg());

                //уведомим струдников 
                string message = "Пользователь " + follower.FirstName + " " + follower.LastName + " Заблокирован." + BotMessage.NewLine() +
                                "Оператор:" + GeneralFunction.FollowerFullName(FollowerId);
                await SendMessageAllBotEmployeess(new BotMessage { TextMessage = message });
            }

            return OkResult;
        }

        /// <summary>
        /// Разблокировать пользо
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UnBlockUser()
        {
            var follower = BusinessLayer.FollowerFunction.UnBlock(Argumetns[0]);

            if (follower != null)
            {
                BotMessage = new FollowerControlMessage(follower);
                await EditMessage(BotMessage.BuildMsg());

                //уведомим струдников 
                string message = "Пользователь " + follower.FirstName + " " + follower.LastName + " Разблокирован." + BotMessage.NewLine() +
                                "Оператор:" + GeneralFunction.FollowerFullName(FollowerId);
                await SendMessageAllBotEmployeess(new BotMessage { TextMessage = message });
            }

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
            string name = ReplyToMessageText;

            var pickuppoint= PickUpPointFunction.InsertPickUpPoint(name);

            await SendPickupPointList();

            return OkResult;


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
            //ключ регистрации.
            string key = CommandName.Substring(OwnerReg.Length);

            //ключ регистрации это 15 символов из второй части Токена телеграм
            if (base.BotInfo.OwnerChatId == null && base.BotInfo.Token.Split(':').ElementAt(1).Substring(0, 15)== key)
                if (AdminFunction.UpdateOwner(BotInfo.Id,Convert.ToInt32(ChatId)) > 0)
                        await SendMessage(new BotMessage { TextMessage = "Добро пожаловать! Нажмите сюда /admin" });

            return OkResult;
            
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

                AdminFunction.RemoveOperator(id);
                 
                return await SendOperatorList();
                
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

            if (hash != null && AdminFunction.InsertAdminKey(hash) != null)
            {
 
                await SendMessage(
                        new BotMessage
                                {
                                    TextMessage = "Пользователь который должен получить права оператора должен ввести следующую команду:" 
                                                    + BotMessage.NewLine()+ BotMessage.Italic("/key " + hash)
                                }
                        );
                
                
            }

           return OkResult;
           
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
        private async Task<IActionResult> SendCurrentStock(int MessageId=0)
        {


            if (Argumetns.Count == 1)
            {
                int SelectPageNumber = Argumetns[0];
                BotMessage = new CurrentStockMessage(SelectPageNumber);
                await SendMessage(BotMessage.BuildMsg(), MessageId);
                return OkResult;
            }
            else
            {
                BotMessage = new CurrentStockMessage();
                await SendMessage(BotMessage.BuildMsg(), MessageId);
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
        /// Сообщение с панелью администратора
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendAdminControlPanelMsg()
        {
            BotMessage = new ControlPanelMessage(base.FollowerId);

            await SendMessage(BotMessage.BuildMsg());  
   
            return base.OkResult;
            
        }

        /// <summary>
        /// Пользователй хочет получить права оператора. Проверка ключа
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private async Task<IActionResult> CheckOperatorKey(string key)
        {
            var Key = AdminFunction.FindAdminKey(key);

            if (Key != null && Key.Admin.Count == 0)
                return await AddNewOpearator(Key);

            return base.OkResult;
        }

        /// <summary>
        /// Добавить нового оператора
        /// </summary>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        private async Task<IActionResult> AddNewOpearator(AdminKey adminKey)
        {
            
                var admin = AdminFunction.FindAdmin(FollowerId);

                if (admin != null)
                    return await SendAdminControlPanelMsg();

                else {

                    admin= AdminFunction.InsertAdmin(FollowerId, adminKey);

                if (admin != null)
                {
                    string meessage = "Зарегистрирован новый оператор системы: " + GeneralFunction.FollowerFullName(admin.FollowerId)
                    + BotMessage.NewLine() + "Ключ: " + adminKey.KeyValue;
                    await SendMessage(BotOwner, new BotMessage { TextMessage = meessage });
                    return await SendAdminControlPanelMsg();
                }

                }

            return OkResult;

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

