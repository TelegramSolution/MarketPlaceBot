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
    public class MoreSettingsMessage:BotMessage
    {
        private InlineKeyboardCallbackButton TimeWorkBtn { get; set; }

        private InlineKeyboardCallbackButton MethodsOfObtainingBtn { get; set; }

        private InlineKeyboardCallbackButton PaymentsSettingsBtn { get; set; }

        private InlineKeyboardCallbackButton PaymentsEnableBtn { get; set; }

        private InlineKeyboardCallbackButton AboutEditorBtn { get; set; }

        private InlineKeyboardCallbackButton VkEditorBtn { get; set; }

        private InlineKeyboardCallbackButton InstagramEditorBtn { get; set; }

        private InlineKeyboardCallbackButton ChatEditorBtn { get; set; }

        private InlineKeyboardCallbackButton ChannelEditorBtn { get; set; }

        private InlineKeyboardCallbackButton DeliveryPriceBtn { get; set; }


        public override BotMessage BuildMsg()
        {
            TimeWorkBtn = BuildInlineBtn("Время работы", BuildCallData(MoreSettingsBot.WorkTimeEditorCmd, MoreSettingsBot.ModuleName),base.ClockEmodji);

            MethodsOfObtainingBtn = BuildInlineBtn("Способы получения", BuildCallData(MoreSettingsBot.MethodOfObtaitingCmd, MoreSettingsBot.ModuleName),base.CarEmodji);

            PaymentsEnableBtn = BuildInlineBtn("Доступные способы оплаты", BuildCallData(MoreSettingsBot.EnablePaymentMethodCmd, MoreSettingsBot.ModuleName),base.CreditCardEmodji);

            PaymentsSettingsBtn= BuildInlineBtn("Настройка платежей", BuildCallData(MoreSettingsBot.SettingsPaymentMethodCmd, MoreSettingsBot.ModuleName),base.CreditCardEmodji);

            AboutEditorBtn= BuildInlineBtn("О нас (ред.)", BuildCallData(MoreSettingsBot.AboutEditCmd, MoreSettingsBot.ModuleName),base.NoteBookEmodji);

            VkEditorBtn = BuildInlineBtn("VK.com (ред.)", BuildCallData(MoreSettingsBot.VkEditCmd, MoreSettingsBot.ModuleName),base.MobileEmodji);

            InstagramEditorBtn = BuildInlineBtn("Instagram (ред.)", BuildCallData(MoreSettingsBot.InstagramEditCmd, MoreSettingsBot.ModuleName), base.MobileEmodji);

            ChatEditorBtn = BuildInlineBtn("Чат (ред.)", BuildCallData(MoreSettingsBot.ChatEditCmd, MoreSettingsBot.ModuleName), base.MobileEmodji);

            ChannelEditorBtn = BuildInlineBtn("Канал (ред.)", BuildCallData(MoreSettingsBot.ChannelEditCmd, MoreSettingsBot.ModuleName), base.MobileEmodji);

            DeliveryPriceBtn= BuildInlineBtn("Стоимость доставки", BuildCallData(MoreSettingsBot.ChannelEditCmd, MoreSettingsBot.ModuleName), base.CashEmodji);

            BackBtn = BackToAdminPanelBtn();

            base.TextMessage = "Дополнительные настройки";

            SetKeyboad();


            return this;
           
        }

        private void SetKeyboad()
        {
            base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            TimeWorkBtn,MethodsOfObtainingBtn
                        },
                new[]
                        {
                            DeliveryPriceBtn,AboutEditorBtn
                        },
                new[]
                        {
                            VkEditorBtn,InstagramEditorBtn,
                        },
                new[]
                        {
                            ChannelEditorBtn,ChatEditorBtn
                        },
                new[]
                        {
                            PaymentsEnableBtn,PaymentsSettingsBtn
                        },

                new[]
                        {
                            BackBtn
                        },

                 });
        }
    }
}
