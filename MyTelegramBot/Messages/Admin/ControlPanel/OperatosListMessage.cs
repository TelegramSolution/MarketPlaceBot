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
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// Сообщение со списком операторов
    /// </summary>
    public class OperatosListMessage:BotMessage
    {
        private InlineKeyboardCallbackButton NewOperatorBtn { get; set; }

        private InlineKeyboardButton SearchOperatorBtn { get; set; }

        public override BotMessage BuildMsg()
        {
            base.TextMessage =Bold("Список операторов системы:");
            string OperatorsList = "";
            SearchOperatorBtn = InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Поиск" + base.SearchEmodji, InlineFind.FindOperators+"|");
            using (MarketBotDbContext db=new MarketBotDbContext())
            {
               var operators= db.Admin.Where(a => a.Enable).Include(a=>a.Follower).ToList();            

                if (operators != null)
                {
                    int counter = 1;

                    foreach(var op in operators)
                    {
                        OperatorsList+=NewLine()+ counter.ToString() + ") " +base.ManAndComputerEmodji + " " + op.Follower.FirstName + " | телефон: " + op.Follower.Telephone+
                            NewLine()+ "забрать права оператора: /removeoperator"+op.Id.ToString()+NewLine();
                        counter++;
                    }
                }
            }

            base.TextMessage += OperatorsList+ NewLine() + Italic("Оператор системы имеет следующие права доступа: Обрабатывать заказы, Обрабатывать заявки технической поддержки.");
            NewOperatorBtn = new InlineKeyboardCallbackButton("Создать оператора",BuildCallData(Bot.AdminModule.OperatorBot.GenerateKeyCmd, Bot.AdminModule.OperatorBot.ModuleName));
            base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            SearchOperatorBtn
                        },
                new[]
                        {
                            NewOperatorBtn
                        },
                new[]
                        {
                            BackToAdminPanelBtn()
                        },


                 });

            return this;
        }
    }
}
