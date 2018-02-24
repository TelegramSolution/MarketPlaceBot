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

        private InlineKeyboardCallbackButton NoConfirmOrdersBtn { get; set; }

        private InlineKeyboardCallbackButton ContactEditPanelBtn { get; set; }

        private InlineKeyboardCallbackButton PaymentsEnableListBtn { get; set; }

        private InlineKeyboardCallbackButton StockViewBtn { get; set; }
        private MyTelegramBot.Admin Admin { get; set; }

        private int FollowerId { get; set; }

        private int AdminId { get; set; }
        public AdminPanelCmdMessage(int FollowerId)
        {
            this.FollowerId = FollowerId;
        }

        public override BotMessage BuildMsg()
        {


                EditProductBtn = new InlineKeyboardCallbackButton("Изменить товар"+ " \ud83d\udd8a", BuildCallData(ProductEditBot.ProductEditorCmd, ProductEditBot.ModuleName));
                EditCategoryBtn = new InlineKeyboardCallbackButton("Изменить категорию"+ " \ud83d\udd8a", BuildCallData(CategoryEditBot.CategoryEditorCmd, CategoryEditBot.ModuleName));
                ContactEditPanelBtn= new InlineKeyboardCallbackButton("Изменить контактные данные"+ " \ud83d\udd8a", BuildCallData(AdminBot.ContactEditCmd, AdminBot.ModuleName));
                NoConfirmOrdersBtn = new InlineKeyboardCallbackButton("Показать необработанные заказы" + " \ud83d\udcd2", BuildCallData(AdminBot.NoConfirmOrderCmd, AdminBot.ModuleName));
                PaymentsEnableListBtn = new InlineKeyboardCallbackButton("Выбрать доступные методы оплаты" + " \ud83d\udcb0", BuildCallData(AdminBot.PayMethodsListCmd, AdminBot.ModuleName));
                StockViewBtn = new InlineKeyboardCallbackButton("Посмотреть остатки", BuildCallData("ViewStock", AdminBot.ModuleName));
            base.TextMessage = Bold("Панель администратора") + NewLine() +
                               "1) Экспорт всех заказов в CSV файл /export" + NewLine() +
                               "2) Экспорт истории изменения остатков /stockexport" + NewLine() +
                               "3) Добавить новый товар /newprod" + NewLine() +
                               "4) Создать новую категорию /newcategory" + NewLine() +
                               "5) Выбрать доступные способы оплаты /paymethods" + NewLine() +
                               "6) Список операторов / Добавить нового / Удалить /operators" + NewLine() +
                               "7) Список доступных городов /cities" + NewLine()+
                               "8) Бот рассылает уведомления в ЛС. Что бы выключить нажмите /off , что бы включить нажмите /on";

                SetInlineKeyBoard();
                return this;

        }

        private void SetInlineKeyBoard()
        {
            
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                    new[]{
                        new[]
                        {
                            StockViewBtn
                        },
                new[]
                        {
                            EditProductBtn
                        },
                new[]
                        {
                            EditCategoryBtn,
                        },

                new[]
                        {
                            NoConfirmOrdersBtn
                        },
                new[]
                        {
                            ContactEditPanelBtn
                        },

                     });
        }
    }


}
