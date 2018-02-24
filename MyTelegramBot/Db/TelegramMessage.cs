using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTelegramBot
{
    public partial class TelegramMessage
    {
        public int Id { get; set; }
        public int? UpdateId { get; set; }
        public string MessageId { get; set; }
        public int? FollowerId { get; set; }
        public string UpdateJson { get; set; }
        public DateTime? DateAdd { get; set; }
        public int? BotInfoId { get; set; }

        public BotInfo BotInfo { get; set; }
        public Follower Follower { get; set; }
    }
}
