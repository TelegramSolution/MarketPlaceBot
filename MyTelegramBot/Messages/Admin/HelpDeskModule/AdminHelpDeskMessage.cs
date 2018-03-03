using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot.AdminModule;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages.Admin
{
    public class AdminHelpDeskMessage : BotMessage
    {
        private InlineKeyboardCallbackButton ViewAttachBtn { get; set; }

        private InlineKeyboardCallbackButton AddCommentBtn { get; set; }

        private InlineKeyboardCallbackButton ClosedBtn { get; set; }

        private InlineKeyboardCallbackButton TakeToWorkBtn { get; set; }

        private InlineKeyboardCallbackButton ViewContactBtn { get; set; }

        private InlineKeyboardCallbackButton FreeHelpBtn { get; set; }

        private HelpDesk HelpDesk { get; set; }

        private int HelpDeskId { get; set; }

        private bool InWork { get; set; }

        /// <summary>
        /// Кому отправляем сообещние
        /// </summary>
        private int FollowerId { get; set; }

        /// <summary>
        /// У кого в обработке
        /// </summary>
        private int FollowerInWork { get; set; }

        public AdminHelpDeskMessage(HelpDesk helpDesk, int FollowerId=0)
        {
            HelpDesk = helpDesk;
            this.FollowerId = FollowerId;
        }

        public AdminHelpDeskMessage(int helpDeskID)
        {
            HelpDeskId = helpDeskID;
        }

        public override BotMessage BuildMsg()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                if (this.HelpDesk == null)
                    this.HelpDesk = db.HelpDesk.Where(h => h.Id == HelpDeskId).Include(h => h.HelpDeskAttachment).
                         Include(h => h.HelpDeskAnswer)
                        .Include(h=>h.HelpDeskInWork)
                        .FirstOrDefault();

                string HelpAnswerText = "";

                if (HelpDesk != null && HelpDesk.Send==true)
                {

                    base.TextMessage = Bold("Номер заявки: ") + HelpDesk.Number.ToString() + NewLine()
                        + Bold("Пользователь: ") +Bot.GeneralFunction.FollowerFullName(db.Follower.Where(f => f.Id == HelpDesk.FollowerId).FirstOrDefault()) + NewLine()
                        + Bold("Дата: ") + HelpDesk.Timestamp.ToString() + NewLine() +
                        Bold("Описание проблемы: ") + HelpDesk.Text + NewLine() + Bold("Прикрепленных файлов: ") + HelpDesk.HelpDeskAttachment.Count.ToString() + NewLine() +
                        "Что бы быстро открыть эту заявку введите команду /ticket" + HelpDesk.Number;

                    string closed = "";

                    if(HelpDesk.HelpDeskAnswer!=null && HelpDesk.HelpDeskAnswer.Count > 0)
                    {
                        int counter = 1;
                        foreach(HelpDeskAnswer answer in HelpDesk.HelpDeskAnswer)
                        {
                            string view_attach = "";

                            HelpAnswerText += counter.ToString() + ")"+Italic("Комментарий:")+" " + answer.Text +" | " +Italic(" Время:") + " "  
                                + answer.Timestamp.ToString() + " | " + Italic(" Пользователь:") + " " +
                                Bot.GeneralFunction.FollowerFullName(db.Follower.Where(f => f.Id == answer.FollowerId).FirstOrDefault()) 
                                 +NewLine()+NewLine();

                            counter++;
                        }

                        if (HelpDesk.Closed == true)
                            closed =Italic( "Заявка закрыта пользоватем " + 
                                Bot.GeneralFunction.FollowerFullName(HelpDesk.HelpDeskAnswer.Where(h => h.Closed == true).FirstOrDefault().FollowerId) + " " +
                              " " + HelpDesk.HelpDeskAnswer.Where(h => h.Closed == true).FirstOrDefault().ClosedTimestamp.ToString());

                        HelpAnswerText += NewLine() + closed;

                    }


                    if(HelpDesk.InWork==true && HelpDesk.HelpDeskInWork.Count > 0) // Узнаем у кого в обработке
                        FollowerInWork =Convert.ToInt32(HelpDesk.HelpDeskInWork.OrderByDescending(h => h.Id).FirstOrDefault().FollowerId);
                    

                    base.TextMessage += NewLine() + NewLine() + HelpAnswerText;
                }

                CreateBtn();
                SetInlineKeyBoard();

            }

            return this;
        }

        private void CreateBtn()
        {
            ViewAttachBtn = new InlineKeyboardCallbackButton("Посмотреть вложения", BuildCallData(HelpDeskProccessingBot.ViewAttachCmd, HelpDeskProccessingBot.ModuleName, HelpDesk.Id));

            AddCommentBtn = new InlineKeyboardCallbackButton("Добавить комментарий", BuildCallData(HelpDeskProccessingBot.AddHelpAnswerCmd, HelpDeskProccessingBot.ModuleName, HelpDesk.Id));

            ClosedBtn = new InlineKeyboardCallbackButton("Закрыть заявку", BuildCallData(HelpDeskProccessingBot.CloseHelpCmd, HelpDeskProccessingBot.ModuleName, HelpDesk.Id));

            TakeToWorkBtn = new InlineKeyboardCallbackButton("Взять в работу", BuildCallData(HelpDeskProccessingBot.TakeHelpCmd, HelpDeskProccessingBot.ModuleName, HelpDesk.Id));

            ViewContactBtn = new InlineKeyboardCallbackButton("Контактные данные", BuildCallData(HelpDeskProccessingBot.ViewContactCmd, HelpDeskProccessingBot.ModuleName, Convert.ToInt32(HelpDesk.FollowerId)));

            FreeHelpBtn = new InlineKeyboardCallbackButton("Освободить", BuildCallData(HelpDeskProccessingBot.FreeHelpCmd, HelpDeskProccessingBot.ModuleName, HelpDesk.Id));


        }

        private void SetInlineKeyBoard()
        {
            //Заявка в работе, но не закрыта и по ней нет комментриев.
            if(HelpDesk.InWork==true && HelpDesk.Closed==false && HelpDesk.HelpDeskAnswer.Count==0)
            base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            ViewContactBtn,ViewAttachBtn
                        },
                new[]
                        {
                            AddCommentBtn
                        },

                new[]
                        {
                            FreeHelpBtn
                        },


                 });

            //Заявка в работе, но не закрыта. По заявке есть комментарии
            if (HelpDesk.InWork==true && HelpDesk.Closed==false && HelpDesk.HelpDeskAnswer.Count >0)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                    new[]{
                new[]
                        {
                            ViewContactBtn,ViewAttachBtn
                        },
                new[]
                        {
                            AddCommentBtn
                        },

                new[]
                        {
                            FreeHelpBtn
                        },

                new[]
                        {
                            ClosedBtn
                        },
                     });

            //Заявка не в работе и не закрыта
            if (HelpDesk.InWork == false && HelpDesk.Closed==false || FollowerId!=FollowerInWork)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            TakeToWorkBtn
                        },



                });

            //ЗАявка в работе и уже выполнена
            if (HelpDesk.InWork == true && HelpDesk.Closed==true || HelpDesk.Closed==true)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            ViewAttachBtn,ViewContactBtn
                        },



                });
        }
    }
}