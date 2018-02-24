using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class HelpDeskInWork
    {
        public int Id { get; set; }
        public int? HelpDeskId { get; set; }
        public int? FollowerId { get; set; }
        public DateTime? Timestamp { get; set; }
        public bool? InWork { get; set; }

        public Follower Follower { get; set; }
        public HelpDesk HelpDesk { get; set; }
    }
}
