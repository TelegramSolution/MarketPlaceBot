using System;
using System.Collections.Generic;

namespace ManagementBots.Db
{
    public partial class WebApp
    {
        public WebApp()
        {
            Bot = new HashSet<Bot>();
            WebAppHistory = new HashSet<WebAppHistory>();
        }

        public int Id { get; set; }
        public int? ServerWebAppId { get; set; }
        public string Port { get; set; }
        public bool Enable { get; set; }
        public bool IsFree { get; set; }

        public ServerWebApp ServerWebApp { get; set; }
        public ICollection<Bot> Bot { get; set; }
        public ICollection<WebAppHistory> WebAppHistory { get; set; }
    }
}
