using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

namespace ManagementBots.BusinessLayer
{
    public class TelegramFunction
    {

        public async static Task<bool> SendTextMessage(string Text, int ChatId, string BotToken)
        {
            try
            {

                TelegramBotClient botClient = new TelegramBotClient(BotToken);

                await botClient.SendTextMessageAsync(ChatId, Text);

                return true;
            }

            catch(Exception e)
            {
                return false;
            }
        }
    }
}
