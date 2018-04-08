using System;
using System.Collections.Generic;

namespace ManagementBots
{
    public partial class Follower
    {
        public Follower()
        {
            Bot = new HashSet<Bot>();
            HelpDesk = new HashSet<HelpDesk>();
            HelpDeskAnswer = new HashSet<HelpDeskAnswer>();
            HelpDeskInWork = new HashSet<HelpDeskInWork>();
            Notification = new HashSet<Notification>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public int? ChatType { get; set; }
        public bool? Blocked { get; set; }
        public string Telephone { get; set; }
        public int? ChatId { get; set; }
        public DateTime? DateAdd { get; set; }

        public ICollection<Bot> Bot { get; set; }
        public ICollection<HelpDesk> HelpDesk { get; set; }
        public ICollection<HelpDeskAnswer> HelpDeskAnswer { get; set; }
        public ICollection<HelpDeskInWork> HelpDeskInWork { get; set; }
        public ICollection<Notification> Notification { get; set; }
    }
}
