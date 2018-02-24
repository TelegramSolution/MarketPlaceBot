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
    public class AdminBot : BotCore
    {
        public const string ModuleName = "Admin";

        private AdminPanelCmdMessage AdminCmdListMsg { get; set; }

        private CategoryListMessage CategoryListMsg { get; set; }

        private AdminProductListMessage AdminProductListMsg { get; set; }

        private AdminProductFuncMessage AdminProductFuncMsg { get; set; }

        private ContactEditMessage ContactEditMsg { get; set; }

        private AdminAllProductsViewMessage AdminAllProductsViewMsg { get; set; }

        private AdminCurrentStockMessage AdminCurrentStockMsg { get; set; }

        private AdminPayMethodsSettings AdminPayMethodsSettingsMsg { get; set; }

        private AdminQiwiSettingsMessage AdminQiwiSettingsMsg { get; set; }

        private StatisticMessage StatisticMsg { get; set; }

        private AdminControlMessage AdminControlMsg { get; set; }

        private AvailableCitiesMessage AvailableCitiesMsg { get; set; }


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

        public const string ImportCsvCmd = "ImportCsv";

        public const string OrderExportCmd = "OrderExport";

        public const string StockExportCmd = "StockExport";

        public const string NoConfirmOrderCmd = "NoConfirmOrder";

        public const string ContactEditCmd = "ContactEdit";

        public const string VkEditCmd = "VkEdit";

        public const string InstagramEditCmd = "InstagramEdit";

        public const string ChatEditCmd = "ChatEdit";

        public const string ChannelEditCmd = "ChannelEdit";

        private const string ForceReplyVk = "Vk.com";

        private const string ForceReplyInstagram = "Instagram.com";

        private const string ForceReplyChat = "Чат";

        private const string ForceReplyChannel = "Канал";

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


        private int Parametr { get; set; }
        public AdminBot(Update _update) : base(_update)
        {
          
        }

        protected override void Constructor()
        {
            try
            {
                AdminQiwiSettingsMsg = new AdminQiwiSettingsMessage();
                AdminCmdListMsg = new AdminPanelCmdMessage(base.FollowerId);
                ContactEditMsg = new ContactEditMessage();
                AdminAllProductsViewMsg = new AdminAllProductsViewMessage();
                AdminCurrentStockMsg = new AdminCurrentStockMessage();
                AdminPayMethodsSettingsMsg = new AdminPayMethodsSettings();
                AdminControlMsg = new AdminControlMessage();
                StatisticMsg = new StatisticMessage();
                AvailableCitiesMsg = new AvailableCitiesMessage();
                
                if (base.Argumetns.Count > 0)
                {
                    Parametr = base.Argumetns[0];
                    AdminProductListMsg = new AdminProductListMessage(this.Parametr);
                    AdminProductFuncMsg = new AdminProductFuncMessage(Parametr);
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

                        case NoConfirmOrderCmd:
                            return await NoConfirmOrder();

                        case "/stockexport":
                            return await StockExport();

                        case "/allprod":
                            return await SendAllProductsView();

                        case "ViewStock":
                            return await SendCurrentStock(0,MessageId);

                        case "/on":
                            return await OnOffPrivateMessage(true);

                        case "/off":
                            return await OnOffPrivateMessage(false);

                        default:
                            break;
                    }
                
            }

            if (IsOwner())
            {
                switch (base.CommandName)
                {

                    //Пользователь нажал на кнопку "Добавить товар", ему пришло Сообщение с иструкцией по добавлению
                    case "/newprod":
                        return await SendInsertProductFAQ();

                    //Пользователь нажал на "Изменить товар", ему пришло сообещние с выбором категории
                    case ProductEditCmd:
                        return await EditProduct();

                    //Пользователь нажал на кнопку "Импорт из CSV" ему пришло сообщение с интрукцией
                    case "/import":
                        return await SendImportFAQ();

                    case ContactEditCmd:
                        return await ContactEdit();

                    case VkEditCmd:
                        return await ForceReplyBuilder(ForceReplyVk);

                    case InstagramEditCmd:
                        return await ForceReplyBuilder(ForceReplyInstagram);

                    case ChatEditCmd:
                        return await ForceReplyBuilder(ForceReplyChat);

                    case ChannelEditCmd:
                        return await ForceReplyBuilder(ForceReplyChannel);

                    case "/export":
                        return await OrderExport();

                    case PayMethodsListCmd:
                        return await SendPaymentMethods();

                    case PaymentTypeEnableCmd:
                        return await PaymentMethodEnable();

                    case StatCmd:
                        return await SendStat();

                    case "/operators":
                        return await SendOperatorList();

                    case "GenerateKey":
                        return await GenerateKey();

                    case AddGroup:
                       return await AddBotToChat();

                    case "/cities":
                        return await SendAvailableCities();

                    case "/newcity":
                        return await ForceReplyBuilder("Введите название города");

                    case "GetCategoryStock":
                        return await SendCurrentStock(Argumetns[0],MessageId);

                    default:
                        break;
                }

                if (base.OriginalMessage.Contains("Введите название города"))
                    return await AddAvailableCity();

                if (base.CommandName.Contains(RemoveAvailableCityCmd))
                    return await RemoveAvailableCity();

                if (base.OriginalMessage.Contains(ForceReplyVk))
                    return await UpdateVk();

                if (base.OriginalMessage.Contains(ForceReplyInstagram))
                    return await UpdateInstagram();

                if (base.OriginalMessage.Contains(ForceReplyChat))
                    return await UpdateChat();

                if (base.OriginalMessage.Contains(ForceReplyChannel))
                    return await UpdateChannel();

                if (base.CommandName.Contains(RemoveOperatorCmd))
                    return await RemoveOperator();



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
        private async Task<IActionResult> SendAvailableCities()
        {
            try
            {
               await SendMessage(AvailableCitiesMsg.BuildMsg());
               return OkResult;
            }

            catch
            {
                return NotFoundResult;
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
                    AvailableСities availableСities = new AvailableСities
                    {
                        CityName = ReplyToMessageText,
                        Timestamp = DateTime.Now
                    };

                    db.AvailableСities.Add(availableСities);
                    db.SaveChanges();

                    return await SendAvailableCities();
                }
            }

            catch
            {
                return NotFoundResult;
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
                return NotFoundResult;
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
                return NotFoundResult;
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
                        await SendMessage(new BotMessage { TextMessage = "Пользователь который должен получить права оператора должен ввести следующую команду:" + Bot.BotMessage.NewLine()+ Bot.BotMessage.Italic("/key " + key.KeyValue) });
                        return OkResult;
                    }

                    else
                        return NotFoundResult;

                }
            }

            else return NotFoundResult;
           
        }

        /// <summary>
        /// Отправить сообщение со списком всех операторов
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendOperatorList()
        {
            try
            {
                if(AdminControlMsg!=null && await SendMessage(AdminControlMsg.BuildMsg()) != null)
                    return OkResult;

                else
                    return NotFoundResult;
            }

            catch
            {
                return NotFoundResult;
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
                await SendMessage(AdminPayMethodsSettingsMsg.BuildMsg(),MessageId);
                return OkResult;
            }

            catch
            {
                return NotFoundResult;
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
                AdminCurrentStockMsg = new AdminCurrentStockMessage(CategoryId);
                await SendMessage(AdminCurrentStockMsg.BuildMsg(), MessageId);
                return OkResult;
            }
            catch
            {
                return NotFoundResult;
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
                await SendMessage(AdminAllProductsViewMsg.BuildMsg());
                return OkResult;
            }

            catch
            {
                return NotFoundResult; 
            }
        }


        /// <summary>
        /// Отправить сообщение со статистикой
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendStat()
        {           

            try
            {
                
                TimeSpan diff = DateTime.Now - LastReportsRequest();
                if (diff.Minutes >= 5)
                {
                    var msgs = StatisticMsg.BuildMessage();
                    await SendMessage(msgs[0]);
                    await SendDocument(msgs[1]);
                    await SendDocument(msgs[2]);
                    InsertReportsRequest();
                    return OkResult;
                }

                else
                {
                    await SendMessage(new BotMessage { TextMessage = "Не более одного запроса в 5 минут" });
                    return OkResult;
                }
            }

            catch
            {
                return NotFoundResult;
            }
        }

        /// <summary>
        /// Вкл/откл метод оплаты
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> PaymentMethodEnable()
        {
            try
            {
                using(MarketBotDbContext db=new MarketBotDbContext())
                {
                  var conf= db.PaymentTypeConfig.Where(p => p.PaymentId == Argumetns[0]).OrderByDescending(p=>p.Id).FirstOrDefault();

                    var type = db.PaymentType.Where(p => p.Id == Argumetns[0]).FirstOrDefault();


                    if (conf != null && type.Enable == true 
                        || PaymentType.GetPaymentTypeEnum(Argumetns[0]) == Services.PaymentTypeEnum.PaymentOnReceipt &&
                        type.Enable == true) // пользователь хочет отключить метод оплаты. 
                                             //Проверям настроен ли он вообще.Если пользователь хочет отключить
                                             //метод оплаты "при получении" то conf может быть пустым т.к этот метод оплаты не требует настроек
                    {
                        type.Enable = false;
                        db.SaveChanges();
                        return await SendPaymentMethods(base.MessageId);
                    }

                    if (conf != null && type.Enable == false 
                        || PaymentType.GetPaymentTypeEnum(Argumetns[0]) == Services.PaymentTypeEnum.PaymentOnReceipt &&
                        type.Enable == false)
                    {
                        type.Enable = true;
                        db.SaveChanges();
                        return await SendPaymentMethods(base.MessageId);
                    }

                    if (conf == null && PaymentType.GetPaymentTypeEnum(Argumetns[0])!= Services.PaymentTypeEnum.PaymentOnReceipt) 
                        // метод оплаты не настроен, кроме метода оплаты "При получении"
                    {
                        await AnswerCallback("Ошибка. Данный способ оплаты не настроен", true);
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
        /// Сообщение с не согласоваными заказми
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> NoConfirmOrder()
        {
            NoConfirmOrdersMessage no = new NoConfirmOrdersMessage();
            await EditMessage(no.BuildMsg());
            return OkResult;
        }

        /// <summary>
        /// Экспорт всех заказов
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> OrderExport()
        {

            TimeSpan diff = DateTime.Now - LastReportsRequest();

            if (diff.Minutes > 1)
            {
                InsertReportsRequest();
                OrderExport export = new OrderExport();

                await SendDocument(new FileToSend { Content = export.Export(), Filename = "Orders.csv" }, "Все заказы в БД");
                return OkResult;
            }

            else
            {
                await SendMessage(new BotMessage { TextMessage = "Не более одного запроса в минуту" });
                return base.OkResult;
            }
        }

        /// <summary>
        /// Экспорт всех данных из таблицы с остатками
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> StockExport()
        {
            TimeSpan diff = DateTime.Now - LastReportsRequest();

            if (diff.Minutes > 1)
            {
                InsertReportsRequest();
                StockExport export =new StockExport();
                await SendDocument(new FileToSend { Content = export.Export(), Filename = "Stock.csv" });
                return OkResult;
            }

            else
            {
                await SendMessage(new BotMessage { TextMessage = "Не более одного запроса в минуту" });
                return base.OkResult;
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
        /// Изменит текущее сообщение с кнопками контактов.
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> ContactEdit()
        {
            if (ContactEditMsg != null &&await EditMessage(ContactEditMsg.BuildMsg()) != null)
                return base.OkResult;

            else
                return base.NotFoundResult;
        }

        /// <summary>
        /// Изменить ссылку на вк
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateVk()
        {
            using(MarketBotDbContext db=new MarketBotDbContext())
            {
               var company= db.Company.Where(c => c.Enable == true).FirstOrDefault();

                if (company != null)
                {
                    company.Vk = base.ReplyToMessageText;
                    await db.SaveChangesAsync();
                }

                return base.OkResult;
            }
        }

        /// <summary>
        /// Изменить ссылку на Инст
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateInstagram()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var company = db.Company.Where(c => c.Enable == true).FirstOrDefault();

                if (company != null)
                {
                    company.Instagram = base.ReplyToMessageText;
                    await db.SaveChangesAsync();
                }

                return base.OkResult;
            }
        }

        /// <summary>
        /// Изменить ссылку на чат
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateChat()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var company = db.Company.Where(c => c.Enable == true).FirstOrDefault();

                if (company != null)
                {
                    company.Chat = base.ReplyToMessageText;
                    await db.SaveChangesAsync();
                }

                return base.OkResult;
            }
        }

        /// <summary>
        /// Изменить ссылку на канал
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateChannel()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var company = db.Company.Where(c => c.Enable == true).FirstOrDefault();

                if (company != null)
                {
                    company.Chanel = base.ReplyToMessageText;
                    await db.SaveChangesAsync();
                }

                return base.OkResult;
            }
        }

        /// <summary>
        /// Сообщение с инстуркцией по импорту данных и csv
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendImportFAQ()
        {
            try
            {

               await base.SendMessage(new BotMessage { TextMessage = "1) Заполните csv файл " + BotMessage.NewLine() + "2) Сохраните файл как Import.csv" + BotMessage.NewLine() + "3) Отправьте файл боту" });

                using(MarketBotDbContext db=new MarketBotDbContext())
                {
                    Configuration configuration = db.Configuration.FirstOrDefault();

                    // FileId файла Пример.csv есть в базе
                    if (configuration!=null && configuration.ExampleCsvFileId!= null) 
                    {
                        FileToSend fileToSend = new FileToSend
                        {
                            Filename = "Пример.csv",
                            FileId = configuration.ExampleCsvFileId
                        };

                        var message = await SendDocument(fileToSend, "Пример заполнения");
                    }

                    // FileID в базе нет, отправляяем файл и сохраняем в бд FileID
                    if (configuration!=null && configuration.ExampleCsvFileId==null) 
                    {
                    var stream= System.IO.File.Open("Пример.csv", FileMode.Open);

                    FileToSend fileToSend = new FileToSend
                    {
                        Filename = "Пример.csv",
                        Content = stream
                    };

                    var message = await SendDocument(fileToSend, "Пример заполнения");

                    configuration.ExampleCsvFileId = message.Document.FileId;
                    db.SaveChanges();

                    }

                    // FileId файла Шаблон.csv есть в базе
                    if (configuration != null && configuration.TemplateCsvFileId != null)
                    {
                        FileToSend fileToSend = new FileToSend
                        {
                            Filename = "Шаблон.csv",
                            FileId = configuration.ExampleCsvFileId
                        };

                        var message = await SendDocument(fileToSend, "Пример заполнения");
                    }

                    // FileID в базе нет, отправляяем файл и сохраняем в бд FileID
                    if (configuration != null && configuration.TemplateCsvFileId == null) 
                    {
                        var stream = System.IO.File.Open("Шаблон.csv", FileMode.Open);

                        FileToSend fileToSend = new FileToSend
                        {
                            Filename = "Шаблон.csv",
                            Content = stream
                        };

                        var message = await SendDocument(fileToSend, "Пример заполнения");

                        configuration.TemplateCsvFileId = message.Document.FileId;
                        db.SaveChanges();

                    }

                }
                 
                return base.OkResult;
            }

            catch (Exception exp)
            {
                return base.NotFoundResult;
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

                if (AdminCmdListMsg!=null && await SendMessage(AdminCmdListMsg.BuildMsg()) != null)
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
                            +Bot.BotMessage.NewLine()+"Ключ: "+ adminKey.KeyValue;
                        await SendMessage(BotOwner, new BotMessage { TextMessage = meessage });
                        return await SendAdminControlPanelMsg();
                    }
                    else
                        return OkResult;
                }
            }
        }

        /// <summary>
        /// Присылает сообщение с инструкцией как добавить новый товар в БД.
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendInsertProductFAQ()
        {
            string Currencies = "";
            string Units = "";
            using (MarketBotDbContext db=new MarketBotDbContext())
            {
                var CurrencyList = db.Currency.ToList();

                var UnitList = db.Units.ToList();

                foreach(Currency c in CurrencyList)
                    Currencies += c.Name + " - " + c.ShortName;

                foreach (Units u in UnitList)
                    Units += u.Name + "-" + u.ShortName;
            }

            const string quote = "\"";
            string Example = "Пришлите фотографию товара, а в поле под фотографией(можно без фотографии, просто ответьте на сообщение бота) добавьте комментарий следующего вида:" +
                             " Название товара, Категория, Цена, Еденица измерения, В наличии, " + quote + "Краткое описание [не обязательно]" + quote + BotMessage.NewLine()
                             + BotMessage.Bold("Например: ") + "Хреновуха, Настойки,500, шт., 5, " + quote + "40 градусов" + quote + BotMessage.NewLine()
                             + BotMessage.Bold("Например: ") + "Рис, Крупы,100, кг., 100" + BotMessage.NewLine()
                             + BotMessage.Bold("Например: ") + "Сникерс, Конфеты, 50, г., 1000" + quote + "Вкусные конфеты. Ага" + quote
                             + BotMessage.NewLine() + BotMessage.NewLine() + BotMessage.Bold("Доступные валюты: ") + Currencies
                             + BotMessage.NewLine()+ BotMessage.Bold("Еденицы измерения: ") + Units;


            ForceReply forceReply = new ForceReply
            {
                Force = true,

                Selective = true
            };

            if (await SendMessage(new BotMessage { TextMessage = Example }) != null
                && await SendMessage(new BotMessage { TextMessage = EnterNameNewProductCmd, MessageReplyMarkup = forceReply }) != null)
                return base.OkResult;


            else
                return base.NotFoundResult;
        }


        /// <summary>
        /// Пользователь нажал на Изменить продук. Появляется сообещение с выбором категории
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> EditProduct()
        {
            //CategoryListMsg = new CategoryListMessage(AdminProductInCategoryCmd, ProductEditBot.ModuleName);
            //if (await EditMessage(CategoryListMsg.Mess()) != null)
                return base.OkResult;


            //else
            //    return base.NotFoundResult;
        }


        /// <summary>
        /// Вернуть к панели администратора
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> BackToAdminPanel()
        {
            if (await EditMessage(AdminCmdListMsg.BuildMsg()) != null)
                return OkResult;

            else
                return NotFoundResult;
        }

        /// <summary>
        /// Проверка номера телефона. Длина должна быть 11 символов и начинаться должен с 7
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private bool VerifyPhoneNumber(string number)
        {
            try
            {
                var seven = number.Substring(0, 1);

                if (number.Length == 11 && seven=="7")
                {
                    long value = Convert.ToInt64(number);
                    return true;
                }

                else
                    return false;
            }

            catch
            {
                return false;
            }
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
                return NotFoundResult;
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
