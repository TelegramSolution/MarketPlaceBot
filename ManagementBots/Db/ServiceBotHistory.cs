using System;
using System.Collections.Generic;

namespace ManagementBots.Db
{
    public partial class ServiceBotHistory
    {
        public int ServiceId { get; set; }
        public int? BotId { get; set; }

        public Bot Bot { get; set; }
        public Service Service { get; set; }
    }
}
