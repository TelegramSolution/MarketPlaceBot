using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot.AdminModule;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages.Admin
{
    public class AdminQiwiSettingsMessage:BotMessage
    {
        private InlineKeyboardCallbackButton AddBtn { get; set; }

        private InlineKeyboardCallbackButton EditBtn { get; set; }

        public override BotMessage BuildMsg()
        {
            string mess = "";

            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var api = db.PaymentType.Where(q => q.Id == Bot.Core.ConstantVariable.PaymentTypeVariable.QIWI).Include(q => q.PaymentTypeConfig).FirstOrDefault();

                if (api != null)
                {
                    mess =Bold("Номер телефона: ") + api.PaymentTypeConfig.OrderByDescending(o=>o.Id).FirstOrDefault().Login + NewLine() + Bold("Ключ: ") +
                        api.PaymentTypeConfig.OrderByDescending(o => o.Id).FirstOrDefault().Pass + 
                        NewLine() +Bold("Дата добавления: ") + api.PaymentTypeConfig.OrderByDescending(o => o.Id).FirstOrDefault().TimeStamp.ToString()+
                        NewLine()+ Italic("Что такое QIWI API и где взять ключ ? ")+ "/whatisqiwiapi "+
                        NewLine() + "Вернуться в панель администратора /admin";

                    EditBtn = new InlineKeyboardCallbackButton("Изменить", BuildCallData(AdminBot.QiwiEditCmd, AdminBot.ModuleName));

                    base.MessageReplyMarkup = new InlineKeyboardMarkup(
                        new[]{
                        new[]
                        {
                            EditBtn
                        } });
                   
                 }

                else
                {
                    mess = "Данные отсутствуют Нажмите кнопку добавить" +
                        NewLine() + Italic("Что такое QIWI API и где взять ключ ? ") + "/whatisqiwiapi " +
                        NewLine() + "Вернуться в панель администратора /admin";
                    AddBtn = new InlineKeyboardCallbackButton("Добавить", BuildCallData(AdminBot.QiwiAddEdit, AdminBot.ModuleName));

                    base.MessageReplyMarkup = new InlineKeyboardMarkup(
                        new[]{
                        new[]
                        {
                            AddBtn
                        } });
                }

            }

            TextMessage = mess;

            return this;
           
        }
    }
}
