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

namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// сообщение со списком пользователей
    /// </summary>
    public class FollowerListMessage : BotMessage
    {
        private List<Follower> FollowerList { get; set; }

        Dictionary<int, List<Follower>> Pages { get; set; }

        MarketBotDbContext db { get; set; }
        public FollowerListMessage(int PageNumber=1)
        {
            base.SelectPageNumber = PageNumber;
            base.PageSize = 4;
        }

        public override BotMessage BuildMsg()
        {
            db = new MarketBotDbContext();

            FollowerList = db.Follower.ToList();        

            Pages = base.BuildDataPage<Follower>(FollowerList, base.PageSize);

            var page = Pages[SelectPageNumber];

            db.Dispose();

            if (page != null)
            {

                base.NextPageBtn = base.BuildNextPageBtn<Follower>(Pages, base.SelectPageNumber, AdminBot.ViewFollowerListCmd, AdminBot.ModuleName);

                base.PreviousPageBtn = base.BuildPreviousPageBtn<Follower>(Pages, base.SelectPageNumber, AdminBot.ViewFollowerListCmd, AdminBot.ModuleName);

                base.BackBtn = BuildInlineBtn("Панель администратора", BuildCallData(AdminBot.BackToAdminPanelCmd, AdminBot.ModuleName));

                base.MessageReplyMarkup = base.PageNavigatorKeyboard(base.NextPageBtn, base.PreviousPageBtn, base.BackBtn);

                base.TextMessage = "Список пользователей (всего " + FollowerList.Count.ToString() + ")" + NewLine() +
                    "Страница " + SelectPageNumber.ToString() + " из " + Pages.Count.ToString() + NewLine();

                int number = 1; // порядковый номер записи

                int counter = 1;

                foreach (Follower follower in page)
                {
                    number = PageSize * (SelectPageNumber - 1) + counter;

                    string firstline = String.Empty; // имя фамилия + никнейм
                    string secondline = String.Empty; // телефон
                    string reginfoline = String.Empty; // дата регистрации
                    string blockedinfoline = String.Empty;

                    if (follower.UserName != null && follower.UserName != "")
                        firstline = number.ToString() + ") " + base.ManEmodji2 + " " + follower.FirstName + " "
                             + follower.LastName + " | " + HrefUrl("https://t.me/" + follower.UserName, follower.UserName) + NewLine();
                    else
                        firstline = number.ToString() + ") " + base.ManEmodji2 + " " + follower.FirstName + " "
                                + follower.LastName + NewLine();

                    if (follower.Telephone != null && follower.Telephone != "")
                        secondline = Bold("Телефон:") + follower.Telephone + NewLine();

                    reginfoline = Bold("Дата регистрации:") + follower.DateAdd.ToString() + NewLine();

                    if (follower.Blocked)
                        blockedinfoline = Italic("Разблокировать:") + " /userunblock" + follower.Id.ToString() + NewLine();

                    else
                        blockedinfoline = Italic("Заблокировать:") + " /userblock" + follower.Id.ToString() + NewLine();

                    base.TextMessage += firstline + secondline + reginfoline + blockedinfoline +NewLine();

                    counter++;
                }

                return this;
            }

            else
                return null;
        }
    }
}
