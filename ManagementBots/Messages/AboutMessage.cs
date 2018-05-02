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

namespace ManagementBots.Messages
{
    public class AboutMessage:BotMessage
    {
        public override BotMessage BuildMsg()
        {
            base.TextMessage = "Чат бот для приема заказов черезе Telegram. " + NewLine() +
                "Особенности:" + NewLine() +
                base.BlueRhombus + "Служба поддержки для ваших пользователей" + NewLine() +
                base.BlueRhombus + " Прием платежей Qiwi, Банковской картой" + NewLine() +
                base.BlueRhombus + "Удобный поиск товаров" + NewLine() +
                base.BlueRhombus + "Многопользовательский режим обработки заказов и заявок" + NewLine() +
                base.BlueRhombus + "Панель администрирования через диалог с ботом" + NewLine() +
                base.BlueRhombus + "Мгновенные уведомления при добавлении заказа / заявки (desk) / платежа" + NewLine() +
                base.BlueRhombus + "Поиск по заказам, платежам, заявкам, пользователям" + NewLine() +
                base.BlueRhombus + "Выгрузка данных из бота в формате Excel" + NewLine() + NewLine() +
                "назад /start";

            return this;
        }
    }
}
