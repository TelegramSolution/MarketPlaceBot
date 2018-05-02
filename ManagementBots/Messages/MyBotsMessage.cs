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
using ManagementBots.Bot;
using ManagementBots.Bot.Core;
using ManagementBots.Db;


namespace ManagementBots.Messages
{
    public class MyBotsMessage : BotMessage
    {
        List<Db.Bot> BotList { get; set; }

        int FollowerId { get; set; }

        Db.BotMngmntDbContext DbContext { get; set; }

        InlineKeyboardCallbackButton[][] BotBtns { get; set; }

        public MyBotsMessage(int FollowerId)
        {
            this.FollowerId = FollowerId;
        }

        public override BotMessage BuildMsg()
        {
            DbContext = new Db.BotMngmntDbContext();

            BotList = DbContext.Bot.Where(b => b.Visable && b.FollowerId == FollowerId && !b.Deleted).ToList();

            DbContext.Dispose();

            base.TextMessage = "Мои боты";

            if (BotList.Count > 0)
            {
                BotBtns = new InlineKeyboardCallbackButton[BotList.Count+1][];

                int count=0;

                foreach(var bot in BotList)
                {
                    BotBtns[count] = new InlineKeyboardCallbackButton[1];

                    if(!bot.Blocked)
                     BotBtns[count][0] = BuildInlineBtn(bot.BotName, BuildCallData(ConnectBot.GetBotCmd, ConnectBot.ModuleName, bot.Id));

                    else
                        BotBtns[count][0] = BuildInlineBtn(bot.BotName + " (Заблокирован)", BuildCallData(ConnectBot.GetBotCmd, ConnectBot.ModuleName, bot.Id));

                    count++;
                }

                BotBtns[BotBtns.Length - 1] = new InlineKeyboardCallbackButton[1];
                BotBtns[BotBtns.Length - 1][0] = BuildInlineBtn("Назад", BuildCallData(MainMenuBot.ToMainMenuCmd, MainMenuBot.ModuleName));

                base.MessageReplyMarkup = new InlineKeyboardMarkup(BotBtns);

                return this;
            }

            else
                return null;

        }
    }
}
