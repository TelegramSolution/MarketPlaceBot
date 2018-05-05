using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class Answer
    {
        public Answer()
        {
            ProductQuestion = new HashSet<ProductQuestion>();
        }

        public int Id { get; set; }
        public int? FollowerId { get; set; }
        public DateTime? TimeStamp { get; set; }
        public string Text { get; set; }

        public Follower Follower { get; set; }
        public ICollection<ProductQuestion> ProductQuestion { get; set; }
    }
}
