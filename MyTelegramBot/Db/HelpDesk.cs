using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class HelpDesk
    {
        public HelpDesk()
        {
            HelpDeskAnswer = new HashSet<HelpDeskAnswer>();
            HelpDeskAttachment = new HashSet<HelpDeskAttachment>();
            HelpDeskInWork = new HashSet<HelpDeskInWork>();
        }

        public int Id { get; set; }
        public int? Number { get; set; }
        public DateTime? Timestamp { get; set; }
        public int? FollowerId { get; set; }
        public string Text { get; set; }
        public bool Send { get; set; }
        public bool Closed { get; set; }
        public bool InWork { get; set; }
        public int? BotInfoId { get; set; }

        public BotInfo BotInfo { get; set; }
        public Follower Follower { get; set; }
        public ICollection<HelpDeskAnswer> HelpDeskAnswer { get; set; }
        public ICollection<HelpDeskAttachment> HelpDeskAttachment { get; set; }
        public ICollection<HelpDeskInWork> HelpDeskInWork { get; set; }
    }
}
