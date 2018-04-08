using System;
using System.Collections.Generic;

namespace ManagementBots
{
    public partial class Bot
    {
        public Bot()
        {
            BotBlocked = new HashSet<BotBlocked>();
            BotDeleted = new HashSet<BotDeleted>();
            ServiceList = new HashSet<ServiceList>();
        }

        public int Id { get; set; }
        public string BotName { get; set; }
        public string Token { get; set; }
        public string Text { get; set; }
        public DateTime? CreateTimeStamp { get; set; }
        public int? FollowerId { get; set; }
        public int? WebAppId { get; set; }
        public int? DomainNameId { get; set; }
        public int? CurrentServiceId { get; set; }

        public ServiceList CurrentService { get; set; }
        public Dns DomainName { get; set; }
        public Follower Follower { get; set; }
        public WebApp WebApp { get; set; }
        public ICollection<BotBlocked> BotBlocked { get; set; }
        public ICollection<BotDeleted> BotDeleted { get; set; }
        public ICollection<ServiceList> ServiceList { get; set; }
    }
}
