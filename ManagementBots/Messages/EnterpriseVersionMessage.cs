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
    public class EnterpriseVersionMessage:BotMessage
    {

        public override BotMessage BuildMsg()
        {
            base.TextMessage = "Серверная версия" + NewLine() +
                "Вы получите установщик и сможете установить бота на своем сервере" + NewLine() +
                "В серверной версии есть возможность приема криптовалютных платежей (Litecoin, Bitcoin, BitcoinCash, Doge, Zcash, Dash)" + NewLine() +
                "Полностью автоматическая установка" + NewLine() +
                "Системные требования: ОС Windows x64, ОЗУ 4гб" + NewLine() +
                "Стоимость: 3000 рублей" + NewLine()+
                "Для покупки серверной версии пишите сюда @tgsolution" + NewLine()+ NewLine()+
                "Назад /start";

            return this;
        }
    }
}
