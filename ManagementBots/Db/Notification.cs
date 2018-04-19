using System;
using System.Collections.Generic;

namespace ManagementBots.Db
{
    public partial class Notification
    {
        public int Id { get; set; }
        public int? FollowerId { get; set; }
        public DateTime? DateAdd { get; set; }
        public string Text { get; set; }
        public bool? Sended { get; set; }

        public Follower Follower { get; set; }
    }
}
