using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.InlineQueryResults;
using MyTelegramBot.Bot;
using Newtonsoft.Json;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// Сообщение с выбором новой еденицы измерения для товара
    /// </summary>
    public class UnitListMessage:BotMessage
    {
        private InlineKeyboardCallbackButton [][] UnitsBtn { get; set; }

        private int ProductId { get; set; }
        public UnitListMessage(int ProductId)
        {
            this.ProductId = ProductId;
            BackBtn = new InlineKeyboardCallbackButton("Назад", BuildCallData("BackToProductEditor", Bot.ProductEditBot.ModuleName, ProductId));
        }

        public override BotMessage BuildMsg()
        {
            using(MarketBotDbContext db=new MarketBotDbContext())
            {
                var units= db.Units.ToList();

                UnitsBtn = new InlineKeyboardCallbackButton[(units.Count/2)+1][];

                for (int i=0; i < units.Count / 2; i=i+1)
                {
                    UnitsBtn[i] = new InlineKeyboardCallbackButton[2];
                    UnitsBtn[i][0] = new InlineKeyboardCallbackButton(units[i*2].Name + "- " + units[i * 2].ShortName, BuildCallData("UpdateProductUnit",Bot.ProductEditBot.ModuleName ,ProductId, units[i * 2].Id));
                    UnitsBtn[i][1] = new InlineKeyboardCallbackButton(units[(i*2)+1].Name + "- " + units[(i * 2) + 1].ShortName, BuildCallData("UpdateProductUnit", Bot.ProductEditBot.ModuleName, ProductId, units[(i * 2) + 1].Id));
                }

                UnitsBtn[UnitsBtn.Length-1] = new InlineKeyboardCallbackButton[1];
                UnitsBtn[UnitsBtn.Length-1][0] = BackBtn;

                base.MessageReplyMarkup = new InlineKeyboardMarkup(UnitsBtn);

                base.TextMessage = "Выберите еденицу измерения";

                return this; 

            }
        }
    }
}
