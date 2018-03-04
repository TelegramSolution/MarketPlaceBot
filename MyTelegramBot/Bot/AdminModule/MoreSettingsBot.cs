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
using MyTelegramBot.Messages.Admin.PaymentsModule;

namespace MyTelegramBot.Bot.AdminModule
{
    public class MoreSettingsBot : BotCore
    {
        public const string ModuleName = "MoreSet";

        public const string VkEditCmd = "VkEdit";

        public const string InstagramEditCmd = "InstagramEdit";

        public const string ChannelEditCmd = "ChannelEdit";

        public const string ChatEditCmd = "ChatEdit";

        public const string VkEditForceReply = "Vk.com";

        public const string InstagramForceReply = "Instagram.com";

        public const string ChatEditForceReply = "TelegramChat";

        public const string ChannelEditForceReply = "TelegramChannel";

        public const string WorkTimeEditorCmd = "WorkTimeEditor";

        public const string AboutEditCmd = "AboutEdit";

        public const string AboutEditorForceReply = "О нас";

        public const string EnablePaymentMethodCmd = "EnablePaymentMethod";

        public const string SettingsPaymentMethodCmd = "SettingsPaymentMethod";

        public const string MethodOfObtaitingCmd = "MethodOfObtaiting";

        public const string QiwiListCmd = "QiwiList";

        public const string BackToMoreSettingsCmd = "BackToMoreSettings";

        public const string MoreSettingsCmd = "MoreSettings";

        public const string UpdDeliveryCmd = "UpdDelivery";

        public const string UpdPickUpCmd = "UpdPickUp";

        public const string DeliveryPriceCmd = "DeliveryPrice";

        public const string StartWorkTimeCmd = "StartWorkTime";

        public const string EndWorkTimeCmd = "EndWorkTime";

        public const string RemoveWorkTimeCmd = "RemoveWorkTime";

        public const string StartWorkTimeForceReplyCmd = "Время начала работы";

        public const string EndWorkTimeForceReplyCmd = "Время окончания работы";

        public const string DeliveryPriceEditCmd = "DeliveryPriceEdit";

        public const string FreeDeliveryRulesEditCmd = "FreeDeliveryRulesEdit";

        public const string RemoveDeliveryPriceCmd = "RemoveDeliveryPrice";

        public const string NewDeliveryPriceForceReply = "Введите стоимость доставки";

        public const string NewFreeDeliveryPriceForceReply = "Бесплатная доставка от";

        public const string QiwiEditTokenCmd = "QiwiEditToken";

        public const string QiwiRemoveCmd = "QiwiRemove";

        public const string ViewYandexKassaCmd = "ViewYandexKassa";

        public const string YandexKassaShopIdEditCmd = "YandexShopIdEdit";

        public const string YandexKassaTokenEditCmd = "YandexTokenEdit";

        public const string YandexRemoveCmd = "YandexRemove";

        public const string YandexAddCmd = "YandexAdd";

        public const string AddQiwi = "/addqiwi";

        public const string QiwiTelForcerReply = "Введите номер телефона для Qiwi";

        public const string QiwiTestConectionCmd = "QiwiTestConection";

        public const string QiwiTokenForceReply = "Введите токен Qiwi:";

        public const string PayCgfCmd = "/paycgf";

        public const string YandexShopidForceReply = "Введите идентификатор магазина";

        public const string YandexTokenForceReply = "Введите токен яндекс кассы";


        MoreSettingsMessage MoreSettingsMsg { get; set; }

        MethodOfObtaining MethodOfObtainingMsg { get; set; }

        WorkTimeMessage WorkTimeMsg { get; set; }

        DeliveryPriceMessage DeliveryPriceMsg { get; set; }

        YandexKassaEditMessage YandexKassaEditMsg { get; set; }

        QiwiListMessage QiwiListMsg { get; set; }

        AdminQiwiSettingsMessage AdminQiwiSettingsMsg { get; set; }

        AdminPayMethodsSettings AdminPayMethodsSettingMsg { get; set; }

        public MoreSettingsBot(Update _update) : base(_update)
        {

        }

        protected override void Constructor()
        {
            MoreSettingsMsg = new MoreSettingsMessage();
        }

