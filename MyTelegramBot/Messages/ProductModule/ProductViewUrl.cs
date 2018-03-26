using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;
using MyTelegramBot.Bot.AdminModule;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages
{
    public class ProductViewUrl:BotMessage
    {


        private Product Product { get; set; }

        private InlineKeyboardCallbackButton BackToProductBtn { get; set; }

        public ProductViewUrl(Product product)
        {
            Product = product;
        }

        public override BotMessage BuildMsg()
        {
            if (Product != null)
            {
                base.TextMessage = Product.TelegraphUrl;

                BackToProductBtn = BuildInlineBtn("Назад к товару", BuildCallData(ProductBot.GetProductCmd, ProductBot.ModuleName, Product.Id));

                base.MessageReplyMarkup = new InlineKeyboardMarkup(new InlineKeyboardButton[] { BackToProductBtn });

                return this;

            }

            else
                return null;
        }
    }
}
