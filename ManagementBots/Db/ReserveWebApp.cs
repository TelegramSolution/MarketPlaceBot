using System;
using System.Collections.Generic;

namespace ManagementBots.Db
{
    public partial class ReserveWebApp
    {
        public int BotId { get; set; }
        public int WebAppId { get; set; }
        public DateTime? TimeStampStart { get; set; }
        public DateTime? TimeStampEnd { get; set; }

        public Bot Bot { get; set; }
        public WebApp WebApp { get; set; }
    }
}
