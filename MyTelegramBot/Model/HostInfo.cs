using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTelegramBot.Model
{
    public class HostInfo
    {
        public string BotName { get; set; }

        public string Token { get; set; }

        public string AppPoolName { get; set; }

        public string SiteName { get; set; }

        public string HttpPort { get; set; }

        public string Path { get; set; }

        public string DbName { get; set; }

        public string DbConnectionString { get; set; }

        public string HostName { get; set; }

        public DateTime CreateTimeStamp { get; set; }

        public string UrlWebHook { get; set; }

        public bool IsFree { get; set; }

        public bool IsDemo { get; set; }

        public bool Blocked { get; set; }

        public int OwnerChatId { get; set; }

    }
}
