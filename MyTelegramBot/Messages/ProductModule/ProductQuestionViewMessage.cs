using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.InlineKeyboardButtons;
using System.Security.Cryptography;
using System.Text;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;
using MyTelegramBot.BusinessLayer;

namespace MyTelegramBot.Messages
{
    public class ProductQuestionViewMessage:BotMessage
    {
        private ProductQuestion ProductQuestion { get; set; }

        public ProductQuestionViewMessage(ProductQuestion productQuestion)
        {
            ProductQuestion = productQuestion;
        }

        public override BotMessage BuildMsg()
        {
            base.TextMessage = "Вопрос №" + ProductQuestion.Id  + NewLine() +
                  Bold("Название товара:") + ProductQuestion.Product.Name + " /item" + ProductQuestion.Product.Id.ToString() + NewLine() +
                  Bold("Время:") + ProductQuestion.TimeStamp.ToString() + NewLine() +
                  Bold("Текст вопроса:") + ProductQuestion.Text;

            if (ProductQuestion.Answer != null)
                base.TextMessage += NewLine() + "________________________" + NewLine() +
                                  Bold("Время:") + ProductQuestion.Answer.TimeStamp.ToString() + NewLine() +
                                  Bold("Ответ оператора:") + ProductQuestion.Answer.Text;

            return this;
        }
    }
}
