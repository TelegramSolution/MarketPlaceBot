using System;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ManagementBots.Bot.Core;
using ManagementBots.Messages;
using ManagementBots.Db;
using ManagementBots.BusinessLayer;
using ManagementBots.MyExeption;

namespace ManagementBots.Bot
{
    public class ConnectBot : BotCore
    {
        public const string ModuleName = "Connect";

        public const string SelectServiceTypeCmd = "SlctService";

        public const string RequestBotTokenCmd = "RequestBotToken";

        public const string EnterBotTokenForce = "Введите токен";

        public const string EnterDurationCmd = "EnterDuration";

        public const string CheckPayCmd = "CheckPay";

        public const string BackToDurationEnterCmd = "BackToDurationEnter";

        public const string EnterpriseVersionCmd = "EnterpriseVersion";

        public const string AboutCmd = "About";

        public const string GetBotCmd = "GetBot";

        public const string MyBotsCmd = "MyBot";

        public const string PaidVersionCmd = "PaidVersion";

        private BotConnectFunction BotConnectFunction { get; set; }



        public ConnectBot(Update _update) : base(_update)
        {

        }

        protected override void Initializer()
        {

        }

        public async override Task<IActionResult> Response()
        {
            if (base.OriginalMessage == EnterBotTokenForce)
                await InstertBot(ReplyToMessageText);

            switch (base.CommandName)
            {
                case RequestBotTokenCmd:
                    return await SendTextMessageAndForceReply("Создайте бота с помощью @Botfather и пришлите Токен доступа" 
                                                              + Core.BotMessage.NewLine()+ 
                                                              Core.BotMessage.HrefUrl("https://drive.google.com/open?id=1f3wx3gaQSgaT50f_1dE9HsGJTUWIRcpG", 
                                                              "Как создать бота с помощью BotFather?"), EnterBotTokenForce);

                case SelectServiceTypeCmd:
                    return await SelectServiceType();

                case EnterDurationCmd:
                    return await SaveDurationDay(Argumetns[0], Argumetns[1], Argumetns[2]);

                case BackToDurationEnterCmd:
                    return await SendEnterDurationService(Argumetns[0], Argumetns[1]);

                case PaidVersionCmd:
                    return await SendEnterDurationService(Argumetns[0], Argumetns[1], base.MessageId);

                case CheckPayCmd:
                    return await CheckPay(Argumetns[0], Argumetns[1]);

                case EnterpriseVersionCmd:
                    return await SendEnterpriseInfo();

                case AboutCmd:
                    return await SendAboutInfo();

                case MyBotsCmd:
                    return await SendMyBots();

                case GetBotCmd:
                    return await SendBotInfo();

                default:
                    return null;

            }



        }

        private async Task<IActionResult> SendBotInfo()
        {
            BotMessage = new BotInfoMessage(Argumetns[0]);

            await EditMessage(BotMessage.BuildMsg());

            return OkResult;
        }

        private async Task<IActionResult> SendMyBots()
        {
            BotMessage = new MyBotsMessage(FollowerId);

            BotMessage.BuildMsg();

            if (BotMessage != null)
                await EditMessage(BotMessage);

            else
                await AnswerCallback("У вас еще нет подключенных ботов", true);

            return OkResult;
        }

        private async Task<IActionResult> SendAboutInfo()
        {
            BotMessage = new AboutMessage();

            await EditMessage(BotMessage.BuildMsg());

            return OkResult;
        }

        private async Task<IActionResult> SendEnterpriseInfo()
        {
            BotMessage =new EnterpriseVersionMessage();

            await EditMessage(BotMessage.BuildMsg());

            return OkResult;
        }

        /// <summary>
        /// Добавляем бота в бд
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        private async Task<IActionResult> InstertBot(string Token)
        {
            try
            {
                var Getme = await TelegramFunction.GetMe(Token);

                BotConnectFunction = new BotConnectFunction();

                var bot = BotConnectFunction.AddBot(Getme, Token, base.FollowerId);

                if (bot != null)
                    return await SendServiceTypeList(bot.Id);

                else
                    return OkResult;
            }


            catch (Exception e)
            {
                if (e.Message.Contains("token"))
                    await SendMessage(new BotMessage { TextMessage= "Ошибка! Не верный токен" });

                return await SendTextMessageAndForceReply("Создайте бота с помощью @Botfather и пришлите Токен доступа", EnterBotTokenForce);

            }

            finally
            {
                if(BotConnectFunction!=null)
                    BotConnectFunction.Dispose();
            }
        }


