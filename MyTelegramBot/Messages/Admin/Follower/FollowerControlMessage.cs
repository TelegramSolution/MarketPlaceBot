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
    /// сообщение с данными пользователя и кнопками блок/разблок 
    /// </summary>
    public class FollowerControlMessage:BotMessage
    {
        int FollowerId { get; set; }

        private InlineKeyboardCallbackButton BlockBtn { get; set; }

        /// <summary>
        /// посмотреть заказы пользователя через Inline
        /// </summary>
        private InlineKeyboardButton ViewOrderBtn { get; set; }

        private InlineKeyboardButton SearchFollowerBtn { get; set; }

        /// <summary>
        /// посмотреть адреса пользователя через Inline
        /// </summary>
        private InlineKeyboardButton ViewAddressBtn { get; set; }

        private InlineKeyboardCallbackButton UnblockBtn { get; set; }

        private Follower Follower { get; set; }

        public FollowerControlMessage (int FollowerId)
        {
            this.FollowerId = FollowerId;
        }

        public FollowerControlMessage(Follower follower)
        {
            Follower = follower;
        }

        public override BotMessage BuildMsg()
        {
            if(Follower==null)
                Follower = FollowerFunction.GetFollower(FollowerId);

            if (Follower != null)
            {
                string status = "";

                string IsOperator = "";

                if (Follower.Blocked)
                {
                    status = Italic("Заблокирован");
                }

                if (Follower.Admin != null && Follower.Admin.Count>0)
                {
                    IsOperator = Bold("Роль: ") + "Оператор системы | удалить  /removeoperator"+Follower.Admin.LastOrDefault().Id.ToString();
                }

                base.TextMessage = Bold("Имя: ") + Follower.FirstName + NewLine() +
                                 Bold("Фамилия: ") + Follower.LastName + NewLine() +
                                 Bold("Профиль:") + HrefUrl("https://t.me/" + Follower.UserName, Follower.UserName) + NewLine() +
                                 Bold("Дата регистрации:") +Follower.DateAdd.ToString()+NewLine()+
                                 Bold("Телефон:") + Follower.Telephone + NewLine() +IsOperator+ NewLine()+status;


                base.MessageReplyMarkup = SetInline();

                return this;

            }

            else
                return null;
        }

        public InlineKeyboardMarkup SetInline()
        {
            if(Follower!=null && Follower.Blocked)
            {
                return new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        BuildInlineBtn("Разблокировать:", BuildCallData(AdminBot.UnBlockFollowerCmd, AdminBot.ModuleName, Follower.Id))
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Адреса", InlineFind.FollowerAddress + "|"+Follower.Id.ToString()),
                        InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Заказы", InlineFind.FolowerOrder + "|"+Follower.Id.ToString())
                    },
                    new[]
                    {
                        BackToAdminPanelBtn()
            }
                });
            }

            else
                return new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        BuildInlineBtn("Заблокировать:", BuildCallData(AdminBot.BlockFollowerCmd, AdminBot.ModuleName, Follower.Id))
                        ,BuildInlineBtn("Дать права оператора",BuildCallData(OperatorBot.AddOperatorRulesCmd,OperatorBot.ModuleName,Follower.Id))
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Адреса", InlineFind.FollowerAddress + "|"+Follower.Id.ToString()),
                        InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Заказы", InlineFind.FolowerOrder + "|"+Follower.Id.ToString())
                    },
                    new[]
                    {
                        BackToAdminPanelBtn()
                    }
                });


        }

    }
}
