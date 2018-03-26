using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot.AdminModule;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages.Admin.PaymentsModule
{
    /// <summary>
    /// список всех номеров телефонов Qiwi который подключены к боту
    /// </summary>
    public class QiwiListMessage:BotMessage
    {

        private List<PaymentTypeConfig> PaymentTypeConfigList { get; set; }

        private MarketBotDbContext db { get; set; }

        private Dictionary<int,List<PaymentTypeConfig>> Pages { get; set; }

        public QiwiListMessage(int selectpage=1)
        {
            base.SelectPageNumber = selectpage;
            base.PageSize = 5;
        }


        public override BotMessage BuildMsg()
        {
            db = new MarketBotDbContext();

            PaymentTypeConfigList = db.PaymentTypeConfig.Where(p => p.PaymentId == ConstantVariable.PaymentTypeVariable.QIWI).ToList();

            base.TextMessage =base.CreditCardEmodji+ "Что бы добавить новую запись нажмите сюда /addqiwi"+NewLine()+
                HrefUrl("https://qiwi.com/api","Получить Qiwi Api")+NewLine()+NewLine();

            Pages = base.BuildDataPage<PaymentTypeConfig>(PaymentTypeConfigList, base.PageSize);

            this.BackBtn = BuildInlineBtn("Назад", BuildCallData(MoreSettingsBot.SettingsPaymentMethodCmd, MoreSettingsBot.ModuleName), base.Previuos2Emodji, false);

            base.MessageReplyMarkup = base.PageNavigatorKeyboard<PaymentTypeConfig>(Pages, MoreSettingsBot.QiwiListCmd, MoreSettingsBot.ModuleName, this.BackBtn);

            if (Pages != null && Pages.Count > 0 && Pages[SelectPageNumber] != null)
            {

                var page = Pages[SelectPageNumber];

                int number = 1; // порядковый номер записи

                int counter = 1;

                foreach (var p in page)
                {
                    number = PageSize * (SelectPageNumber - 1) + counter;

                    base.TextMessage += number.ToString() + ") Номер телефона:" + p.Login + NewLine() +
                        "Токен доступа:" + p.Pass + NewLine()
                        + "Изменить: /paycgf" + p.Id.ToString()+NewLine()+NewLine();

                    counter++;
                }

                
            }

            db.Dispose();

            return this;
        }
    }
}
