﻿using System;
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

    /// <summary>
    /// Панель администратора
    /// </summary>
    public class ControlPanelMessage:BotMessage
    {
        private InlineKeyboardButton EditProductBtn { get; set; }

        private InlineKeyboardCallbackButton EditCategoryBtn { get; set; }

        private InlineKeyboardCallbackButton StockViewBtn { get; set; }

        private InlineKeyboardCallbackButton ViewFollowerBtn { get; set; }

        private InlineKeyboardButton HelpDesktBtn { get; set; }

        private InlineKeyboardCallbackButton ViewOrdersBtn { get; set; }

        private InlineKeyboardButton ViewPaymentsBtn { get; set; }

        private InlineKeyboardCallbackButton AddProuctBtn { get; set; }

        private InlineKeyboardCallbackButton AddCategoryBtn { get; set; }

        private InlineKeyboardCallbackButton ViewCitiesBtn { get; set; }

        private InlineKeyboardCallbackButton ViewOperatorsBtn { get; set; }

        private InlineKeyboardCallbackButton ViewPickupPointBtn { get; set; }

        private InlineKeyboardCallbackButton MoreSettingsBtn { get; set; }

        /// <summary>
        /// 2-я стр. с кнопками
        /// </summary>
        private InlineKeyboardCallbackButton ControlPanelPage2Btn { get; set; }

        private InlineKeyboardCallbackButton NotificationBtn { get; set; }

        private InlineKeyboardCallbackButton ExportBtn { get; set; }

        private MyTelegramBot.Admin Admin { get; set; }

        private int FollowerId { get; set; }

        private int AdminId { get; set; }
        public ControlPanelMessage(int FollowerId=0)
        {
            this.FollowerId = FollowerId;
        }

        public override BotMessage BuildMsg()
        {

                EditProductBtn = InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Редактор"+base.PenEmodji, InlineFind.EditProduct + "|");

                HelpDesktBtn= InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Тех. поддержка" + base.PenEmodji, InlineFind.HelpdDesk + "|");

                EditCategoryBtn = new InlineKeyboardCallbackButton("Изм. категорию"+ " \ud83d\udd8a", BuildCallData(CategoryEditBot.CategoryEditorCmd, CategoryEditBot.ModuleName));

                StockViewBtn = BuildInlineBtn("Остатки", BuildCallData("ViewStock", AdminBot.ModuleName),base.Depth2Emodji);
                
                ViewFollowerBtn = BuildInlineBtn("Пользователи", BuildCallData(AdminBot.ViewFollowerListCmd, AdminBot.ModuleName), base.ManEmodji2);

                ViewOrdersBtn = BuildInlineBtn("Заказы", BuildCallData(AdminBot.ViewOrdersListCmd, AdminBot.ModuleName), base.PackageEmodji);

                ViewPaymentsBtn = InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Платежи" + base.CreditCardEmodji, InlineFind.Payment + "|");

                MoreSettingsBtn = BuildInlineBtn("Доп. настройки", BuildCallData(MoreSettingsBot.MoreSettingsCmd, MoreSettingsBot.ModuleName), base.CogwheelEmodji);

                ControlPanelPage2Btn = BuildInlineBtn(base.Next2Emodji, BuildCallData(AdminBot.AdminPage2Cmd, AdminBot.ModuleName));

                ExportBtn = BuildInlineBtn("Экспорт данных в .xlsx", BuildCallData(AdminBot.ExportViewerCmd, AdminBot.ModuleName), base.NoteBookEmodji);

            base.TextMessage = Bold("Панель администратора") + NewLine() +
                               "1) Добавить новый товар /addprod" + NewLine() +
                               "2) Создать новую категорию /newcategory" + NewLine() +
                               "3) Бот рассылает уведомления в ЛС. Что бы выключить нажмите /off , что бы включить нажмите /on" +NewLine()+
                               "4) Документация /doc ";

                SetInlineKeyBoard();
                return this;

        }

        private void SetInlineKeyBoard()
        {
            
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                    new[]{
                new[]
                        {
                            ExportBtn
                        },
                new[]
                        {
                            EditProductBtn,EditCategoryBtn
                        },
                new[]
                        {
                            ViewFollowerBtn,StockViewBtn
                        },
                new[]
                        {
                            HelpDesktBtn,ViewOrdersBtn
                        },
                new[]
                        {
                            ViewPaymentsBtn
                        },
                new[]
                        {
                            MoreSettingsBtn,ControlPanelPage2Btn
                        },

                     });
        }

        public InlineKeyboardMarkup Page2Btn()
        {

            ViewCitiesBtn = BuildInlineBtn("Города", BuildCallData(AdminBot.ViewCitiesCmd, AdminBot.ModuleName), base.Build2Emodji);

            ViewOperatorsBtn = BuildInlineBtn("Операторы", BuildCallData(OperatorBot.ViewOperatosCmd, OperatorBot.ModuleName), base.ManAndComputerEmodji);

            ViewPickupPointBtn = BuildInlineBtn("Пункты самовывоза", BuildCallData(AdminBot.ViewPickupPointCmd, AdminBot.ModuleName), base.Build2Emodji);

            BackBtn = BuildInlineBtn(base.Previuos2Emodji, BuildCallData(AdminBot.BackToAdminPanelCmd, AdminBot.ModuleName));


            NotificationBtn = BuildInlineBtn("Рассылки", BuildCallData(NotificationBot.NotificationViewCmd, NotificationBot.ModuleName),base.SenderEmodji);

            return new InlineKeyboardMarkup(
            new[]{
                new[]
                        {
                            ViewCitiesBtn,ViewPickupPointBtn
                        },
                new[]
                        {
                            ViewOperatorsBtn
                        },
                new[]
                        {
                            BackBtn,NotificationBtn
                        },
                

            });

        }
    }


}
