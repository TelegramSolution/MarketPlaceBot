using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class Notification
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? FollowerId { get; set; }
        public DateTime? DateAdd { get; set; }

        public Follower Follower { get; set; }
        public Product Product { get; set; }
    }
}
