using System;
using System.Collections.Generic;

namespace ManagementBots.Db
{
    public partial class Server
    {
        public Server()
        {
            WebApp = new HashSet<WebApp>();
        }

        public int Id { get; set; }
        public string ServerName { get; set; }
        public string Ip { get; set; }
        public string WanIp { get; set; }
        public bool Enable { get; set; }
        public string Text { get; set; }

        public ICollection<WebApp> WebApp { get; set; }
    }
}
