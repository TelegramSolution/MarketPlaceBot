using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class OrdersInWork
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public DateTime? Timestamp { get; set; }
        public int? FollowerId { get; set; }
        public bool? InWork { get; set; }

        public Follower Follower { get; set; }
        public Orders Order { get; set; }
    }
}
