using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;
using System.Web;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.AdminModule;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages.Admin
{
    public class ProductQuestionAdminViewMessage:BotMessage
    {
        private ProductQuestion ProductQuestion { get; set; }

        private InlineKeyboardCallbackButton AddAnswerBtn { get; set; }

        private InlineKeyboardCallbackButton NextQuestionBtn { get; set; }

        private InlineKeyboardCallbackButton PreviousQuestionBtn { get; set; }

        private InlineKeyboardButton ToBotChatBtn { get; set; }

        private MarketBotDbContext DbContext { get; set; }

        public ProductQuestionAdminViewMessage(ProductQuestion productQuestion)
        {
            ProductQuestion = productQuestion;
        }
        public override BotMessage BuildMsg()
        {
            DbContext = new MarketBotDbContext();

            string bot_name = GeneralFunction.GetBotName();

            ToBotChatBtn = InlineKeyboardButton.WithUrl("Перейти к боту","https://t.me/"+bot_name);

            base.TextMessage = "Вопрос №" + ProductQuestion.Id + " /question" + ProductQuestion.Id.ToString() + NewLine() +
                              Bold("Название товара:") + ProductQuestion.Product.Name + " /item" + ProductQuestion.Product.Id.ToString() + NewLine() +
                              Bold("Пользователь:") + HrefUrl("https://t.me/"+ProductQuestion.Follower.UserName, ProductQuestion.Follower.ToString()) + NewLine() +
                              Bold("Время:") + ProductQuestion.TimeStamp.ToString() + NewLine() +
                              Bold("Текст вопроса:") + ProductQuestion.Text;

            if (ProductQuestion.Answer != null)
                base.TextMessage += NewLine() + "________________________" + NewLine() +
                                  Bold("Пользователь:") + HrefUrl("https://t.me/" + ProductQuestion.Answer.Follower.UserName, ProductQuestion.Answer.Follower.ToString()) + NewLine() +
                                  Bold("Время:") + ProductQuestion.Answer.TimeStamp.ToString() + NewLine() +
                                  Bold("Ответ оператора:") + ProductQuestion.Answer.Text;

            if (ProductQuestion.Answer == null)
                AddAnswerBtn = BuildInlineBtn("Ответить", BuildCallData(AdminBot.AddAnswerCmd, AdminBot.ModuleName, ProductQuestion.Id));

            int nextId = GetNextQuestionId(ProductQuestion.Id);
            int previousId = GetPreviousQuestionId(ProductQuestion.Id);

            if(nextId> 0 && previousId > 0)
            {
                NextQuestionBtn = BuildInlineBtn(base.Next2Emodji, BuildCallData(AdminBot.GetQuestionCmd, AdminBot.ModuleName, nextId));
                PreviousQuestionBtn = BuildInlineBtn(base.Previuos2Emodji, BuildCallData(AdminBot.GetQuestionCmd, AdminBot.ModuleName, previousId));
            }

            base.MessageReplyMarkup = SetKeyboard();

            return this;
        }

        private InlineKeyboardMarkup SetKeyboard()
        {
            if(AddAnswerBtn!=null && NextQuestionBtn!=null && PreviousQuestionBtn!=null)
                return new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            ToBotChatBtn
                        },
                new[]   {
                            AddAnswerBtn
                        },
                new[]
                        {
                            PreviousQuestionBtn, NextQuestionBtn
                        },

               });

            if (AddAnswerBtn != null && NextQuestionBtn == null && PreviousQuestionBtn == null)
                return new InlineKeyboardMarkup(
                    new[]
                    {
                        new[]
                        {
                            ToBotChatBtn
                        },
                        new[]
                        {
                            AddAnswerBtn
                        }
                    });

            if (AddAnswerBtn == null && NextQuestionBtn != null && PreviousQuestionBtn != null)
                return new InlineKeyboardMarkup(
                new[]
                {
                    new[]
                    {
                        PreviousQuestionBtn, NextQuestionBtn
                    }
                });

            else
                return new InlineKeyboardMarkup(
                new[]
                {
                        new[]
                        {
                           ToBotChatBtn
                        }
                });
        }

        private int GetNextQuestionId(int CurrentQuestionId)
        {
           
           var question=DbContext.ProductQuestion.Where(q => q.Id > CurrentQuestionId).FirstOrDefault();

            if (question != null)
                return question.Id;

            question = DbContext.ProductQuestion.FirstOrDefault();

            if (question != null)
                return question.Id;

            else
                return -1;
        }

        private int GetPreviousQuestionId(int CurrentQuestionId)
        {
            var question = DbContext.ProductQuestion.Where(q => q.Id < CurrentQuestionId).LastOrDefault();

            if (question != null)
                return question.Id;

            question = DbContext.ProductQuestion.LastOrDefault();

            if (question != null)
                return question.Id;

            else
                return -1;
        }
    }
}
