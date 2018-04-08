using System;
using System.Collections.Generic;

namespace ManagementBots
{
    public partial class WebAppHistory
    {
        public int Id { get; set; }
        public DateTime? TimeStamp { get; set; }
        public int? BotId { get; set; }
        public int? WebAppId { get; set; }
    }
}
