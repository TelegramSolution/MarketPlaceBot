using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Bot.AdminModule
{
    public class HelpProcess: BotCore,IProcessing
    {
        private Messages.Admin.HelpDeskMiniViewMessage HelpDeskMiniViewMsg { get; set; }
        public HelpProcess(Update update):base(update)
        {

        }

        protected override void Initializer()
        {

        }

        public override Task<IActionResult> Response()
        {
            return null;
        }


        public async Task<bool> CheckNotInWork<T>(T t)
        {
            if ((t as HelpDesk) != null && (t as HelpDesk).InWork == false) // заявка ненаходится в обработке
            {
                await SendMessage(new BotMessage { TextMessage = "Необходимо взять заявку в обработку" });
                return true;
            }

            else
                return false;
        }

        public async Task<bool> CheckInWork<T>(T t)
        {
            //у кого в работе в данный момент
            var in_work = (t as HelpDesk).HelpDeskInWork.OrderByDescending(h => h.Id).FirstOrDefault();

            if (in_work != null && in_work.FollowerId == FollowerId)
                return true;

            if (in_work != null && in_work.FollowerId != FollowerId)
                return !await CheckInWorkOfAnotherUser(in_work); // функция возвращает ТРУ если она в обработке у какого то другого пользвателя

            if (in_work == null && (t as HelpDesk).InWork == false)
                return await CheckNotInWork((t as HelpDesk));

            else
                return false;
        }

        public async Task<bool> CheckIsDone<T>(T t)
        {

             //заявка уже закрыта
            if ((t as HelpDesk) != null && (t as HelpDesk).Closed == true)
            {
                await SendMessage(new BotMessage { TextMessage = "Заявка уже закрыта" });
                return true;
            }

            else
                return false;
        }

        public async Task<bool> NotifyChanges(string text, int Id)
        {
            try
            {
                HelpDeskMiniViewMsg = new HelpDeskMiniViewMessage(text, Id);
                var mess = HelpDeskMiniViewMsg.BuildMsg();

                return await SendMessageAllBotEmployeess(mess);
            }

            catch
            {
                return false;
            }
        }

        public async Task<bool> CheckInWorkOfAnotherUser<T>(T t)
        {
            if ((t as HelpDeskInWork) != null && (t as HelpDeskInWork).FollowerId != FollowerId)
            //заявка в обрабтке у другого пользователя
            {
                await SendMessage(new BotMessage { TextMessage = "Заявка в обработке у пользователя: " + GeneralFunction.FollowerFullName((t as HelpDeskInWork).FollowerId) });
                return true;
            }

            else
                return false;
        }
    }
}
