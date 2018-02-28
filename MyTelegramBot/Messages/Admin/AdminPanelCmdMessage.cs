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

    /// <summary>
    /// Панель администратора
    /// </summary>
    public class AdminPanelCmdMessage:Bot.BotMessage
    {
        private InlineKeyboardCallbackButton EditProductBtn { get; set; }

        private InlineKeyboardCallbackButton EditCategoryBtn { get; set; }

        private InlineKeyboardCallbackButton StockViewBtn { get; set; }

        private InlineKeyboardCallbackButton ViewFollowerBtn { get; set; }


        private InlineKeyboardCallbackButton ViewOrdersBtn { get; set; }

        private InlineKeyboardCallbackButton ViewPaymentsBtn { get; set; }

        private InlineKeyboardCallbackButton AddProuctBtn { get; set; }

        private InlineKeyboardCallbackButton AddCategoryBtn { get; set; }

        private InlineKeyboardCallbackButton CitiesBtn { get; set; }

        private InlineKeyboardCallbackButton OperatorsBtn { get; set; }

        private MyTelegramBot.Admin Admin { get; set; }

        private int FollowerId { get; set; }

        private int AdminId { get; set; }
        public AdminPanelCmdMessage(int FollowerId)
        {
            this.FollowerId = FollowerId;
        }

        public override BotMessage BuildMsg()
        {


                EditProductBtn = new InlineKeyboardCallbackButton("Изм. товар"+ " \ud83d\udd8a", BuildCallData(ProductEditBot.ProductEditorCmd, ProductEditBot.ModuleName));
                EditCategoryBtn = new InlineKeyboardCallbackButton("Изм. категорию"+ " \ud83d\udd8a", BuildCallData(CategoryEditBot.CategoryEditorCmd, CategoryEditBot.ModuleName));
                //ContactEditPanelBtn= new InlineKeyboardCallbackButton("Изменить контактные данные"+ " \ud83d\udd8a", BuildCallData(AdminBot.ContactEditCmd, AdminBot.ModuleName));
                //NoConfirmOrdersBtn = new InlineKeyboardCallbackButton("Показать необработанные заказы" + " \ud83d\udcd2", BuildCallData(AdminBot.NoConfirmOrderCmd, AdminBot.ModuleName));
                //PaymentsEnableListBtn = new InlineKeyboardCallbackButton("Выбрать доступные методы оплаты" + " \ud83d\udcb0", BuildCallData(AdminBot.PayMethodsListCmd, AdminBot.ModuleName));
                StockViewBtn = BuildInlineBtn("Остатки", BuildCallData("ViewStock", AdminBot.ModuleName),base.Depth2Emodji);
                

            ViewFollowerBtn = BuildInlineBtn("Пользователи", BuildCallData(AdminBot.ViewFollowerListCmd, AdminBot.ModuleName), base.ManEmodji2);

            ViewOrdersBtn = BuildInlineBtn("Заказы", BuildCallData(AdminBot.ViewOrdersListCmd, AdminBot.ModuleName), base.PackageEmodji);

            ViewPaymentsBtn = BuildInlineBtn("Платежи", BuildCallData(AdminBot.ViewPaymentsListCmd, AdminBot.ModuleName), base.CreditCardEmodji);

            CitiesBtn = BuildInlineBtn("Города", BuildCallData(AdminBot.ViewCitiesCmd, AdminBot.ModuleName), base.Build2Emodji);

            OperatorsBtn = BuildInlineBtn("Операторы", BuildCallData(AdminBot.ViewOperatosCmd, AdminBot.ModuleName), base.ManAndComputerEmodji);


            base.TextMessage = Bold("Панель администратора") + NewLine() +
                               "1) Добавить новый товар /newprod" + NewLine() +
                               "2) Создать новую категорию /newcategory" + NewLine() +
                               "3) Бот рассылает уведомления в ЛС. Что бы выключить нажмите /off , что бы включить нажмите /on";

                SetInlineKeyBoard();
                return this;

        }

        private void SetInlineKeyBoard()
        {
            
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                    new[]{
                new[]
                        {
                            EditProductBtn,EditCategoryBtn
                        },
                new[]
                        {
                            ViewFollowerBtn,OperatorsBtn,CitiesBtn
                        },

                new[]
                        {
                            ViewPaymentsBtn,ViewOrdersBtn
                        },
                new[]
                        {
                            StockViewBtn
                        },

                     });
        }
    }


}
