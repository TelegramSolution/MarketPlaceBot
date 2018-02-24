using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Сообщение с вариантами оплаты
    /// </summary>
    public class PaymentsMethodsListMessage:BotMessage
    {
        private InlineKeyboardCallbackButton [][] PaymentsMethodsListBtns { get; set; }

        public PaymentsMethodsListMessage()
        {
            BackBtn = new InlineKeyboardCallbackButton("Назад", BuildCallData(Bot.OrderBot.MethodOfObtainingListCmd, OrderBot.ModuleName));
        }

        public override BotMessage BuildMsg()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var methods = db.PaymentType.Where(p=>p.Enable==true).ToList();

                if (methods.Count >0)
                {

                    PaymentsMethodsListBtns = new InlineKeyboardCallbackButton[methods.Count+1][];

                    base.TextMessage = "Выберите метод оплаты. " + "\ud83d\udcb3";

                    int counter = 0;

                    foreach(PaymentType pt in methods)
                    {
                        PaymentsMethodsListBtns [counter] = new InlineKeyboardCallbackButton[1];
                        PaymentsMethodsListBtns [counter][0] = new InlineKeyboardCallbackButton(pt.Name, BuildCallData(OrderBot.PaymentMethodCmd, OrderBot.ModuleName,pt.Id));
                        counter++;
                    }

                    PaymentsMethodsListBtns[methods.Count] = new InlineKeyboardCallbackButton[1];
                    PaymentsMethodsListBtns[counter][0] = BackBtn;

                    base.MessageReplyMarkup = new InlineKeyboardMarkup(PaymentsMethodsListBtns);

                    return this;
                }

                else
                {
                    return null;
                }
            }

          
        }
    }
}
