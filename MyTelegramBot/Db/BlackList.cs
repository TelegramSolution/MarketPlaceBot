using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class BlackList
    {
        public int Id { get; set; }
        public DateTime DateAdd { get; set; }
        public int Duration { get; set; }
        public int FollowerId { get; set; }
        public bool? Deleted { get; set; }

        public Follower Follower { get; set; }
    }
}
