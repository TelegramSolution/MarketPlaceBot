using System;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ManagementBots.Bot.Core;
using ManagementBots.Messages;
using ManagementBots.Db;
using ManagementBots.BusinessLayer;

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
                    return await SendTextMessageAndForceReply("Создайте бота с помощью @Botfather и пришлите Токен доступа", EnterBotTokenForce);

                case SelectServiceTypeCmd:
                    return await SelectServiceType();

                case EnterDurationCmd:
                    return await SaveDurationDay(Argumetns[0], Argumetns[1], Argumetns[2]);

                case BackToDurationEnterCmd:
                    return await SendEnterDurationService(Argumetns[0], Argumetns[1]);

                default:
                    return null;

            }



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
                await SendMessage(new BotMessage { TextMessage = e.Message });
                return await SendTextMessageAndForceReply("Создайте бота с помощью @Botfather и пришлите Токен доступа", EnterBotTokenForce);

            }

            finally
            {
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
        /// пользователь выбрал тарифный план
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
                    await SendMessage(new BotMessage { TextMessage = "Услуга активирована" });
                    bot.SendMessageToOwner("Нажмите сюда /admin");
                }

                else /// тафиф платной версии. высылаем сообщение с выбором периода времени аренды
                   return await SendEnterDurationService(Argumetns[0], Argumetns[1],base.MessageId);


                return OkResult;
            }

            catch (Exception e)
            {
                await SendMessage(new BotMessage { TextMessage = e.Message });
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

                var Invoice= BotConnectFunction.PaidVersion(BotId, serviceTypeId, DayDuration);

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
    }
}
