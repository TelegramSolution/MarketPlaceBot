using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Сообщение с выбором способа получения заказа 
    /// </summary>
    public class MethodOfObtainingMessage:Bot.BotMessage
    {
        private InlineKeyboardCallbackButton DeliveryBtn { get; set; }

        private InlineKeyboardCallbackButton PickupBtn { get; set; }

        private InlineKeyboardCallbackButton [][] ButtonsList { get; set; }

        private Configuration Configuration { get; set; }

        private string BotName { get; set; }

        public MethodOfObtainingMessage(string BotName)
        {
            this.BotName = BotName;
        }

        public override BotMessage BuildMsg()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
                Configuration = db.BotInfo.Where(b => b.Name == BotName).Include(b => b.Configuration)
                    .FirstOrDefault().Configuration;

            BackBtn = BuildInlineBtn("Назад", BuildCallData(Bot.BasketBot.BackToBasketCmd, Bot.BasketBot.ModuleName),base.BasketEmodji);

            DeliveryBtn = BuildInlineBtn("Доставка",
                 BuildCallData(Bot.OrderBot.SelectMethodOfObtainingCmd, Bot.OrderBot.ModuleName, Bot.OrderBot.IsDeliveryId),base.CarEmodji);

            PickupBtn = BuildInlineBtn("Самовывоз",
                 BuildCallData(Bot.OrderBot.SelectMethodOfObtainingCmd, Bot.OrderBot.ModuleName, Bot.OrderBot.IsPickupId),base.ManEmodji);

            base.TextMessage = "Выберите способ получения заказа";

            if (Configuration!=null && Configuration.Delivery && Configuration.Pickup)
            {
                ButtonsList = new InlineKeyboardCallbackButton[2][];
                ButtonsList[0] = new InlineKeyboardCallbackButton[2];
                ButtonsList[0][0] = DeliveryBtn;
                ButtonsList[0][1] = PickupBtn;
                ButtonsList[1] = new InlineKeyboardCallbackButton[1];
                ButtonsList[1][0] = BackBtn;
            }

            if(Configuration!=null && Configuration.Delivery  && !Configuration.Pickup)
            {
                ButtonsList = new InlineKeyboardCallbackButton[2][];
                ButtonsList[0] = new InlineKeyboardCallbackButton[1];
                ButtonsList[0][0] = DeliveryBtn;
                ButtonsList[1] = new InlineKeyboardCallbackButton[1];
                ButtonsList[1][0] = BackBtn;
            }

            if(Configuration!=null && !Configuration.Delivery && Configuration.Pickup)
            {
                ButtonsList = new InlineKeyboardCallbackButton[2][];
                ButtonsList[0] = new InlineKeyboardCallbackButton[1];
                ButtonsList[0][0] = PickupBtn;
                ButtonsList[1] = new InlineKeyboardCallbackButton[1];
                ButtonsList[1][0] = BackBtn;
            }

            if(Configuration==null || Configuration!=null && !Configuration.Delivery && !Configuration.Pickup)
            {
                ButtonsList = new InlineKeyboardCallbackButton[1][];
                ButtonsList[0] = new InlineKeyboardCallbackButton[1];
                ButtonsList[0][0] = BackBtn;
            }

            base.MessageReplyMarkup = new InlineKeyboardMarkup(ButtonsList);
            return this;
        }

    }
}
