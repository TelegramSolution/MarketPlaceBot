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
    /// Оценка от 1 до 5 к отзыву
    /// </summary>
    public class RaitingMessage:BotMessage
    {

        private InlineKeyboardCallbackButton OneBtn { get; set; }

        private InlineKeyboardCallbackButton TwoBtn { get; set; }

        private InlineKeyboardCallbackButton ThreeBtn { get; set; }

        private InlineKeyboardCallbackButton FourBtn { get; set; }

        private InlineKeyboardCallbackButton FiveBtn { get; set; }

        private int FeedBackId { get; set; }

        MarketBotDbContext db;

        FeedBack FeedBack { get; set; }

        public RaitingMessage(int FeedBackId)
        {
            this.FeedBackId = FeedBackId;
        }

        public RaitingMessage(FeedBack FeedBack)
        {
            this.FeedBack = FeedBack;

        }
        public override BotMessage BuildMsg()
        {
            OneBtn = BuildInlineBtn("1", BuildCallData(OrderBot.SelectRaitingCmd, OrderBot.ModuleName, FeedBackId,1),base.StartEmodji);

            TwoBtn = BuildInlineBtn("2", BuildCallData(OrderBot.SelectRaitingCmd, OrderBot.ModuleName, FeedBackId,2), base.StartEmodji);

            ThreeBtn = BuildInlineBtn("3", BuildCallData(OrderBot.SelectRaitingCmd, OrderBot.ModuleName, FeedBackId,3), base.StartEmodji);

            FourBtn = BuildInlineBtn("4", BuildCallData(OrderBot.SelectRaitingCmd, OrderBot.ModuleName, FeedBackId,4), base.StartEmodji);

            FiveBtn = BuildInlineBtn("5", BuildCallData(OrderBot.SelectRaitingCmd, OrderBot.ModuleName, FeedBackId,5), base.StartEmodji);

            db = new MarketBotDbContext();

            if (FeedBack == null && FeedBackId > 0)
                FeedBack = db.FeedBack.Find(FeedBackId);

            db.Dispose();

            if (FeedBack != null)
            {
                base.TextMessage = FeedBack.Text+NewLine()+Italic("Введите оценку от 1 до 5");
                SetKeyBoard();
                
                return this;
            }

            else
            {
                return null;
            }
        }

        public void SetKeyBoard()
        {
            base.MessageReplyMarkup = new InlineKeyboardMarkup(
                        new[]
                        {
                        new[]
                        {
                            OneBtn,TwoBtn,ThreeBtn,FourBtn,FiveBtn
                        }
            });
        }
    }
}
