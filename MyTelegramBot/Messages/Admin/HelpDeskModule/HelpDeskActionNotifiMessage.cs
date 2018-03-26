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

namespace MyTelegramBot.Messages.Admin
{
    public class HelpDeskActionNotifiMessage:BotMessage
    {
        private HelpDeskAnswer HelpDeskAnswer { get; set; }

        private HelpDeskInWork HelpDeskInWork { get; set; }

        private HelpDesk HelpDesk { get; set; }

        private InlineKeyboardCallbackButton OpenHelpDeskBtn { get; set; }

        private InlineKeyboardButton ToBotDialogBtn { get; set; }

        private InlineKeyboardButton ToUserDialogBtn { get; set; }

        public HelpDeskActionNotifiMessage(HelpDesk helpDesk, HelpDeskAnswer  deskAnswer)
        {
            this.HelpDesk = helpDesk;
            this.HelpDeskAnswer = deskAnswer;

        }

        public HelpDeskActionNotifiMessage(HelpDesk helpDesk, HelpDeskInWork deskInWork)
        {
            this.HelpDesk = helpDesk;
            this.HelpDeskInWork = deskInWork;
        }

        public override BotMessage BuildMsg()
        {
            if(HelpDesk!=null && HelpDeskAnswer!=null && HelpDeskAnswer.Follower != null)
            {
                base.TextMessage = "Пользователь " + HelpDeskAnswer.Follower.FirstName + HelpDeskAnswer.Follower.LastName
                    + " добавил комментарий к Заявке №" + HelpDesk.Number.ToString() + NewLine() +
                    "Комментарий:" + HelpDeskAnswer.Text;

                this.ToBotDialogBtn = InlineKeyboardButton.WithUrl("Перейти к боту", "https://t.me/" + GeneralFunction.GetBotName());

                this.ToUserDialogBtn = InlineKeyboardButton.WithUrl(HelpDeskAnswer.Follower.FirstName + " " + HelpDeskAnswer.Follower.LastName, "https://t.me/" + HelpDeskAnswer.Follower.UserName);

                this.OpenHelpDeskBtn = BuildInlineBtn("Открыть заявку", BuildCallData(HelpDeskProccessingBot.GetHelpDeskCmd, HelpDeskProccessingBot.ModuleName, this.HelpDesk.Id));

                base.MessageReplyMarkup = SetKeyboard();

                return this;
            }

            if (HelpDesk != null && HelpDeskInWork != null && HelpDeskInWork.Follower != null)
            {
                base.TextMessage = "Пользователь " + HelpDeskInWork.Follower.FirstName + HelpDeskInWork.Follower.LastName
                + " взял в работу заявку №" + HelpDesk.Number.ToString();

                this.ToBotDialogBtn = InlineKeyboardButton.WithUrl("Перейти к боту", "https://t.me/" + GeneralFunction.GetBotName());

                this.ToUserDialogBtn = InlineKeyboardButton.WithUrl(HelpDeskInWork.Follower.FirstName + " " + HelpDeskInWork.Follower.LastName, "https://t.me/" + HelpDeskInWork.Follower.UserName);

                this.OpenHelpDeskBtn = BuildInlineBtn("Открыть заявку", BuildCallData(HelpDeskProccessingBot.GetHelpDeskCmd, HelpDeskProccessingBot.ModuleName, this.HelpDesk.Id));

                base.MessageReplyMarkup = SetKeyboard();

                return this;
            }

            else
                return null;
            
        }

        private InlineKeyboardMarkup SetKeyboard()
        {
            return new InlineKeyboardMarkup(
                     new[]{
                    new[]
                    {
                        OpenHelpDeskBtn
                    },
                    new[]
                    {
                        ToBotDialogBtn
                    },
                    new[]
                    {
                        ToUserDialogBtn
                    }

                      });
        }
    }
}
