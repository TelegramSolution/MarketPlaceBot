using System;
using System.Collections.Generic;

namespace ManagementBots.Db
{
    public partial class Dns
    {
        public Dns()
        {
            Bot = new HashSet<Bot>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
        public string SslPath { get; set; }
        public DateTime? TimeStamp { get; set; }
        public bool? Enable { get; set; }

        public ICollection<Bot> Bot { get; set; }
    }
}
