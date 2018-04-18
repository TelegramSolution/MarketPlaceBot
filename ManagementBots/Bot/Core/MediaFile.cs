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

namespace ManagementBots.Bot.Core
{
    public class MediaFile
    {
        public FileToSend FileTo { get; set; }

        /// <summary>
        /// тип файла 1- фото, 2 - , 3- и т.д
        /// </summary>
        public int FileTypeId { get; set; }

        /// <summary>
        /// Id файла в таблице AttachmentFs (таблица в которой хранятся сами файлы)
        /// </summary>
        public int AttachmentFsId { get; set; }

        /// <summary>
        /// текстовое сообщние под файлом (фотографией или видео)
        /// </summary>
        public string Caption { get; set; }

    }
}
