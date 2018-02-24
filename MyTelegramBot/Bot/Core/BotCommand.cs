using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTelegramBot.Bot
{
    public class BotCommand
    {
        /// <summary>
        /// название команды
        /// </summary>
        public string Cmd { get; set; }

        /// <summary>
        /// параметр. если несколько то чере &
        /// </summary>
        public List<int> Arg { get; set; }

        /// <summary>
        /// Название модуля к которому относится команда
        /// </summary>
        public string M { get; set; }
    }
}
