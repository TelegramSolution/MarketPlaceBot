using System;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyTelegramBot.Messages;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Bot
{
    public class MainMenuBot: BotCore
    {

        public const string ModuleName = "Main";

        MainMenuBotMessage MainMenuMsg { get; set; }

        ContactMessage ContactMsg { get; set; }

        
        public MainMenuBot(Update _update) : base(_update)
        {

        }

        protected override void Initializer()
        {
            MainMenuMsg = new MainMenuBotMessage();
            ContactMsg = new ContactMessage();
        }

        public async override Task<IActionResult> Response()
        {
          

            if (CommandName == "/start")
                return await SendMainMenu();

            if (CommandName == "MainMenu")
                return await SendMainMenu(MessageId);

            if (base.CommandName == "Contact")
                return await SendContactList();


            else return null;
        }


        private async Task<IActionResult> SendMainMenu(int MessageId=0)
        {
            AddUser();
            if (await SendMessage(MainMenuMsg.BuildMsg(),MessageId) != null)
                return base.OkResult;

            else
                return base.OkResult;
        }

        private async Task<IActionResult> SendContactList()
        {
            if (await SendMessage(ContactMsg.BuildMessage()) != null)
                return base.OkResult;

            else
                return base.OkResult;
        }

        private void AddUser()
        {
            using (MarketBotDbContext db=new MarketBotDbContext()) {
                if (base.FollowerId < 1)
                {
                    Follower follower = new Follower
                    {
                        FirstName = Update.Message.Chat.FirstName,
                        LastName = Update.Message.Chat.LastName,
                        UserName = Update.Message.Chat.Username,
                        ChatId = Convert.ToInt32(Update.Message.Chat.Id),
                        DateAdd = DateTime.Now,
                        Blocked=false
                        

                    };

                    db.Follower.Add(follower);
                    db.SaveChanges();
                }
            }
        }


    }
}
