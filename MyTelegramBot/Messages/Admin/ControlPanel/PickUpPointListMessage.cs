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
using MyTelegramBot.Bot.AdminModule;
using MyTelegramBot.Bot.Core;
using MyTelegramBot.BusinessLayer;


namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// Управление пунктами самовывоза
    /// </summary>
    public class PickUpPointListMessage : BotMessage
    {
        private List<PickupPoint> PickupList { get; set; }

        private Dictionary<int, List<PickupPoint>> Pages { get; set; }


        public PickUpPointListMessage (int PageNumber = 1)
        {
            base.SelectPageNumber = PageNumber;
            base.PageSize = 5;
        }

        public override BotMessage BuildMsg()
        {

            PickupList = PickUpPointFunction.PickUpPointList();

            Pages = base.BuildDataPage<PickupPoint>(PickupList, base.SelectPageNumber);
            

            if (Pages!=null && Pages.Count>0 && Pages.Count>= SelectPageNumber && Pages[SelectPageNumber]!=null)
            {
                var page = Pages[SelectPageNumber];

                base.MessageReplyMarkup = base.PageNavigatorKeyboard<PickupPoint>(Pages, AdminBot.ViewPickupPointCmd, AdminBot.ModuleName, base.BackToAdminPanelBtn());

                base.TextMessage = "Список пунктов самовывоза ( всего " + PickupList.Count.ToString() + " )" + NewLine() +
                    "Страница " + SelectPageNumber.ToString() + " из " + Pages.Count.ToString() + NewLine();

                int number = 1; // порядковый номер записи

                int counter = 1;

                foreach (var pickup in PickupList)
                {
                    number = PageSize * (SelectPageNumber - 1) + counter;

                    if(pickup.Enable)
                        base.TextMessage+= number.ToString() + ") " + pickup.Name + " | скрыть /pickupdisable" + pickup.Id.ToString() + NewLine();

                    else
                        base.TextMessage += number.ToString() + ") " + pickup.Name + " | отображать /pickupenable" + pickup.Id.ToString() + NewLine();

                    counter++;
                }

                base.TextMessage += NewLine() + " Добавить новый пункт самовывоза /addpickuppoint";

            }

            else
            {
                base.TextMessage += NewLine() + " Добавить новый пункт самовывоза /addpickuppoint";

                base.MessageReplyMarkup = base.BackToAdminPanelKeyboard();

            }


            return this;

        }

    }
}
