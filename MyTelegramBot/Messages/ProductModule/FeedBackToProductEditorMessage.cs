using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.InlineKeyboardButtons;
using System.Security.Cryptography;
using System.Text;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages
{
    public class FeedBackToProductEditorMessage:BotMessage
    {
        private InlineKeyboardCallbackButton AddCommentBtn { get; set; }

        private InlineKeyboardCallbackButton [][] Buttons { get; set; }

        private InlineKeyboardCallbackButton SaveBtn { get; set; }

        private InlineKeyboardCallbackButton [] RaitingBtns { get; set; }

        private Product Product { get; set; }

        private FeedBack FeedBack { get; set; }

        private Orders Order { get; set; }

        private int ProductId { get; set; }

        private int FeedBackId { get; set; }

        private int OrderId { get; set; }

        private MarketBotDbContext db;

        public FeedBackToProductEditorMessage(int OrderId,int ProductId)
        {
            this.ProductId = ProductId;
            this.OrderId = OrderId;
        }

        public FeedBackToProductEditorMessage (int FeedBackId)
        {
            this.FeedBackId = FeedBackId;
        }

        public FeedBackToProductEditorMessage( FeedBack feedBack)
        {
            this.FeedBack = feedBack;
        }



        public override BotMessage BuildMsg()
        {
            
            db = new MarketBotDbContext();

            if (FeedBackId > 0) // отзыв уже сохранен в базе
            {
                FeedBack = db.FeedBack.Where(f => f.Id == FeedBackId).Include(f => f.Product).FirstOrDefault();
                Product = FeedBack.Product;
                BackBtn = BuildInlineBtn("Назад", BuildCallData(Bot.OrderBot.CmdBackFeedBackView, Bot.OrderBot.ModuleName,Convert.ToInt32(FeedBack.OrderId),FeedBackId));
                SaveBtn = BuildInlineBtn("Сохранить", BuildCallData(Bot.OrderBot.CmdSaveFeedBack, Bot.OrderBot.ModuleName, Convert.ToInt32(FeedBack.OrderId), FeedBackId),base.DoneEmodji);
                AddCommentBtn = BuildInlineBtn("Добавить комментарий", BuildCallData(Bot.OrderBot.CmdAddCommentFeedBack, Bot.OrderBot.ModuleName, Convert.ToInt32(FeedBack.OrderId), FeedBackId),base.PenEmodji);
                base.TextMessage =base.BlueRhombus+ "Название товара:" + Product.Name + NewLine() +
                    Bold("Оценка:") + FeedBack.RaitingValue + NewLine() +
                    Bold("Время:") + FeedBack.DateAdd.ToString() + NewLine() +
                    Bold("Комментарий:") + FeedBack.Text + NewLine();

                base.MessageReplyMarkup = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(
                new[]{
                                 new[]
                                    {
                                            AddCommentBtn,SaveBtn
                                    },
                                new[]
                                    {
                                            BackBtn
                                    },

                });
            }

            if(FeedBackId==0 && OrderId>0 && ProductId > 0) //отзыва еще нет
            {
                Product = db.Product.Find(ProductId);
                BackBtn = BuildInlineBtn("Назад", BuildCallData(Bot.OrderBot.CmdBackFeedBackView, Bot.OrderBot.ModuleName, OrderId));
                RaitingBtns = new InlineKeyboardCallbackButton[5];

                RaitingBtns[0] = BuildInlineBtn("1", BuildCallData(Bot.OrderBot.CmdFeedBackRaiting, Bot.OrderBot.ModuleName, OrderId, ProductId, 1), base.StartEmodji);
                RaitingBtns[1] = BuildInlineBtn("2", BuildCallData(Bot.OrderBot.CmdFeedBackRaiting, Bot.OrderBot.ModuleName, OrderId, ProductId, 2), base.StartEmodji);
                RaitingBtns[2] = BuildInlineBtn("3", BuildCallData(Bot.OrderBot.CmdFeedBackRaiting, Bot.OrderBot.ModuleName, OrderId, ProductId, 3), base.StartEmodji);
                RaitingBtns[3] = BuildInlineBtn("4", BuildCallData(Bot.OrderBot.CmdFeedBackRaiting, Bot.OrderBot.ModuleName, OrderId,ProductId, 4), base.StartEmodji);
                RaitingBtns[4] = BuildInlineBtn("5", BuildCallData(Bot.OrderBot.CmdFeedBackRaiting, Bot.OrderBot.ModuleName, OrderId,ProductId, 5), base.StartEmodji);

                base.TextMessage = base.BlueRhombus+ "Название товара:" + Product.Name + NewLine() +NewLine()
                    + Italic("Поставьте оценку от 1 до 5");
                    
                base.MessageReplyMarkup = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(
                new[]{
                                RaitingBtns,
                                new[]
                                    {
                                            BackBtn
                                    },
                                
                });
            }

            return this;
        }
    }
}
