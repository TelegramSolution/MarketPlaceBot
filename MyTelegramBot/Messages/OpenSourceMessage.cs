using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using MyTelegramBot.Bot;

namespace MyTelegramBot.Messages
{
    public class OpenSourceMessage:BotMessage
    {

        public override BotMessage BuildMsg()
        {

            this.TextMessage = "Это проект с открытым исходым. Вы можете скачать его " +HrefUrl("https://github.com/", "здесь");

            BackBtn = new Telegram.Bot.Types.InlineKeyboardButtons.InlineKeyboardCallbackButton("Назад", BuildCallData("MainMenu", Bot.MainMenuBot.ModuleName));

            base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            BackBtn
                        },
                });

            return this;


        }
    }
}
