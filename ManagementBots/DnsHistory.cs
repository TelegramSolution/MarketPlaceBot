using System;
using System.Collections.Generic;

namespace ManagementBots
{
    public partial class DnsHistory
    {
        public int Id { get; set; }
        public int? DnsId { get; set; }
        public int? BotId { get; set; }
        public DateTime? TimeStamp { get; set; }
    }
}
