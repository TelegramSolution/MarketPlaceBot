using System;
using System.Collections.Generic;

namespace ManagementBots
{
    public partial class Configuration
    {
        public int Id { get; set; }
        public string ManualFileId { get; set; }
        public string PrivateGroupChatId { get; set; }
        public int? BotInfoId { get; set; }
        public bool? VerifyTelephone { get; set; }
        public bool? OwnerPrivateNotify { get; set; }
        public string UserNameFaqFileId { get; set; }

        public BotInfo BotInfo { get; set; }
    }
}
