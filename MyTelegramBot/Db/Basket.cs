using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class Basket
    {
        public int Id { get; set; }
        public int FollowerId { get; set; }
        public int ProductId { get; set; }
        public int Amount { get; set; }
        public DateTime? DateAdd { get; set; }
        public bool Enable { get; set; }
        public int? BotInfoId { get; set; }

        public BotInfo BotInfo { get; set; }
        public Follower Follower { get; set; }
        public Product Product { get; set; }
    }
}
