using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Microsoft.AspNetCore;

namespace UnitTestMarketPlaceBot
{
    [TestClass]
    public class UnitTest1
    {
        Update update { get; set; }

        [TestMethod]
        public void TestMethod1()
        {
            StreamReader stream = new StreamReader("Update.Message.BotCommand.json");
            string text = stream.ReadToEnd();

            update = JsonConvert.DeserializeObject<Update>(text);
            update.Message.Text = "/start";

            MyTelegramBot.Bot.MainMenuBot mainMenuBot = new MyTelegramBot.Bot.MainMenuBot(update);
              mainMenuBot.Response();

            //MyTelegramBot.Messages.MainMenuBotMessage m = new MyTelegramBot.Messages.MainMenuBotMessage();
            //var result= m.BuildMsg();
            //Assert.IsNotNull(result);
        }
    }
}