        public async override Task<IActionResult> Response()
        {
            if (IsOwner())
            {

                if (base.OriginalMessage == StartWorkTimeForceReplyCmd)
                    return await SaveTime(StartWorkTimeForceReplyCmd);

                if (base.OriginalMessage == EndWorkTimeForceReplyCmd)
                    return await SaveTime(EndWorkTimeForceReplyCmd);

                if (base.OriginalMessage == NewDeliveryPriceForceReply)
                    return await SavePrice(NewDeliveryPriceForceReply);

                if (base.OriginalMessage == NewFreeDeliveryPriceForceReply)
                    return await SavePrice(NewFreeDeliveryPriceForceReply);

                if (base.OriginalMessage == VkEditForceReply)
                    return await SaveCompanyInfo(VkEditForceReply);

                if (base.OriginalMessage == InstagramForceReply)
                    return await SaveCompanyInfo(InstagramForceReply);

                if (base.OriginalMessage == ChatEditForceReply)
                    return await SaveCompanyInfo(ChatEditForceReply);

                if (base.OriginalMessage == ChannelEditForceReply)
                    return await SaveCompanyInfo(ChannelEditForceReply);

                if (base.OriginalMessage == AboutEditorForceReply)
                    return await SaveCompanyInfo(AboutEditorForceReply);

                if (base.OriginalMessage.Contains(QiwiTelForcerReply))
                    return await InsertQiwiTelephone();

                if (base.CommandName.Contains(PayCgfCmd))
                    return await SendQiwiEditor();

                if (base.OriginalMessage.Contains(QiwiTokenForceReply))
                    return await UpdQiwiToken();

                if (base.OriginalMessage == YandexShopidForceReply)
                    return await InsertYandexShopId();

                if (base.OriginalMessage.Contains(YandexTokenForceReply))
                    return await UpdYandexToken();

                switch (base.CommandName)
                {
                    case AddQiwi:
                        return await SendForceReplyMessage(QiwiTelForcerReply);

                    case MoreSettingsCmd:
                        return await SendMoreSettings(base.MessageId);

                    case MethodOfObtaitingCmd:
                        return await SendMethodOfObtaining();

                    case UpdDeliveryCmd:
                        return await UpdDelivery();

                    case UpdPickUpCmd:
                        return await UpdPickUp();

                    case BackToMoreSettingsCmd:
                        return await SendMoreSettings(base.MessageId);

                    case WorkTimeEditorCmd:
                        return await SendWorkTime(base.MessageId);

                    case StartWorkTimeCmd: 
                        return await TimeInputSendForceReply(StartWorkTimeForceReplyCmd);

                    case EndWorkTimeCmd:
                        return await TimeInputSendForceReply(EndWorkTimeForceReplyCmd);

                    case RemoveWorkTimeCmd:
                        return await RemoveWorkTime();

                    case DeliveryPriceCmd:
                        return await SendDeliveryPriceInfo(base.MessageId);

                    case DeliveryPriceEditCmd:
                        return await PriceInputSendForceReply(NewDeliveryPriceForceReply);

                    case FreeDeliveryRulesEditCmd:
                        return await PriceInputSendForceReply(NewFreeDeliveryPriceForceReply);

                    case RemoveDeliveryPriceCmd:
                        return await RemoveDeliveryPrice();

                    case VkEditCmd:
                        return await UrlInputSendForcerReply(VkEditForceReply);

                    case InstagramEditCmd:
                        return await UrlInputSendForcerReply(InstagramForceReply);

                    case ChatEditCmd:
                        return await UrlInputSendForcerReply(ChatEditForceReply);

                    case ChannelEditCmd:
                        return await UrlInputSendForcerReply(ChannelEditForceReply);

                    case AboutEditCmd:
                        return await SendTextMessageAndForceReply("Введите информацию", AboutEditorForceReply);

                    case SettingsPaymentMethodCmd:
                        return await SendPaymentsSettings();

                    case QiwiListCmd:
                        return await SendQiwiList();

                    case ViewYandexKassaCmd:
                        return  await SendYandexKassaView();

                    case QiwiTestConectionCmd:
                        return await QiwiTestConnection();

                    case QiwiRemoveCmd:
                        return await QiwiRemove();

                    case QiwiEditTokenCmd:
                        return await base.SendForceReplyMessage(QiwiTokenForceReply+Argumetns[0]);

                    case YandexKassaShopIdEditCmd:
                        return await SendForceReplyMessage(YandexShopidForceReply);

                    case YandexKassaTokenEditCmd:
                        return await SendForceReplyMessage(YandexTokenForceReply);

                    case YandexAddCmd:
                        return await SendForceReplyMessage(YandexShopidForceReply);

                    case YandexRemoveCmd:
                        return await RemoveYandex();

                    default:
                        return null;
                }


            }



            else
                return null;
        }

