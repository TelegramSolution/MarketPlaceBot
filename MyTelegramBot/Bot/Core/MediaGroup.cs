using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace MyTelegramBot.Bot
{
    public class MediaGroup
    {
        /// <summary>
        /// ассоциативный массив Файл в бд и соответствующий ему FileId на сервере телеграм
        /// </summary>
        public Dictionary<int,string> FsIdTelegramFileId { get; set; }

        public List<InputMediaBase> ListMedia { get; set; }


    }
}
