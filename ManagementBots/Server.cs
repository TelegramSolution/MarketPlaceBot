using System;
using System.Collections.Generic;

namespace ManagementBots
{
    public partial class Server
    {
        public int Id { get; set; }
        public string ServerName { get; set; }
        public string Ip { get; set; }
        public string WanIp { get; set; }
        public bool? Enable { get; set; }
    }
}
