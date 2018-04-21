using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.IO;

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

            catch (Exception e)
            {
                return false;
            }
        }

        public async static Task<User> GetMe(string Token)
        {

            TelegramBotClient botClient = new TelegramBotClient(Token);

            return await botClient.GetMeAsync();

        }

        public async static Task<WebhookInfo> SetWebHook(string Token, string PathCert, string WebHookUrl)
        {
            TelegramBotClient telegramBot = new TelegramBotClient(Token);

            FileToSend fileToSend = new FileToSend
            {
                Content = System.IO.File.OpenRead(PathCert),
                Filename = "@Cert.pem"
            };

             await telegramBot.SetWebhookAsync(WebHookUrl, fileToSend);

             return await telegramBot.GetWebhookInfoAsync();
        }

        public async static Task<WebhookInfo> SetWebHook(string Token, string WebHookUrl)
        {
            TelegramBotClient telegramBot = new TelegramBotClient(Token);

            await telegramBot.SetWebhookAsync(WebHookUrl);

            return await telegramBot.GetWebhookInfoAsync();
        }
    }
}
