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
    public class OrdersListMessage:BotMessage
    {

        private List<Orders> OrderList { get; set; }

        Dictionary<int, List<Orders>> Pages { get; set; }

        MarketBotDbContext db { get; set; }

        public OrdersListMessage(int PageNumber=1)
        {
            base.PageSize = 4;
            base.SelectPageNumber = PageNumber;
        }

        public override BotMessage BuildMsg()
        {

            db = new MarketBotDbContext();

            OrderList = db.Orders.Include(o => o.CurrentStatusNavigation).Include(o=>o.OrderProduct).OrderByDescending(o => o.Id).ToList();


            Pages = BuildDataPage<Orders>(OrderList, base.PageSize);

            var page = Pages[base.SelectPageNumber];

            if (page != null)
            {
                base.NextPageBtn = base.BuildNextPageBtn<Orders>(Pages, base.SelectPageNumber, AdminBot.ViewOrdersListCmd, AdminBot.ModuleName);

                base.PreviousPageBtn = base.BuildPreviousPageBtn<Orders>(Pages, base.SelectPageNumber, AdminBot.ViewOrdersListCmd, AdminBot.ModuleName);

                base.BackBtn = BuildInlineBtn("Панель администратора", BuildCallData(AdminBot.BackToAdminPanelCmd, AdminBot.ModuleName));

                base.MessageReplyMarkup = base.PageNavigatorKeyboard(base.NextPageBtn, base.PreviousPageBtn, base.BackBtn);

                base.TextMessage = "Список заказов (Всего заказов в системе " + OrderList.Count.ToString() + ")" + NewLine() +
                    "Страница " + base.SelectPageNumber.ToString() + " из " + Pages.Count.ToString() + NewLine();

                int number = 1; // порядковый номер записи

                int counter = 1;

                foreach (Orders order in page)
                {
                    number = PageSize * (SelectPageNumber - 1) + counter;

                    if(order.CurrentStatusNavigation!=null)
                        order.CurrentStatusNavigation.Status = db.Status.Find(order.CurrentStatusNavigation.StatusId);

                    if(order.CurrentStatusNavigation!=null && order.CurrentStatusNavigation.Status!=null)
                    base.TextMessage += number.ToString() + ") " + base.PackageEmodji + " Заказ №" + order.Number.ToString() + " /order" + order.Number.ToString() + NewLine() +
                        Bold("Время добавления:") + order.DateAdd.ToString() + NewLine() +
                        Bold("Текущий статус:") + order.CurrentStatusNavigation.Status.Name + NewLine() +
                        Bold("Общая стоимость:") + order.TotalPrice().ToString() + NewLine()+NewLine();

                    else
                        base.TextMessage += number.ToString() + ") " + base.PackageEmodji + " Заказ №" + order.Number.ToString() + " /order" + order.Number.ToString() + NewLine() +
                        Bold("Время добавления:") + order.DateAdd.ToString() + NewLine() +
                        Bold("Общая стоимость:") + order.TotalPrice().ToString() + NewLine() + NewLine();

                    counter++;
                }

                db.Dispose();

                return this;
            }

            else
                return null;
        }
    }
}
