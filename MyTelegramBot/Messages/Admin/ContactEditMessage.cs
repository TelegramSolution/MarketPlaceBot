using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.AdminModule;

namespace MyTelegramBot.Messages.Admin
{
    public class ContactEditMessage:Bot.BotMessage
    {
        private InlineKeyboardCallbackButton VkEditBtn { get; set; }

        private InlineKeyboardCallbackButton InstagramEditBtn { get; set; }

        private InlineKeyboardCallbackButton ChatEditBtn { get; set; }

        private InlineKeyboardCallbackButton ChannelEditBtn { get; set; }

        public override BotMessage BuildMsg()
        {
            base.TextMessage = "Выберите действие";
            VkEditBtn = new InlineKeyboardCallbackButton("VK.COM", BuildCallData("VkEdit", AdminBot.ModuleName));
            InstagramEditBtn = new InlineKeyboardCallbackButton("Instagram", BuildCallData("InstagramEdit", AdminBot.ModuleName));
            ChatEditBtn = new InlineKeyboardCallbackButton("Чат в телеграм", BuildCallData("ChatEdit", AdminBot.ModuleName));
            ChannelEditBtn = new InlineKeyboardCallbackButton("Канал в телеграм", BuildCallData("ChannelEdit", AdminBot.ModuleName));
            BackBtn = new InlineKeyboardCallbackButton("Назад", BuildCallData("BackToAdminPanel", AdminBot.ModuleName));
            SetInlineKeyBoard();
            return this;
        }

        private void SetInlineKeyBoard()
        {

            base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            VkEditBtn
                        },
                new[]
                        {
                            InstagramEditBtn
                        },
                new[]
                        {
                            ChatEditBtn
                        },
                new[]
                        {
                            ChannelEditBtn
                        },
                new[]
                        {
                            BackBtn
                        }

                 });
        }
    }
}
