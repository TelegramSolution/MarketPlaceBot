using System;
using System.Collections.Generic;

namespace ManagementBots
{
    public partial class BotDeleted
    {
        public int Id { get; set; }
        public int? BotId { get; set; }
        public DateTime? DeletedTimeStamp { get; set; }

        public Bot Bot { get; set; }
    }
}
