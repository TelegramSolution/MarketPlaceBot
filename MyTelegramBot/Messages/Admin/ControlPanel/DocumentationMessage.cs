using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyTelegramBot.Bot.Core;
using MyTelegramBot.Model;
using Newtonsoft.Json;
using Telegram.Bot.Types.ReplyMarkups;

namespace MyTelegramBot.Messages.Admin
{
    public class DocumentationMessage:BotMessage
    {
        List<Documentation> list { get; set; }

        public override BotMessage BuildMsg()
        {
            list = new List<Documentation>();

            list = JsonConvert.DeserializeObject<List<Documentation>>(ReadDocJson());

            base.TextMessage = "Документация"+NewLine();

            int count = 1;

            foreach (var doc in list)
            {
               base.TextMessage+=count.ToString() + ") " + HrefUrl(doc.Url, doc.Title) + NewLine() + NewLine();
                count++;
            }

            base.MessageReplyMarkup = new InlineKeyboardMarkup(new[] { new[] { BackToAdminPanelBtn() } });

            return base.BuildMsg();
        }

        private string ReadDocJson()
        {
            using(System.IO.StreamReader sr=new System.IO.StreamReader("Files\\doc.json"))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
