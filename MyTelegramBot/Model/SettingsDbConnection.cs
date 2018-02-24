using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTelegramBot.Model
{
    public class SettingsDbConnection
    {
        public string Host { get; set; }

        public string Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string DbName { get; set; }
    }
}
