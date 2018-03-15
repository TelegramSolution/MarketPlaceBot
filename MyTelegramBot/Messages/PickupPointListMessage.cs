using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// собещние с выбором пункта самовывоза
    /// </summary>
    public class PickupPointListMessage:BotMessage
    {
        List<PickupPoint> PickupPoitList { get; set; }

        private InlineKeyboardCallbackButton[][] PickupPointListBtn { get; set; }

        public PickupPointListMessage()
        {
            BackBtn = new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton("Назад", BuildCallData(Bot.OrderBot.MethodOfObtainingListCmd, Bot.OrderBot.ModuleName));
        }

        public override BotMessage BuildMsg()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
                PickupPoitList = db.PickupPoint.Where(p=>p.Enable==true).ToList();


            if(PickupPoitList!=null && PickupPoitList.Count > 0)
            {
                PickupPointListBtn= new InlineKeyboardCallbackButton[PickupPoitList.Count() + 1][];
                int counter = 0;
                foreach (PickupPoint point in PickupPoitList)
                {
                    PickupPointListBtn[counter] = new InlineKeyboardCallbackButton[1];
                    PickupPointListBtn[counter][0] = new InlineKeyboardCallbackButton(point.Name,
                        BuildCallData(Bot.OrderBot.SelectPickupPointCmd, Bot.OrderBot.ModuleName, point.Id));
                    counter++;
                }

                PickupPointListBtn[counter] = new InlineKeyboardCallbackButton[1];
                PickupPointListBtn[counter][0] = BackBtn;

                base.TextMessage = "Выберите пункт самовывоза";
                base.MessageReplyMarkup = new InlineKeyboardMarkup(PickupPointListBtn);
            }

            else
            {
                PickupPointListBtn = new InlineKeyboardCallbackButton[1][];
                PickupPointListBtn[0] = new InlineKeyboardCallbackButton[1];
                PickupPointListBtn[0][0] = BackBtn;
                base.TextMessage = "Нет доступных пунктов самовывоза. Вернитесь назад и выберите другой способ получения заказа";
                base.MessageReplyMarkup = new InlineKeyboardMarkup(PickupPointListBtn);
            }

            return this;
        }
    }
}
