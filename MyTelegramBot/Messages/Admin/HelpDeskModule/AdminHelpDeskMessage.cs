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
using MyTelegramBot.BusinessLayer;

namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// Сообещние с заявкой
    /// </summary>
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
        private HelpDeskInWork InWorkNow { get; set; }


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

            if (this.HelpDesk == null)
                this.HelpDesk = HelpDeskFunction.GetHelpDesk(HelpDeskId);

            string HelpAnswerText = "";

            if (HelpDesk != null && HelpDesk.Send==true)
            {

                base.TextMessage = Bold("Номер заявки: ") + HelpDesk.Number.ToString() + NewLine()
                    + Bold("Пользователь: ") +Bot.GeneralFunction.FollowerFullName(HelpDesk.Follower) + NewLine()
                    + Bold("Дата: ") + HelpDesk.Timestamp.ToString() + NewLine() 
                    + Bold("Описание проблемы: ") + HelpDesk.Text + NewLine() + Bold("Прикрепленных файлов: ") + HelpDesk.HelpDeskAttachment.Count.ToString() + NewLine() +
                    "Что бы быстро открыть эту заявку введите команду /ticket" + HelpDesk.Number;

                if(HelpDesk.HelpDeskAnswer!=null && HelpDesk.HelpDeskAnswer.Count > 0)
                {
                    int counter = 1;
                    foreach(HelpDeskAnswer answer in HelpDesk.HelpDeskAnswer)
                    {

                        HelpAnswerText += counter.ToString() + ") Комментарий:"+" " + answer.Text +" | " 
                                            +" Время:" +" " + answer.Timestamp.ToString() + " | " + " Пользователь:" + " "
                                            +Bot.GeneralFunction.FollowerFullName(answer.FollowerId) 
                                            +NewLine()+NewLine();

                        counter++;
                    }


                }

                // Узнаем у кого в обработке
                InWorkNow =HelpDesk.HelpDeskInWork.LastOrDefault();

                base.TextMessage += NewLine() + NewLine() + HelpAnswerText;

                CreateBtn();
                base.MessageReplyMarkup = SetInlineKeyBoard();
            }           

            return this;
        }

        private void CreateBtn()
        {
            ViewAttachBtn = BuildInlineBtn("Посмотреть вложения", BuildCallData(HelpDeskProccessingBot.ViewAttachCmd, HelpDeskProccessingBot.ModuleName, HelpDesk.Id),base.PictureEmodji);

            AddCommentBtn = BuildInlineBtn("Добавить комментарий", BuildCallData(HelpDeskProccessingBot.AddHelpAnswerCmd, HelpDeskProccessingBot.ModuleName, HelpDesk.Id,Convert.ToInt32(HelpDesk.Number)),NoteBookEmodji);

            ClosedBtn = BuildInlineBtn("Закрыть заявку", BuildCallData(HelpDeskProccessingBot.CloseHelpCmd, HelpDeskProccessingBot.ModuleName, HelpDesk.Id),base.DoneEmodji);

            TakeToWorkBtn = BuildInlineBtn("Взять в работу", BuildCallData(HelpDeskProccessingBot.TakeToWorkCmd, HelpDeskProccessingBot.ModuleName, HelpDesk.Id),base.Next2Emodji);

            ViewContactBtn = BuildInlineBtn("Контактные данные", BuildCallData(HelpDeskProccessingBot.ViewContactCmd, HelpDeskProccessingBot.ModuleName, Convert.ToInt32(HelpDesk.FollowerId)),MobileEmodji);

            FreeHelpBtn = BuildInlineBtn("Освободить", BuildCallData(HelpDeskProccessingBot.FreeHelpCmd, HelpDeskProccessingBot.ModuleName, HelpDesk.Id),base.Previuos2Emodji);


        }

        private InlineKeyboardMarkup SetInlineKeyBoard()
        {
            //Заявка в работе, но не закрыта и по ней нет комментриев.
            if(InWorkNow != null && InWorkNow.InWork == true && HelpDesk.Closed==false && HelpDesk.HelpDeskAnswer.Count==0)
            return new InlineKeyboardMarkup(
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
            if (InWorkNow!=null && InWorkNow.InWork==true && HelpDesk.Closed==false && HelpDesk.HelpDeskAnswer.Count >0)
                return new InlineKeyboardMarkup(
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
                            ClosedBtn
                        },

                new[]
                        {
                            FreeHelpBtn
                        },
                     });

            //Заявка не зазкрты или Заявка в работе у кого то другого
            if (HelpDesk.Closed==false && InWorkNow!=null  && InWorkNow.FollowerId!=FollowerId || InWorkNow!=null && InWorkNow.InWork==false)
                return new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            TakeToWorkBtn
                        },



                });

            //ЗАявка в работе и уже выполнена
            if (HelpDesk.Closed==true)
                return new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            ViewAttachBtn,ViewContactBtn
                        },


                });

            else
                return new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            TakeToWorkBtn
                        },



        });

        }
    }
}