using System;
using System.Collections.Generic;

namespace ManagementBots
{
    public partial class BotInfo
    {
        public BotInfo()
        {
            AttachmentTelegram = new HashSet<AttachmentTelegram>();
            HelpDesk = new HashSet<HelpDesk>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? ChatId { get; set; }
        public string Token { get; set; }
        public DateTime? Timestamp { get; set; }
        public int? OwnerChatId { get; set; }
        public string WebHookUrl { get; set; }

        public Configuration Configuration { get; set; }
        public ICollection<AttachmentTelegram> AttachmentTelegram { get; set; }
        public ICollection<HelpDesk> HelpDesk { get; set; }
    }
}