        /// <summary>
        /// Собщение с выбором тарифа для нового бота 
        /// </summary>
        /// <param name="BotId"></param>
        /// <returns></returns>
        private async Task<IActionResult> SendServiceTypeList(int BotId)
        {
            BotMessage = new SelectServiceType(BotId);

            await SendMessage(BotMessage.BuildMsg());

            return OkResult;
        }

        /// <summary>
        /// пользователь выбрал тарифный план. 
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SelectServiceType()
        {
            try
            {
                BotMngmntDbContext dbContext = new BotMngmntDbContext();

                var ServiceType = dbContext.ServiceType.Find(Argumetns[1]);

                if (ServiceType.IsDemo) // это тариф пробной версии. Запускаем сразу
                {
                    BotConnectFunction = new BotConnectFunction();

                    SendAction(Telegram.Bot.Types.Enums.ChatAction.Typing);

                    var bot = await BotConnectFunction.SelectServiceType(Argumetns[0], Argumetns[1]);

                    BotMessage = new ServiceInfoMessage(bot, bot.Service);

                    await EditMessage(BotMessage.BuildMsg());

                    await SendMessage(new BotMessage { TextMessage = "Услуга активирована. Отпрвьте вашему боту команду /admin" });
                    bot.SendMessageToOwner("Нажмите сюда /admin");

                    await SendMessage(Convert.ToInt32(BotInfo.OwnerChatId),new BotMessage { TextMessage="Подключен новый бот @"+bot.BotName + " | Демоверсия" });
                }

                else /// тафиф платной версии. высылаем сообщение с выбором периода времени аренды
                   return await SendEnterDurationService(Argumetns[0], Argumetns[1],base.MessageId);


                return OkResult;
            }

            catch (Exception e)
            {
                await SendMessage(new BotMessage { TextMessage = e.Message });
                await SendMessage(Convert.ToInt32(BotInfo.OwnerChatId), new BotMessage { TextMessage = e.Message });
                return OkResult;
            }

            finally
            {
                if(BotConnectFunction!=null)
                    BotConnectFunction.Dispose();
            }
        }

        private async Task<IActionResult> SendEnterDurationService(int BotId, int serviceTypeId, int MessageId=0)
        {
            BotMessage = new EnterDurationServiceMessage(serviceTypeId, BotId);

            await SendMessage(BotMessage.BuildMsg(), MessageId);

            return OkResult;
        }

        private async Task<IActionResult> SaveDurationDay(int BotId,int serviceTypeId, int DayDuration)
        {
            try
            {
                BotConnectFunction = new BotConnectFunction();

                var Invoice= BotConnectFunction.SelectPaidVersion(BotId, serviceTypeId, DayDuration);

                BotMessage = new InvoiceViewMessage(Invoice, BotId, serviceTypeId);

                await EditMessage(BotMessage.BuildMsg());

                return OkResult;

            }

            catch (Exception e)
            {
                await SendMessage(new BotMessage { TextMessage = e.Message });
                return OkResult;
            }
        }

        private async Task<IActionResult>CheckPay(int BotId, int InvoiceId)
        {
            try
            {
                BotConnectFunction = new BotConnectFunction();
                base.SendAction(Telegram.Bot.Types.Enums.ChatAction.Typing);
                var bot= await BotConnectFunction.CheckPay(BotId, InvoiceId);

                BotMessage = new BotInfoMessage(bot);

                await base.EditMessage(BotMessage.BuildMsg());

                return OkResult;
            }

            catch (BotInstallExeption.BotIsLaunchedExeption e)
            {
                await base.AnswerCallback(e.Message, true);
                await SendMessage(Convert.ToInt32(BotInfo.OwnerChatId), new BotMessage { TextMessage = e.Message });
                return OkResult;
            }

            catch(Exception e)
            {
                await base.AnswerCallback(e.Message, true);
                await SendMessage(Convert.ToInt32(BotInfo.OwnerChatId), new BotMessage { TextMessage = e.Message });
                return OkResult;
            }

            finally
            {
                BotConnectFunction.Dispose();
            }
        }
    }
}
