using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using MyTelegramBot.Messages.Admin;

namespace MyTelegramBot.Bot.AdminModule
{
    public class OrderProcess:Bot.BotCore,IProcessing
    {
        private OrderMiniViewMessage OrderMiniViewMsg { get; set; }
        public OrderProcess(Update update):base(update)
        {

        }

        protected override void Constructor()
        {

        }

        public override Task<IActionResult> Response()
        {
            throw new NotImplementedException();
        }


        public async Task<bool> CheckNotInWork<T>(T t)
        {
            try
            {
                if ((t as Orders) != null && (t as Orders).OrdersInWork.Count == 0 ||
                    (t as Orders) != null && (t as Orders).OrdersInWork.OrderByDescending(o => o.Id).FirstOrDefault().InWork == false) // заявка ненаходится в обработке
                {
                    await SendMessage(new BotMessage { TextMessage = "Необходимо взять заказ в обработку" });
                    return true;
                }

                else
                    return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> CheckInWork<T>(T t)
        {
            //у кого в работе в текущий момент
            var in_work = (t as Orders).OrdersInWork.OrderByDescending(h => h.Id).FirstOrDefault();

            if (in_work != null && in_work.FollowerId == FollowerId && in_work.InWork==true)
                return true;

            if (in_work != null && in_work.InWork == true && in_work.FollowerId != FollowerId)
                return !await CheckInWorkOfAnotherUser(in_work);

            if (in_work == null)
                return !await CheckNotInWork((t as Orders));

            else
                return false;
        }

        public async Task<bool> CheckIsDone<T>(T t)
        {

            //заявка уже закрыта
            if ((t as Orders) != null && (t as Orders).DoneId!=null)
            {
                await SendMessage(new BotMessage { TextMessage = "Заказ уже выполнен" });
                return true;
            }

            else
                return false;
        }

        public async Task<bool> NotifyChanges(string text, int Id)
        {
            try
            {

                OrderMiniViewMsg = new OrderMiniViewMessage(text, Id);
                return await SendMessageAllBotEmployeess(OrderMiniViewMsg.BuildMsg());
            }

            catch
            {
                return false;
            }
        }

        public async Task<bool> CheckInWorkOfAnotherUser<T>(T t)
        {
            if ((t as OrdersInWork) != null && (t as OrdersInWork).FollowerId != FollowerId)
            //заявка в обрабтке у другого пользователя
            {
                await SendMessage(new BotMessage { TextMessage = "Заказ в обработке у пользователя: " + GeneralFunction.FollowerFullName((t as OrdersInWork).FollowerId) });
                return true;
            }

            else
                return false;
        }
    }
}
