using System;
using System.Collections.Generic;

namespace ManagementBots.Db
{
    public partial class WebHookUrlHistory
    {
        public int? WebHookUrlId { get; set; }
        public int Id { get; set; }
        public DateTime? Timestamp { get; set; }
        public int? BotId { get; set; }

        public Bot Bot { get; set; }
        public WebHookUrl WebHookUrl { get; set; }
    }
}
