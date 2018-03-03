using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.InlineQueryResults;
using MyTelegramBot.Bot;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using System.IO;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// сообщение с отзывом для товара
    /// </summary>
    public class ViewProductFeedBackMessage:BotMessage
    {

        private InlineKeyboardCallbackButton NextFeedBackBtn { get; set; }

        private InlineKeyboardCallbackButton PreviusFeedBackBtn { get; set; }

        Product Product { get; set; }

        FeedBack FeedBack { get; set; }

        private int ProductId { get; set; }

        private int FeedBackId { get; set; }

        private MarketBotDbContext db { get; set; }

        public ViewProductFeedBackMessage (int ProductId, int FeedBackId = 0)
        {
            this.ProductId = ProductId;
            this.FeedBackId = FeedBackId;
            BackBtn = BuildInlineBtn("Назад к товару", BuildCallData(ProductBot.GetProductCmd, ProductBot.ModuleName, ProductId));
        }


        public override BotMessage BuildMsg()
        {
            db = new MarketBotDbContext();

            Product = db.Product.Find(this.ProductId);

            var list = db.FeedBack.Where(f => f.ProductId == ProductId && f.Enable).ToList();

            if (FeedBackId == 0)
                FeedBack = list.OrderBy(f => f.Id).FirstOrDefault();

            if(FeedBackId>0)
                FeedBack= list.Find(f => f.Id==FeedBackId);

            if (FeedBack != null)
            {
                //порядковый номер отзыва 
                int index = list.FindIndex(x => x == FeedBack) + 1;

                //общее кол-во отзывов по товару
                int count = list.Count;

                base.TextMessage = BlueRhombus + " Отзыв к товару : " + Product.Name + " ( " + index.ToString() + " из " + count.ToString() + " )" + NewLine() +
                    Bold("Время:") + FeedBack.DateAdd.ToString() + NewLine() +
                    Bold("Комментарий:") + FeedBack.Text + NewLine() +
                    Bold("Оценка:") + ConcatEmodjiStar(Convert.ToInt32(FeedBack.RaitingValue));

                SetButtons(FeedBack.Id);
                db.Dispose();
                return this;
            }



            else
            {
                db.Dispose();
                return null;
            }

        }

        private void SetButtons(int FeedBackId)
        {
            int next, previous;

            next = NextFeedbackId(FeedBackId);
            previous = PreviousFeedbackId(FeedBackId);

            if(next > 0 && previous > 0)
            {
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            BackBtn
                        },
                new[]
                        {
                            PreviusFeedBackBtn=BuildInlineBtn(base.PreviuosEmodji,BuildCallData(ProductBot.CmdViewFeedBack,ProductBot.ModuleName,ProductId,previous)),
                            NextFeedBackBtn=BuildInlineBtn(base.NextEmodji,BuildCallData(ProductBot.CmdViewFeedBack,ProductBot.ModuleName,ProductId,next))
                        }


                });
            }

            if(next < 0 && previous < 0)
            {
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            BackBtn
                        },

                });
            }
        }

        private string ConcatEmodjiStar (int count)
        {
            string res = "";
            for (int i = 0; i < count; i++)
                res += base.StartEmodji;

            return res;
        }

        private int NextFeedbackId (int CurrentFeedBackId)
        {
            var feedback = db.FeedBack.Where(f => f.Id > CurrentFeedBackId && f.ProductId == ProductId && f.Enable).FirstOrDefault();

            if (feedback != null)
                return feedback.Id;

            else
            {
               feedback= db.FeedBack.Where(f => f.Id != CurrentFeedBackId && f.ProductId == ProductId && f.Enable).OrderBy(f=>f.Id).FirstOrDefault();

                if (feedback != null)
                    return feedback.Id;

                else
                    return -1;
            }
        }

        private int PreviousFeedbackId(int CurrentFeedBackId)
        {
            var feedback = db.FeedBack.Where(f => f.Id < CurrentFeedBackId && f.ProductId == ProductId && f.Enable).OrderByDescending(f=>f.Id).FirstOrDefault();

            if (feedback != null)
                return feedback.Id;

            else
            {
                feedback = db.FeedBack.Where(f => f.Id != CurrentFeedBackId && f.ProductId == ProductId && f.Enable).OrderByDescending(f => f.Id).FirstOrDefault();

                if (feedback != null)
                    return feedback.Id;

                else
                    return -1;
            }
        }
    }
}
