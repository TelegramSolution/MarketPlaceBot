using System;
using System.Collections.Generic;

namespace ManagementBots.Db
{
    public partial class ReserveWebHookUrl
    {
        public int BotId { get; set; }
        public int WebHookUrlId { get; set; }
        public DateTime? TimeStampStart { get; set; }
        public DateTime? TimeStampEnd { get; set; }

        public Bot Bot { get; set; }
        public WebHookUrl WebHookUrl { get; set; }
    }
}
