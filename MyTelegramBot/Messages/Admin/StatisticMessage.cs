using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;
using System.IO;
using Telegram.Bot.Types;

namespace MyTelegramBot.Messages.Admin
{
    public class StatisticMessage
    {
        private BotInfo BotInfo { get; set; }
        /// <summary>
        /// Кол-во Сообщений в разере по дня
        /// </summary>
        private const string MessageCountStatSql = "Select  COUNT(*), CAST(TelegramMessage.DateAdd as date) FROM TelegramMessage GROUP BY CAST(TelegramMessage.DateAdd as date) ";

        /// <summary>
        /// Количество новых пользователей за сегодня
        /// </summary>
        private const string NewFollowerStatToDaySql = "Select Id FROM Follower WHERE CAST(Follower.DateAdd as date)=CAST(GETDATE() as date)";

        /// <summary>
        /// Количество сообщений за сегодня
        /// </summary>
        private const string MessageCountStatToDaySql = "Select Id FROM TelegramMessage WHERE CAST(TelegramMessage.DateAdd as date)=CAST(GETDATE() as date)";

        /// <summary>
        /// Статистика зарегистрированный пользователей в разрезе по дня
        /// </summary>
        private const string FollowerStatSql = "Select  COUNT(*), CAST(Follower.DateAdd as date) FROM Follower GROUP BY CAST(Follower.DateAdd as date)";

        BotMessage[] Messages = new Bot.BotMessage[3]; 

        public StatisticMessage()
        {
            //this.BotInfo = botInfo;
    
        }

        public BotMessage[] BuildMessage()
        {
            Messages[0] = StaticMsg();
            Messages[1] = BuildMessageStatMsg();
            Messages[2] = BuildFollowerRegisterStatMsg();

            return Messages;
        }

        private BotMessage StaticMsg()
        {
            BotMessage message = new BotMessage();
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                try
                {
                    var NewFollowerStatToDayCount = db.Follower.FromSql(NewFollowerStatToDaySql).Count();

                    var MessageCountStatToDayCount = db.TelegramMessage.FromSql(MessageCountStatToDaySql).Count();



                    message.TextMessage = "Статистика всех ботов" + Bot.BotMessage.NewLine() + Bot.BotMessage.Bold("Количество отправленных ботам сообещний за сегодняшний день: ") + MessageCountStatToDayCount.ToString() +
                                       Bot.BotMessage.NewLine() + Bot.BotMessage.Bold("Количество новых пользователей за сегодняшний день: ") + NewFollowerStatToDayCount.ToString() +
                                         Bot.BotMessage.NewLine() + Bot.BotMessage.Bold("Всего пользователей: ") + db.Follower.ToList().Count.ToString() +
                                         Bot.BotMessage.NewLine() + Bot.BotMessage.Bold("Общее количество сообщений отправленных ботам за весь период:") + db.TelegramMessage.ToList().Count.ToString();

                    return message;
                }

                catch (Exception exp)
                {
                    return null;
                }
            }
        }

        private BotMessage BuildMessageStatMsg()
        {
            BotMessage message = new BotMessage();

            string column_name = "Дата;Количество сообщений;";

            try
            {
                ///Выгружаем Статистику сообщений в разерезе по дня
                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    var list = db.TelegramMessage.GroupBy(t => t.DateAdd.Value.Date).ToList();
                    message.MediaFile = new MediaFile
                    {
                        Caption = "Статистика сообщений в разрезе по дням",
                        FileTo = BuildCsv<TelegramMessage>(list, column_name, "MessageStat.csv"),
                        TypeFileTo= EnumMediaFile.Document
                    };
                    
                    
                    return message;
                }
            }
            catch (Exception exp)
            {
                return null;
            }
        }

        private BotMessage BuildFollowerRegisterStatMsg()
        {
            BotMessage message = new BotMessage();

            string column_name = "Дата;Количество новых пользователей;";

            try
            {
                ///Выгружаем Статистику зарегистрированных пользователей
                ///в разерезе по дня
                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    var list = db.Follower.GroupBy(f => f.DateAdd.Value.Date).ToList();
                    message.MediaFile = new MediaFile
                    {
                        Caption = "Количество зарегестрированных пользователей в разрезе по дням",
                        FileTo = BuildCsv<Follower>(list, column_name, "FollowerRegisterStat.csv"),
                        TypeFileTo = EnumMediaFile.Document
                    };

                    return message;
                }
            }

            catch (Exception exp)
            {
                return null;
            }
        }

        private FileToSend BuildCsv<T>(List<IGrouping<DateTime,T>> list,string column_name, string filename)
        {
            string Line = "";

            string tmpLine = "";

            foreach (var date in list)
            {

                var date_value = date.Key.Date.ToString().Substring(0, 10);
                var count = date.Count();
                tmpLine = String.Format("{0};{1};", date_value, count.ToString())+ Bot.BotMessage.NewLine();
                Line += tmpLine;
            }

            GeneralFunction.WriteToFile(column_name + Bot.BotMessage.NewLine() + Line, filename);


            return new FileToSend { Filename = filename, Content = GeneralFunction.ReadFile(filename) };
        }
    }
}
