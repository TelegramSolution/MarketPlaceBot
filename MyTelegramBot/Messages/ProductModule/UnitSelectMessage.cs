using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.InlineKeyboardButtons;
using System.Security.Cryptography;
using System.Text;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;
using MyTelegramBot.BusinessLayer;



namespace MyTelegramBot.Messages.ProductModule
{
    /// <summary>
    /// выбор еденици измерения для товара
    /// </summary>
    public class UnitSelectMessage : BotMessage
    {
        private List<Units> UnitList { get; set; }

        private Telegram.Bot.Types.KeyboardButton[][] UnitBtn { get; set; }

        public override BotMessage BuildMsg()
        {
            UnitList= UnitsFunction.UnitsList();

            UnitBtn = new Telegram.Bot.Types.KeyboardButton[UnitList.Count][];

            int count = 0;

            base.TextMessage = "1";

            foreach (var unit in UnitList)
            {
                UnitBtn[count] = new Telegram.Bot.Types.KeyboardButton[1];
                UnitBtn[count][0] = new Telegram.Bot.Types.KeyboardButton("Еденица измерения:"+unit.Name);
                count++;
            }

            Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup replyKeyboard = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup(UnitBtn);

            base.MessageReplyMarkup = replyKeyboard;

            return this;
        }
    }
}
