using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;
using MyTelegramBot.Bot.AdminModule;
using MyTelegramBot.Bot;
namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// Сообщение с текущими остатками по товорам которые только что бы приобретены. 
    /// Это сообщение отсылается после того как Оператор нажал "Выполнено"
    /// </summary>
    public class StockChangesMessage:Bot.BotMessage
    {
        private List<Stock> StockList { get; set; }
        public StockChangesMessage(List<Stock> stock)
        {
            this.StockList = stock;
        }

        public override BotMessage BuildMsg()
        {
            int counter = 1;
            base.TextMessage = Bold("Изменения в остатках:")+NewLine();
            foreach (Stock s in StockList)
            {
                if (s.Product.Unit == null)
                    using (MarketBotDbContext db = new MarketBotDbContext())
                        s.Product.Unit = db.Units.Where(u => u.Id == s.Product.UnitId).FirstOrDefault();

                if(s.Balance>0)
                base.TextMessage += counter.ToString() + ") " + s.Product.Name + " осталось " + s.Balance.ToString() + " " + s.Product.Unit.ShortName
                     + " | " + s.DateAdd.ToString() + NewLine();

                if(s.Balance<1)
                    base.TextMessage += counter.ToString() + ") " + s.Product.Name + " осталось " + s.Balance.ToString() + " " + s.Product.Unit.ShortName
                        + " | " + s.DateAdd.ToString() +Bold(" Обратите внимание! Пользователь не сможет добавить товар в корзину! ") + NewLine();

                counter++;
            }
            return this;
        }
    }
}
