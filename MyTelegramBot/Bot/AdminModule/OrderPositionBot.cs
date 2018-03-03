using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Bot
{
    public class OrderPositionBot:BotCore
    {
        public const string ModuleName = "OrderPos";

        public const string GetPositionCmd = "GetPosition";

        public const string AddToPositionCmd = "AddToPosition";

        public const string RemoveFromPositionCmd = "RemoveFromPosition";

        private int PositionId { get; set; }

        private OrderPositionEditMessage OrderPositionEditMsg { get; set; }

        private OrderPositionProccess OrderPositionProccessMsg { get; set; }
        public OrderPositionBot(Update _update) : base(_update)
        {
         
        }

        protected override void Constructor()
        {
            try
            {
                if (base.Argumetns.Count > 0)
                {
                    PositionId = Argumetns[0];
                    OrderPositionEditMsg = new OrderPositionEditMessage(PositionId);
                }
            }

            catch
            {

            }
        }

        public async override Task<IActionResult> Response()
        {
            switch (base.CommandName)
            {
                case GetPositionCmd:
                    return await GetPostion();

                case AddToPositionCmd:
                    return await AddToPosition();

                case RemoveFromPositionCmd:
                    return await RemoveFromPosition();

                default:
                    return null;
            }

                             
        }

        private async Task<IActionResult> GetPostion()
        {

            if (await EditMessage(OrderPositionEditMsg.BuildMsg()) != null)
                return base.OkResult;

            else
                return base.NotFoundResult;
        }

        private async Task<IActionResult> AddToPosition()
        {
            int Amount = 1;
            OrderPositionProccessMsg = new OrderPositionProccess(PositionId, Amount);
            if (await EditMessage(OrderPositionProccessMsg.BuildMessage()) != null)
                return base.OkResult;

            else
                return base.NotFoundResult;
        }

        private async Task<IActionResult> RemoveFromPosition()
        {
            int Amount = -1;
            OrderPositionProccessMsg = new OrderPositionProccess(PositionId, Amount);
            if (await EditMessage(OrderPositionProccessMsg.BuildMessage()) != null)
                return base.OkResult;

            else
                return base.NotFoundResult;
        }
    }
}
