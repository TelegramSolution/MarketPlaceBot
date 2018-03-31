using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;
using System.Web;
using Telegram.Bot.Types.InlineKeyboardButtons;
using MyTelegramBot.Bot.AdminModule;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// сообещние с просьбой указать свой username в настройках telegram
    /// </summary>
    public class UserNameImageMessage:BotMessage
    {
        private InlineKeyboardCallbackButton NextBtn { get; set; }

        Configuration Configuration { get; set; }

        public UserNameImageMessage(Configuration configuration)
        {
            Configuration = configuration;
        }

        public override BotMessage BuildMsg()
        {
            NextBtn = new InlineKeyboardCallbackButton("Далее", BuildCallData("VerifyUserName",Bot.OrderBot.ModuleName));

            // файл еще не разу не отправлялся. Считываем его из папки 
            if (Configuration!=null && Configuration.UserNameFaqFileId==null || Configuration==null)
            base.MediaFile = new MediaFile
            {
                Caption = "Для того что бы мы могли связаться с вами, в настройках Телеграм укажите свой ник-нейм. Потом нажмите далее." + NewLine() +
                "См. картинку.",
                FileTo = new FileToSend { Content = Bot.GeneralFunction.ReadFile("UserNameFaq.png"), Filename = "UserNameFaq.png" },

            };

            else // Отрпавляем только Id файла на сервер телеграм
                base.MediaFile = new MediaFile
                {
                    Caption = "Для того что бы мы могли связаться с вами, в настройках Телеграм укажите свой ник-нейм. Потом нажмите далее." + NewLine() +
                    "См. картинку.",
                    FileTo = new FileToSend { FileId=Configuration.UserNameFaqFileId, Filename = "UserNameFaq.png" },

                };

            base.MessageReplyMarkup = new InlineKeyboardMarkup(new[] {
                new[]
                {
                    NextBtn
                }
            });

            return this;
        }
    }
}