        private async Task<IActionResult> RemoveYandex()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            var yandex = db.PaymentTypeConfig.Where(p => p.PaymentId == ConstantVariable.PaymentTypeVariable.DebitCardForYandexKassa).LastOrDefault();

            if (yandex != null)
            {
                yandex.Pass = ReplyToMessageText;
                db.PaymentTypeConfig.Remove(yandex);
                db.SaveChanges();
                db.Dispose();
            }

            return await SendMoreSettings();
        }

        private async Task<IActionResult> UpdYandexToken()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            var yandex = db.PaymentTypeConfig.Where(p => p.PaymentId == ConstantVariable.PaymentTypeVariable.DebitCardForYandexKassa).LastOrDefault();

            if (yandex != null)
            {
                yandex.Pass = ReplyToMessageText;
                db.Update<PaymentTypeConfig>(yandex);
                db.SaveChanges();
                db.Dispose();
            }

            return await SendYandexKassaView();
        }

        private async Task<IActionResult> InsertYandexShopId()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            PaymentTypeConfig paymentTypeConfig = new PaymentTypeConfig
            {
                Enable = true,
                Login = ReplyToMessageText,
                TimeStamp = DateTime.Now,
                Pass = "",
                PaymentId=ConstantVariable.PaymentTypeVariable.DebitCardForYandexKassa
            };

            db.PaymentTypeConfig.Add(paymentTypeConfig);
            db.SaveChanges();
            db.Dispose();
            return await SendYandexKassaView();
        }

        private async Task<IActionResult> SendQiwiEditor ()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            int id = Convert.ToInt32(CommandName.Substring(PayCgfCmd.Length));

            var qiwi = db.PaymentTypeConfig.Find(id);

            if (qiwi != null)
            {
                AdminQiwiSettingsMsg = new AdminQiwiSettingsMessage(qiwi);
                await SendMessage(AdminQiwiSettingsMsg.BuildMsg());
                db.Dispose();
            }

            return OkResult;
        }

        private async Task<IActionResult> UpdQiwiToken()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            string token = ReplyToMessageText;

            int id =Convert.ToInt32(OriginalMessage.Substring(QiwiTokenForceReply.Length));

            var qiwi = db.PaymentTypeConfig.Find(id);

            if (qiwi != null)
            {
                qiwi.Pass = token;
                db.Update<PaymentTypeConfig>(qiwi);
                db.SaveChanges();
                db.Dispose();

                AdminQiwiSettingsMsg = new AdminQiwiSettingsMessage(qiwi);
                await SendMessage(AdminQiwiSettingsMsg.BuildMsg());

            }

            return OkResult;
        }

        private async Task<IActionResult> QiwiRemove()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            var Qiwi= db.PaymentTypeConfig.Find(Argumetns[0]);

            db.PaymentTypeConfig.Remove(Qiwi);

            db.SaveChanges();

            return await SendPaymentsSettings();
        }

        private async Task<IActionResult> QiwiTestConnection()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            var Qiwi = db.PaymentTypeConfig.Find(Argumetns[0]);

            bool test= await Services.Qiwi.QiwiFunction.TestConnection(Qiwi.Login, Qiwi.Pass);

            if (test)
                await AnswerCallback("Успех!", true);

            else
                await AnswerCallback("Ошибка соединения", true);

            db.Dispose();

            return OkResult;
        }

        private async Task<IActionResult> InsertQiwiTelephone()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            var qiwi = db.PaymentTypeConfig.Where(p => p.PaymentId == ConstantVariable.PaymentTypeVariable.QIWI && p.Login == ReplyToMessageText).FirstOrDefault();

            if (qiwi != null)
                return await SendTextMessageAndForceReply("Данный номер телефона уже добавлен", QiwiTelForcerReply);

            else
            {
                PaymentTypeConfig paymentTypeConfig = new PaymentTypeConfig
                {
                    Enable = true,
                    Login = ReplyToMessageText,
                    TimeStamp = DateTime.Now,
                    Pass = String.Empty,
                    PaymentId=ConstantVariable.PaymentTypeVariable.QIWI
                };

                db.PaymentTypeConfig.Add(paymentTypeConfig);
                db.SaveChanges();

                AdminQiwiSettingsMsg = new AdminQiwiSettingsMessage(paymentTypeConfig);
                await SendMessage(AdminQiwiSettingsMsg.BuildMsg());
                return OkResult;
            }
        }

        private async Task<IActionResult> SendYandexKassaView()
        {
            YandexKassaEditMsg = new YandexKassaEditMessage();
            await EditMessage(YandexKassaEditMsg.BuildMsg());
            return OkResult;
        }

        private async Task<IActionResult> SendQiwiList()
        {
            if(Argumetns.Count==1)
                QiwiListMsg = new QiwiListMessage(Argumetns[0]);

            else
                QiwiListMsg = new QiwiListMessage();

            await EditMessage(QiwiListMsg.BuildMsg());
            return OkResult;
        }

        private async Task<IActionResult> SendPaymentsSettings()
        {
            AdminPayMethodsSettingMsg = new AdminPayMethodsSettings();
            await EditMessage(AdminPayMethodsSettingMsg.BuildMsg());
            return OkResult;
        }

        private async Task<IActionResult> SendMoreSettings(int MessageId = 0)
        {
            await SendMessage(MoreSettingsMsg.BuildMsg(), MessageId);

            return OkResult;
        }

        /// <summary>
        /// отпрваить сообщение со способами получения заказа
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendMethodOfObtaining()
        {
            MethodOfObtainingMsg = new MethodOfObtaining(base.BotInfo);

            await EditMessage(MethodOfObtainingMsg.BuildMsg());

            return OkResult;
        }

        private async Task<IActionResult> UpdDelivery()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            ///Доставка активна, пользователь пытается ее деактивировать, тогда не останется доступных способов оплаты
            if (BotInfo.Configuration.Pickup == false && BotInfo.Configuration.Delivery)
            {
                await AnswerCallback("Должен быть доступен хотя бы один способ получения заказа", true);

            }

            // Пользователь 
            if (BotInfo.Configuration.Pickup == true)
            {
                if (BotInfo.Configuration.Delivery)
                    BotInfo.Configuration.Delivery = false;

                else
                    BotInfo.Configuration.Delivery = true;

                db.Update<Configuration>(BotInfo.Configuration);

                int sabe = db.SaveChanges();

                db.Dispose();

                return await SendMethodOfObtaining();

            }

            else
                return OkResult;
        }

        private async Task<IActionResult> SendDeliveryPriceInfo(int MessageId = 0)
        {
            DeliveryPriceMsg = new DeliveryPriceMessage(base.BotInfo);

            await SendMessage(DeliveryPriceMsg.BuildMsg(), MessageId);

            return OkResult;
        }

        private async Task<IActionResult> UpdPickUp()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            ///Доставка активна, пользователь пытается ее деактивировать, тогда не останется доступных способов оплаты
            if (BotInfo.Configuration.Delivery == false && BotInfo.Configuration.Pickup)
            {
                await AnswerCallback("Должен быть доступен хотя бы один способ получения заказа", true);

            }

            // Пользователь 
            if (BotInfo.Configuration.Delivery == true)
            {
                if (BotInfo.Configuration.Pickup)
                    BotInfo.Configuration.Pickup = false;

                else
                    BotInfo.Configuration.Pickup = true;

                db.Update<Configuration>(BotInfo.Configuration);

                int sabe = db.SaveChanges();

                db.Dispose();

                return await SendMethodOfObtaining();

            }

            else
                return OkResult;
        }

        private async Task<IActionResult> SendWorkTime(int MessageId = 0)
        {
            WorkTimeMsg = new WorkTimeMessage(base.BotInfo);

            await SendMessage(WorkTimeMsg.BuildMsg(), MessageId);

            return OkResult;
        }

        private async Task<IActionResult> TimeInputSendForceReply(string ForceRelplyMsg)
        {

           return await SendTextMessageAndForceReply("Введите время. Например: 9:00", ForceRelplyMsg);

        }

        private async Task<IActionResult> PriceInputSendForceReply(string ForceRelplyMsg)
        {
            return await SendTextMessageAndForceReply("Укажите стоимость. Например: 500", ForceRelplyMsg);

        }

        private async Task<IActionResult> UrlInputSendForcerReply(string ForceRelplyMsg)
        {
            return await SendTextMessageAndForceReply("Пришлите ссылку на ресурс", ForceRelplyMsg);
        }

        private async Task<IActionResult> SaveTime(string OriginalMessage)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {

                TimeSpan timeSpan = Convert.ToDateTime(ReplyToMessageText).TimeOfDay;


                if (OriginalMessage == StartWorkTimeForceReplyCmd)
                {
                    BotInfo.Configuration.StartTime = timeSpan;

                    db.Update<Configuration>(BotInfo.Configuration);

                    db.SaveChanges();

                    return await SendWorkTime();

                }

                if (OriginalMessage == EndWorkTimeForceReplyCmd)
                {
                    BotInfo.Configuration.EndTime = timeSpan;

                    db.Update<Configuration>(BotInfo.Configuration);

                    db.SaveChanges();

                    return await SendWorkTime();

                }

                else
                    return OkResult;
            }

            catch
            {
                await SendMessage(new BotMessage { TextMessage = "Ошибка! Неверный формат данных" });
                return await TimeInputSendForceReply(OriginalMessage);
            }

            finally
            {
                db.Dispose();
            }
        }


        private async Task<IActionResult> SavePrice(string OriginalMessage)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {

                double price = Convert.ToDouble(ReplyToMessageText);


                if (OriginalMessage == NewDeliveryPriceForceReply)
                {
                    BotInfo.Configuration.ShipPrice = price;

                    db.Update<Configuration>(BotInfo.Configuration);

                    db.SaveChanges();

                    return await SendDeliveryPriceInfo();

                }

                if (OriginalMessage == NewFreeDeliveryPriceForceReply)
                {
                    BotInfo.Configuration.FreeShipPrice = price;

                    db.Update<Configuration>(BotInfo.Configuration);

                    db.SaveChanges();

                    return await SendDeliveryPriceInfo();

                }

                else
                    return OkResult;
            }

            catch
            {
                await SendMessage(new BotMessage { TextMessage = "Ошибка! Неверный формат данных" });
                return await PriceInputSendForceReply(OriginalMessage);
            }

            finally
            {
                db.Dispose();
            }
        }

        private async Task<IActionResult> SaveCompanyInfo (string OriginalMessage)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            var Company = db.Company.FirstOrDefault();

            if (Company != null && OriginalMessage == VkEditForceReply)
                Company.Vk = ReplyToMessageText;

            if (Company != null && OriginalMessage == InstagramEditCmd)
                Company.Instagram = ReplyToMessageText;

            if (Company != null && OriginalMessage == ChatEditForceReply)
                Company.Chat = ReplyToMessageText;

            if (Company != null && OriginalMessage == ChannelEditForceReply)
                Company.Chanel = ReplyToMessageText;

            if (Company != null && OriginalMessage == AboutEditorForceReply)
                Company.Text = ReplyToMessageText;

            db.Update<Company>(Company);
            if (db.SaveChanges() > 0)
                await SendMessage(new BotMessage { TextMessage = "Сохранено" });

            return OkResult;
            


        }

        private async Task<IActionResult> RemoveWorkTime()
        {
            using(MarketBotDbContext db=new MarketBotDbContext())
            {
                BotInfo.Configuration.StartTime = null;
                BotInfo.Configuration.EndTime = null;
                db.Update<Configuration>(BotInfo.Configuration);
                db.SaveChanges();
                return await SendDeliveryPriceInfo(base.MessageId);
            }
        }

        private async Task<IActionResult> RemoveDeliveryPrice()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                BotInfo.Configuration.ShipPrice = 0;
                BotInfo.Configuration.FreeShipPrice = 0;
                db.Update<Configuration>(BotInfo.Configuration);
                db.SaveChanges();
                return await SendDeliveryPriceInfo(base.MessageId);
            }
        }
    }
}
