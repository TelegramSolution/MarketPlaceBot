using System;
using System.Collections.Generic;

namespace ManagementBots.Db
{
    public partial class Bot
    {
        public Bot()
        {
            BotBlocked = new HashSet<BotBlocked>();
            BotDeleted = new HashSet<BotDeleted>();
            ServiceNavigation = new HashSet<Service>();
        }

        public int Id { get; set; }
        public string BotName { get; set; }
        public string Token { get; set; }
        public string Text { get; set; }
        public DateTime? CreateTimeStamp { get; set; }
        public int? FollowerId { get; set; }
        public int? WebAppId { get; set; }
        public int? DomainNameId { get; set; }
        public int? ServiceId { get; set; }
        public bool? Visable { get; set; }
        public bool? Deleted { get; set; }
        public bool? Blocked { get; set; }

        public Dns DomainName { get; set; }
        public Follower Follower { get; set; }
        public Service Service { get; set; }
        public WebApp WebApp { get; set; }
        public ICollection<BotBlocked> BotBlocked { get; set; }
        public ICollection<BotDeleted> BotDeleted { get; set; }
        public ICollection<Service> ServiceNavigation { get; set; }
    }
}
