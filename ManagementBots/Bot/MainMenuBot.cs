using System;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ManagementBots.Bot.Core;

namespace ManagementBots.Bot
{
    public class MainMenuBot: BotCore
    {

        public const string ModuleName = "Main";

        public const string ToMainMenuCmd = "MainMenu";

       

        
        public MainMenuBot(Update _update) : base(_update)
        {

        }

        protected override void Initializer()
        {
            
        }

        public async override Task<IActionResult> Response()
        {
            if (base.CommandName == "/start")
                return await SendMainMenu();

            else
                return null;
        }


        private async Task<IActionResult> SendMainMenu()
        {
            BotMessage =new Messages.MainMenuBotMessage();

            await SendMessage(base.ChatId, BotMessage.BuildMsg());

            return OkResult;
        }



    }
}
