using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class MethodOfObtainingInfoMessage:Bot.BotMessage
    {
        private Configuration Configuration { get; set; }

        private IList<PickupPoint> PickupPointList { get; set; }

        private string Pickup { get; set; }

        private string Delivery { get; set; }

        private string ShipPrice { get; set; }

        private string FreeShipInfo { get; set; }

        public MethodOfObtainingInfoMessage BuildMessage()
        {
            using(MarketBotDbContext db=new MarketBotDbContext())
            {

                Configuration = db.BotInfo.Where(b => b.Name == Bot.GeneralFunction.GetBotName()).Include(b=>b.Configuration)
                    .FirstOrDefault().Configuration;

                Configuration.Currency = db.Currency.Find(Configuration.CurrencyId);

                //PickupPointList = db.PickupPoint.ToList();



                if (Configuration.Pickup)
                    Pickup = "Самовывоз";

                if (Configuration.Delivery)
                    Delivery = "Доставка";

                if (Configuration.Delivery && Configuration.ShipPrice > 0)
                    ShipPrice = Bold("Стоимость доставки:") + Configuration.ShipPrice.ToString();

                if (Configuration.Delivery && Configuration.FreeShipPrice > 0)
                    FreeShipInfo = "Бесплатная доставка от " + Configuration.FreeShipPrice + " " + Configuration.Currency.ShortName;

                base.TextMessage = "Способы получения заказа:" + NewLine() +
                                    Pickup + NewLine() +
                                    Delivery + NewLine() + NewLine() +
                                    ShipPrice + NewLine() +
                                    FreeShipInfo;

                BackBtn = new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton("Назад", BuildCallData("MainMenu",Bot.MainMenuBot.ModuleName));

                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                    new[]{
                    new[]
                        {
                            BackBtn
                        },
                    });

                return this;


            }
        }
    }
}
