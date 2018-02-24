using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.AdminModule;

namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// Сообщение с админскими функциями для редактирования категории
    /// </summary>
    public class AdminCategoryFuncMessage:BotMessage
    {
        private InlineKeyboardCallbackButton EditCategoryNameBtn { get; set; }

        private InlineKeyboardCallbackButton EditCategoryEnableBtn { get; set; }

        private InlineKeyboardCallbackButton BackToAdminPanelBtn { get; set; }

        private Category Category { get; set; }

        private int CategoryId { get; set; }

        public AdminCategoryFuncMessage(int CategoryId)
        {
            this.CategoryId = CategoryId;
        }

        public AdminCategoryFuncMessage BuildMessage()
        {
            using (MarketBotDbContext db=new MarketBotDbContext())
                Category = db.Category.Where(c => c.Id == CategoryId).FirstOrDefault();
            

            if (Category != null)
            {
                EditCategoryNameBtn = BuildInlineBtn("Изменить название", 
                                                    BuildCallData(Bot.CategoryEditBot.CategoryEditNameCmd, Bot.CategoryEditBot.ModuleName, CategoryId), base.PenEmodji);

                if(Category.Enable)
                    EditCategoryEnableBtn= BuildInlineBtn("Активно",
                                                    BuildCallData(Bot.CategoryEditBot.CategoryEditEnableCmd, Bot.CategoryEditBot.ModuleName, CategoryId), base.CheckEmodji);

                else
                    EditCategoryEnableBtn = BuildInlineBtn("Активно",
                                                    BuildCallData(Bot.CategoryEditBot.CategoryEditEnableCmd, Bot.CategoryEditBot.ModuleName, CategoryId), base.UnCheckEmodji);

                BackToAdminPanelBtn = BuildInlineBtn("Панель Администратора", BuildCallData(AdminBot.BackToAdminPanelCmd, Bot.AdminModule.AdminBot.ModuleName),base.CogwheelEmodji);

                SetInlineKeyBoard();               

                base.TextMessage =base.BlueRhombus+" "+ Category.Name+ base.BlueRhombus+ NewLine() + "выберите действие:";
                
            }

            return this;
        }

        private void SetInlineKeyBoard()
        {
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                    new[]{
                new[]
                        {
                            EditCategoryNameBtn
                        },
                new[]
                        {
                            EditCategoryEnableBtn
                        },

                new[]
                        {
                            BackToAdminPanelBtn
                        },

                     });


        }
    }
}
