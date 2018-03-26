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
using MyTelegramBot.Messages.Admin.Notification;

namespace MyTelegramBot.Bot.AdminModule
{
    /// <summary>
    /// Управление рассылками
    /// </summary>
    public class NotificationBot:BotCore
    {
        public const string ModuleName = "Notifi";

        /// <summary>
        /// Кнопка создать рассылку
        /// </summary>
        public const string NotificationCreateCmd = "NotificationCreate";

        /// <summary>
        /// Отправить рассылку
        /// </summary>
        public const string NotificationSendCmd = "NotificationSend";

        /// <summary>
        /// Просмотр всех рассылок в бд
        /// </summary>
        public const string NotificationViewCmd = "NotificationView";


        /// <summary>
        /// детали рассылки
        /// </summary>
        public const string GetNotificationCmd = "/notifi";

        /// <summary>
        /// удалить еще не отправленную рассылку
        /// </summary>
        public const string NotificationRemoveCmd = "NotificationRemoveCmd";



        public const string EnterNotifiTextForceReply = "Введите текст рассылки";

        int NotifiId { get; set; }

        public NotificationBot(Update update):base(update)
        {

        }

        protected override void Initializer()
        {
            if (Argumetns.Count > 0)
                NotifiId = Argumetns[0];
        }

        public async override Task<IActionResult> Response()
        {
            if (IsOwner())
            {
                if (OriginalMessage == EnterNotifiTextForceReply)
                    return await AddNotifi();

                if (base.CommandName.Contains(GetNotificationCmd))
                    return await SendNotifiDetails();

                switch (base.CommandName)
                {

                    case NotificationViewCmd:
                        {
                            if (Argumetns.Count == 0)
                                return await SendNotifiList(MessageId:MessageId);

                            else
                                return await SendNotifiList(Argumetns[0],MessageId);
                        }

                    case NotificationSendCmd:
                        return await SendNewNotifi();

                    case NotificationRemoveCmd:
                        return await RemoveNotifi();

                    case NotificationCreateCmd:
                        return await SendForceReplyMessage(EnterNotifiTextForceReply);

                    default:
                        return null;
                }


            }

            else
                return null;
        }

        private async Task<IActionResult> SendNotifiList(int PageNumber=1,int MessageId=0)
        {
            BotMessage = new NotificationListMessage(PageNumber);
            await SendMessage(BotMessage.BuildMsg(), MessageId);
            return OkResult;
        }


        private async Task<IActionResult> SendNewNotifi()
        {
           
            var Followers = FollowerFunction.GetFollowerList();

            var Notifi = NotificationFunction.GetNotification(NotifiId);

            if (Notifi!=null && !Notifi.Sended)
            {
                Notifi = NotificationFunction.NotificationIsSended(NotifiId);

                await SendNotifiEditor(Notifi);

                foreach (var follower in Followers)
                {
                    await SendMessage(follower.ChatId, new BotMessage { TextMessage = Notifi.Text });

                    //пауза три секунды. Если начать отправлять все сразу телегам будет выдавать ошибку.
          
                    System.Threading.Thread.Sleep(300);
                }
            }

            if(Notifi!=null && Notifi.Sended)
            {
                await AnswerCallback("Рассылка уже отправлена", true);
            }

            if (Notifi == null)
                await AnswerCallback("Данные отсутствуют", true);

            return OkResult;
        }

        private async Task<IActionResult> SendNotifiEditor(int Id, int MessageId=0)
        {
            var notifi = NotificationFunction.GetNotification(Id);

            if (notifi != null)
            {
                BotMessage = new EditorNotificationMessage(notifi);
                await SendMessage(BotMessage.BuildMsg(), MessageId);
                return OkResult;
            }

            else
            {
                await AnswerCallback("Данные отсутсвуют", true);
                return OkResult;
            }
        }

        private async Task<IActionResult> SendNotifiEditor(Notification notification)
        {
            if (notification != null)
            {
                BotMessage = new EditorNotificationMessage(notification);
                await SendMessage(BotMessage.BuildMsg(), MessageId);
                return OkResult;
            }

            else
            {
                await AnswerCallback("Данные отсутсвуют", true);
                return OkResult;
            }
        }

        private async Task<IActionResult> RemoveNotifi()
        {
            var Notifi = NotificationFunction.GetNotification(NotifiId);

            if (Notifi!=null && !Notifi.Sended && NotificationFunction.RemoveNotification(NotifiId) > 0)
            {
                await AnswerCallback("Удалено", true);
                await SendNotifiList();

            }

            if (Notifi == null)
            {
                await AnswerCallback("Данные отсутствуют", true);
            }

            if (Notifi!=null && Notifi.Sended)
            {
                await AnswerCallback("Рассылка уже отправлена пользователям", true);
            }

            return OkResult;
        }

        private async Task<IActionResult> AddNotifi()
        {
            var list = NotificationFunction.NotificationList();

            if (list.LastOrDefault() != null && list.LastOrDefault().DateAdd.Value.Year == DateTime.Now.Year &&
                list.LastOrDefault().DateAdd.Value.DayOfYear == DateTime.Now.DayOfYear)
            {
                await SendMessage(new BotMessage {TextMessage= "Сегодня уже была сделана рассылка. Попробуйте завтра" });
            }

            else
            {
                string text = base.ReplyToMessageText;

                var notifi = NotificationFunction.InsertNotification(text, FollowerId);

                BotMessage = new EditorNotificationMessage(notifi);

                await SendMessage(BotMessage.BuildMsg());
            }

            return OkResult;
        }

        private async Task<IActionResult> SendNotifiDetails()
        {
            try
            {
                NotifiId =Convert.ToInt32(CommandName.Substring(GetNotificationCmd.Length));

                var Notifi = NotificationFunction.GetNotification(NotifiId);

                if (Notifi != null)
                {
                    BotMessage = new NotificationDetailsMessage(Notifi);
                    await SendMessage(BotMessage.BuildMsg());
                }

                return OkResult;
            }

            catch
            {
                return OkResult;
            }
        }

    }
}
