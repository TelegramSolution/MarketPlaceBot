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

namespace MyTelegramBot.Bot
{
    public class MediaFile
    {
        public FileToSend FileTo { get; set; }

        public EnumMediaFile TypeFileTo { get; set; }

        /// <summary>
        /// Id файла в таблице AttachmentFs (таблица в которой храняться сами файлы)
        /// </summary>
        public int AttachmentFsId { get; set; }

        /// <summary>
        /// текстовое сообщние под файлом
        /// </summary>
        public string Caption { get; set; }

        public static EnumMediaFile HowMediaType(int? TypeId)
        {
            if (TypeId == 1)
                return EnumMediaFile.Photo;

            if (TypeId == 2)
                return EnumMediaFile.Video;

            if (TypeId == 3)
                return EnumMediaFile.Audio;

            if (TypeId == 4)
                return EnumMediaFile.Voice;

            if (TypeId == 5)
                return EnumMediaFile.VideoNote;

            if (TypeId == 6)
                return EnumMediaFile.Document;

            else
                return EnumMediaFile.Document;
        }
    }
}
