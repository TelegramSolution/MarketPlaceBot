using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTelegramBot
{
    public class ReportsRequestLog
    {
        public int Id { get; set; }
        public DateTime? DateAdd { get; set; }
        public int? FollowerId { get; set; }

        public Follower Follower { get; set; }
    }
}
