﻿using System;
using System.Collections.Generic;

namespace ManagementBots
{
    public partial class HelpDeskAnswer
    {
        public int Id { get; set; }
        public int? HelpDeskId { get; set; }
        public DateTime? Timestamp { get; set; }
        public int? FollowerId { get; set; }
        public string Text { get; set; }
        public bool? Closed { get; set; }
        public DateTime? ClosedTimestamp { get; set; }

        public Follower Follower { get; set; }
        public HelpDesk HelpDesk { get; set; }
    }
}
