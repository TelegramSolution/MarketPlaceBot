using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class ProductQuestion
    {
        public int Id { get; set; }
        public int? FollowerId { get; set; }
        public DateTime? TimeStamp { get; set; }
        public int? ProductId { get; set; }
        public int? AnswerId { get; set; }
        public string Text { get; set; }

        public Answer Answer { get; set; }
        public Follower Follower { get; set; }
        public Product Product { get; set; }
    }
}
