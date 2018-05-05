using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;
using System.Web;
using Telegram.Bot.Types.InlineKeyboardButtons;
using ManagementBots.Bot;
using ManagementBots.Bot.Core;
using ManagementBots.Db;

namespace ManagementBots.Messages
{
    public class EnterDurationServiceMessage : BotMessage
    {

        private int ServiceTypeId { get; set; }

        private int BotId { get; set; }

        private Db.Bot Bot { get; set; }

        private ServiceType ServiceType { get; set; }

        private BotMngmntDbContext DbContext { get; set; }

        private const int Day15 = 15;

        private const int Day30 = 30;

        private const int Day60 = 60;


        public EnterDurationServiceMessage(int ServiceTypeId, int BotId)
        {
            this.ServiceTypeId = ServiceTypeId;
            this.BotId = BotId;
        }



        public override BotMessage BuildMsg()
        {
            DbContext = new BotMngmntDbContext();

            ServiceType = DbContext.ServiceType.Find(Convert.ToInt32(ServiceTypeId));

            base.TextMessage = ServiceType.Name + NewLine() + "Минимальная продолжительность:" + ServiceType.MinDuration.ToString();

            base.MessageReplyMarkup = SetKeyboard();

            return this;
        }

        private InlineKeyboardMarkup SetKeyboard()
        {
            return new InlineKeyboardMarkup(
                new[]{
                        new[]
                        {
                            BuildInlineBtn(ServiceType.MinDuration.ToString() +" дней",
                                            BuildCallData(ConnectBot.EnterDurationCmd,ConnectBot.ModuleName,BotId,ServiceType.Id,Convert.ToInt32(ServiceType.MinDuration)))
                        },

                        new[]
                        {
                            BuildInlineBtn(Day15.ToString() +" дней",
                                            BuildCallData(ConnectBot.EnterDurationCmd,ConnectBot.ModuleName,BotId,ServiceType.Id,Day15))
                        },
                        new[]
                        {
                            BuildInlineBtn(Day30.ToString() +" дней",
                                            BuildCallData(ConnectBot.EnterDurationCmd,ConnectBot.ModuleName,BotId,ServiceType.Id,Day30))
                        },
                        new[]
                        {
                            BuildInlineBtn(Day60.ToString() +" дней",
                                            BuildCallData(ConnectBot.EnterDurationCmd,ConnectBot.ModuleName,BotId,ServiceType.Id,Day60))
                        }
                });
        }
    }
}
