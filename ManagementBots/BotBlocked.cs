using System;
using System.Collections.Generic;

namespace ManagementBots
{
    public partial class BotBlocked
    {
        public int Id { get; set; }
        public int? BotId { get; set; }
        public string Text { get; set; }
        public DateTime? BlockedTimeStamp { get; set; }
        public DateTime? UnblockedTimeStamp { get; set; }

        public Bot Bot { get; set; }
    }
}
