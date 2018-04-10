using System;
using System.Collections.Generic;

namespace ManagementBots.Db
{
    public partial class WebApp
    {
        public WebApp()
        {
            Bot = new HashSet<Bot>();
        }

        public int Id { get; set; }
        public int? ServerId { get; set; }
        public string Port { get; set; }
        public bool? Enable { get; set; }

        public ICollection<Bot> Bot { get; set; }
    }
}
