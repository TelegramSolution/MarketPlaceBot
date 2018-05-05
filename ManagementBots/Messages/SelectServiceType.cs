using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementBots.Bot.Core;
using ManagementBots.Db;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using ManagementBots.Bot;

namespace ManagementBots.Messages
{
    public class SelectServiceType:BotMessage
    {

        public SelectServiceType(int BotId)
        {
            this.BotId = BotId;
        }

        private int BotId { get; set; }

        private List<ServiceType> ServiceTypeList { get; set; }

        private InlineKeyboardCallbackButton [][] ServiceTypeBtns { get; set; }

        BotMngmntDbContext DbContext { get; set; }

        public override BotMessage BuildMsg()
        {
            DbContext = new BotMngmntDbContext();

            ServiceTypeList = DbContext.ServiceType.Where(s => s.Enable).ToList();

            DbContext.Dispose();

            BackBtn = base.BuildInlineBtn("Назад", base.BuildCallData("BackToMainMenu", MainMenuBot.ModuleName), base.Previuos2Emodji);

            ServiceTypeBtns = new InlineKeyboardCallbackButton[ServiceTypeList.Count+2][];

            ServiceTypeBtns[ServiceTypeBtns.Length-1] = new InlineKeyboardCallbackButton[1];
            ServiceTypeBtns[ServiceTypeBtns.Length-1][0] = BackBtn;

            ServiceTypeBtns[ServiceTypeBtns.Length - 2] = new InlineKeyboardCallbackButton[1];
            ServiceTypeBtns[ServiceTypeBtns.Length - 2][0] = BuildInlineBtn("Серверная версия. 3000 рублей",BuildCallData(ConnectBot.EnterpriseVersionCmd,ConnectBot.ModuleName));

            int count = 0;

            foreach (var service in ServiceTypeList)
            {
                ServiceTypeBtns[count] = new InlineKeyboardCallbackButton[1];
                ServiceTypeBtns[count][0] = BuildInlineBtn(service.Name, BuildCallData(ConnectBot.SelectServiceTypeCmd, ConnectBot.ModuleName, BotId, service.Id));
                count++;
            }

            base.TextMessage = "Выберите тариф";

            base.MessageReplyMarkup = new InlineKeyboardMarkup(ServiceTypeBtns);

            return this;


        }
    }
}
